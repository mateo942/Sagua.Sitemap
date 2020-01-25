using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sagua.Sitemap.Models;
using Sagua.Sitemap.Options;
using Sagua.Sitemap.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Router
{
    public class DumyMatchSitemapNode : IMatchSitemapNode
    {
        private const string BY_PATH_CACHE = "DumyMatch_Path_{0}";
        private string GetCacheNameForPath(string path)
            => string.Format(BY_PATH_CACHE, path);

        private readonly ISitemapNodeRepository _sitemapNodeRepository;
        private readonly MatchOptions _matchOptions;
        private readonly ILogger<DumyMatchSitemapNode> _logger;
        private readonly IMemoryCache _memoryCache;

        public DumyMatchSitemapNode(ISitemapNodeRepository sitemapNodeRepository, IOptions<MatchOptions> matchOptions,
            ILogger<DumyMatchSitemapNode> logger, IMemoryCache memoryCache)
        {
            _sitemapNodeRepository = sitemapNodeRepository;
            _matchOptions = matchOptions.Value;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<SitemapNode>> FindByName(string name)
        {
            var nodes = await _sitemapNodeRepository.GetListAsync(new Queries.SitemapNodeQuery
            {
                Name = name
            });

            return new List<SitemapNode>(nodes);
        }

        public async Task<IEnumerable<SitemapNode>> FindByPath(string path)
        {
            if (!string.IsNullOrEmpty(_matchOptions.BasePath))
            {
                path = path.Replace(_matchOptions.BasePath, "");
            }

            var nodes = await _memoryCache.GetOrCreateAsync(GetCacheNameForPath(path), async entry =>
            {
                entry.SetSlidingExpiration(_matchOptions.SlidingExpiration);

                var nodes = await _sitemapNodeRepository.GetListAsync(new Queries.SitemapNodeQuery
                {
                    Path = path
                });

                if(nodes == null || nodes.Any() == false)
                {
                    var dynamicNodes = await _sitemapNodeRepository.GetListAsync(new Queries.SitemapNodeQuery
                    {
                        NodeType = NodeType.UniqueDefault
                    });

                    var passNode = new List<SitemapNode>();
                    foreach (var node in dynamicNodes)
                    {
                        var a = new Ant(node.Path);
                        if (a.IsMatch(path))
                        {
                            passNode.Add(node);
                        }
                    }
                    nodes = passNode;
                }

                return nodes;
            });

            return new List<SitemapNode>(nodes);
        }

        public async Task<IEnumerable<SitemapNode>> FindByUri(Uri uri)
        {
            var nodes = await _sitemapNodeRepository.GetListAsync(new Queries.SitemapNodeQuery
            {
                Path = uri.ToString()
            });

            return new List<SitemapNode>(nodes);
        }
    }

    /// <summary>
    /// Represents a class which matches paths using ant-style path matching.
    /// </summary>
    [DebuggerDisplay("Pattern = {regex}")]
    internal class Ant
    {
        private readonly string originalPattern;
        private readonly Regex regex;

        /// <summary>
        /// Initializes a new <see cref="Ant"/>.
        /// </summary>
        /// <param name="pattern">Ant-style pattern.</param>
        public Ant(string pattern)
        {
            originalPattern = pattern ?? string.Empty;
            regex = new Regex(
                EscapeAndReplace(originalPattern),
                RegexOptions.Singleline
            );
        }

        /// <summary>
        /// Validates whether the input matches the given pattern.
        /// </summary>
        /// <param name="input">Path for which to check if it matches the ant-pattern.</param>
        /// <returns>Whether the input matches the pattern.</returns>
        /// <inheritdoc/>
        public bool IsMatch(string input)
        {
            input = input ?? string.Empty;
            return regex.IsMatch(GetUnixPath(input));
        }

        private static string EscapeAndReplace(string pattern)
        {
            var unix = GetUnixPath(pattern);

            if (unix.EndsWith("/"))
            {
                unix += "**";
            }

            pattern = Regex.Escape(unix)
                .Replace(@"/\*\*/", "(.*[/])")
                .Replace(@"\*\*/", "(.*)")
                .Replace(@"/\*\*", "(.*)")
                .Replace(@"\*", "([^/]*)")
                .Replace(@"\?", "(.)")
                .Replace(@"}", ")")
                .Replace(@"\{", "(")
                .Replace(@",", "|");

            return $"^{pattern}$";
        }

        private static string GetUnixPath(string txt) => txt.Replace(@"\", "/").TrimStart('/');

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => originalPattern;
    }
}
