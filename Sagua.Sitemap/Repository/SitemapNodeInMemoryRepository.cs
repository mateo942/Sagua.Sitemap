using Microsoft.Extensions.Logging;
using Sagua.Sitemap.Models;
using Sagua.Sitemap.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Repository
{
    public class SitemapNodeInMemoryRepository : ISitemapNodeRepository
    {
        private static readonly ISet<SitemapNode> _sitemapNodes = new HashSet<SitemapNode>();

        private readonly ILogger<SitemapNodeInMemoryRepository> _logger;

        public SitemapNodeInMemoryRepository(ILogger<SitemapNodeInMemoryRepository> logger)
        {
            _logger = logger;
        }

        public Task<SitemapNode> CreateAsync(SitemapNode sitemapNode)
        {
            _sitemapNodes.Add(sitemapNode);

            return Task.FromResult(sitemapNode);
        }

        public Task<SitemapNode> GetAsync(Guid id)
        {
            var sitemapNode = _sitemapNodes
                .FirstOrDefault(x => x.Id == id);
            if(sitemapNode == null)
                throw new InvalidOperationException($"Node: {id} not found");

            return Task.FromResult(sitemapNode);
        }

        public Task<IEnumerable<SitemapNode>> GetListAsync(SitemapNodeQuery sitemapNodeQuery)
        {
            var query = _sitemapNodes.AsQueryable();

            if (!string.IsNullOrEmpty(sitemapNodeQuery.Name))
            {
                query = query.Where(x => x.Name.Contains(sitemapNodeQuery.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (sitemapNodeQuery.ParentId.HasValue)
            {
                query = query.Where(x => x.ParentId == sitemapNodeQuery.ParentId);
            }

            if (!string.IsNullOrEmpty(sitemapNodeQuery.Path))
            {
                query = query.Where(x => x.Path.Equals(sitemapNodeQuery.Path, StringComparison.OrdinalIgnoreCase));
            }

            if (sitemapNodeQuery.NodeType.HasValue)
            {
                query = query.Where(x => x.NodeType == sitemapNodeQuery.NodeType);
            }

            return Task.FromResult(query.AsEnumerable());
        }

        public Task RemoveAsync(Guid id)
        {
            var sitemapNode = _sitemapNodes
                .FirstOrDefault(x => x.Id == id);
            if (sitemapNode == null)
                throw new InvalidOperationException($"Node: {id} not found");

            _sitemapNodes.Remove(sitemapNode);

            return Task.CompletedTask;
        }

        public Task<SitemapNode> UpdateAsync(Guid id, SitemapNode sitemapNode)
        {
            return Task.FromResult(sitemapNode);
        }
    }
}
