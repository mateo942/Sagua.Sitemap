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
        private readonly ISitemapNodeRepository _sitemapNodeRepository;
        private readonly MatchOptions _matchOptions;
        private readonly ILogger<DumyMatchSitemapNode> _logger;

        public DumyMatchSitemapNode(ISitemapNodeRepository sitemapNodeRepository, IOptions<MatchOptions> matchOptions,
            ILogger<DumyMatchSitemapNode> logger)
        {
            _sitemapNodeRepository = sitemapNodeRepository;
            _matchOptions = matchOptions.Value;
            _logger = logger;
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
            var nodes = await _sitemapNodeRepository.GetListAsync(new Queries.SitemapNodeQuery
            {
                Path = path
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
