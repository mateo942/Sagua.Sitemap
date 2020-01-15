using Microsoft.Extensions.Logging;
using Sagua.Sitemap.Repository;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Blazor
{
    public class SitemapNodeConfiguration
    {
        public string Icon { get; set; }

        public void ApplayConfiguration(Models.SitemapNode node)
        {
            if (!string.IsNullOrEmpty(Icon))
            {
                node.SetIcon(Icon);
            }
        }
    }

    public class BuildMenuSitemapNode
    {
        private readonly ISitemapNodeRepository _sitemapNodeRepository;
        private readonly ILogger<BuildMenuSitemapNode> _logger;

        public BuildMenuSitemapNode(ISitemapNodeRepository sitemapNodeRepository, ILogger<BuildMenuSitemapNode> logger)
        {
            _sitemapNodeRepository = sitemapNodeRepository;
            _logger = logger;
        }

        public async Task<Models.SitemapNode> AddAsync(string name, string path, Guid parentId, Action<SitemapNodeConfiguration> configurtation = null)
        {
            var node = new Models.SitemapNode(Guid.NewGuid(), name, path, parentId, Models.NodeType.Menu);

            var cfg = new SitemapNodeConfiguration();
            configurtation?.Invoke(cfg);
            cfg.ApplayConfiguration(node);

            node = await _sitemapNodeRepository.CreateAsync(node);
            return node;
        }

        public async Task<Models.SitemapNode> AddAsync(string name, string path, Action<SitemapNodeConfiguration> configurtation = null)
        {
            var node = new Models.SitemapNode(Guid.NewGuid(), name, path, Models.NodeType.Menu);

            var cfg = new SitemapNodeConfiguration();
            configurtation?.Invoke(cfg);
            cfg.ApplayConfiguration(node);

            node = await _sitemapNodeRepository.CreateAsync(node);
            return node;
        }

        public async Task<Models.SitemapNode> AddAsync<TPage>(string name, Action<SitemapNodeConfiguration> configurtation = null) where TPage : Microsoft.AspNetCore.Components.ComponentBase
        {
            var pageType = typeof(TPage);
            var routeAttribute = pageType.GetCustomAttribute<Microsoft.AspNetCore.Components.RouteAttribute>();
            if (routeAttribute == null)
                throw new InvalidOperationException($"Page: {pageType} has not included route attibute");

            return await AddAsync(name, routeAttribute.Template, configurtation);
        }

        public async Task<Models.SitemapNode> AddAsync<TPage>(string name, Guid parentId, Action<SitemapNodeConfiguration> configurtation = null) where TPage : Microsoft.AspNetCore.Components.ComponentBase
        {
            var pageType = typeof(TPage);
            var routeAttribute = pageType.GetCustomAttribute<Microsoft.AspNetCore.Components.RouteAttribute>();
            if (routeAttribute == null)
                throw new InvalidOperationException($"Page: {pageType} has not included route attibute");

            return await AddAsync(name, routeAttribute.Template, parentId, configurtation);
        }

    }
}
