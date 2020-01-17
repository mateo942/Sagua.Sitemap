using System;
using System.Collections.Generic;
using System.Text;

namespace Sagua.Sitemap.Dto
{
    public class BreadcrumbDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public BreadcrumbDto Parent { get; set; }

        public static BreadcrumbDto Default
            => new BreadcrumbDto()
            {
                Name = "Not found"
            };
    }
}
