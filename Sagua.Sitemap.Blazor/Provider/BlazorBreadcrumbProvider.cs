using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sagua.Sitemap.Options;
using Sagua.Sitemap.Providers;
using Sagua.Sitemap.Repository;
using Sagua.Sitemap.Router;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sagua.Sitemap.Blazor.Provider
{
    public class BlazorBreadcrumbProvider : BreadcrumbProvider, IDisposable
    {
        private readonly NavigationManager _navigationManager;

        public BlazorBreadcrumbProvider(ISitemapNodeRepository sitemapNodeRepository, IMatchSitemapNode matchSitemapNode,
            IOptions<BreadcrumbOptions> breadcrumbOptions, IMemoryCache memoryCache, ILogger<BreadcrumbProvider> logger,
            NavigationManager navigationManager) : base(sitemapNodeRepository, matchSitemapNode, breadcrumbOptions, memoryCache, logger)
        {
            _navigationManager = navigationManager;
            _navigationManager.LocationChanged += _navigationManager_LocationChanged;
            this.SetActiveAsync(_navigationManager.Uri).GetAwaiter().GetResult();
        }

        private async void _navigationManager_LocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            _logger.LogDebug($"Change location: {e.Location}");

            await this.SetActiveAsync(e.Location);
        }

        public void Dispose()
        {
            _navigationManager.LocationChanged -= _navigationManager_LocationChanged;
        }
    }
}
