using Sagua.Sitemap.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Repository
{
    public interface ISitemapNodeRepository
    {
        Task<IEnumerable<Models.SitemapNode>> GetListAsync(SitemapNodeQuery sitemapNodeQuery);
        Task<Models.SitemapNode> GetAsync(Guid id);
        Task<Models.SitemapNode> CreateAsync(Models.SitemapNode sitemapNode);
        Task<Models.SitemapNode> UpdateAsync(Guid id, Models.SitemapNode sitemapNode);
        Task RemoveAsync(Guid id);
    }
}
