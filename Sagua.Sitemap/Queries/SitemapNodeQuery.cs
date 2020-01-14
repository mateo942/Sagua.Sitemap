using Sagua.Sitemap.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sagua.Sitemap.Queries
{
    public class SitemapNodeQuery
    {
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public string Path { get; set; }
        public NodeType? NodeType { get; set; }
    }
}
