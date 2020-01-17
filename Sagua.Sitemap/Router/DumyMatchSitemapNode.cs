using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sagua.Sitemap.Models;
using Sagua.Sitemap.Options;
using Sagua.Sitemap.Repository;
using System;
using System.Collections.Generic;
using System.Text;
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
}
