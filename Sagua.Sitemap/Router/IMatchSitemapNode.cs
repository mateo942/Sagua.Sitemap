using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Router
{
    public interface IMatchSitemapNode
    {
        public Task<IEnumerable<Sitemap.Models.SitemapNode>> FindByUri(Uri uri);
        public Task<IEnumerable<Sitemap.Models.SitemapNode>> FindByName(string name);
        public Task<IEnumerable<Sitemap.Models.SitemapNode>> FindByPath(string path);
    }
}
