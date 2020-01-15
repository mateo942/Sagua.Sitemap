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
    public class BlazorMenuProvider : MenuProvider, IDisposable
    {
        protected readonly NavigationManager _navigationManager;

        public BlazorMenuProvider(ISitemapNodeRepository sitemapNodeRepository, IMatchSitemapNode matchSitemapNode,
            IOptions<MenuOptions> menuOptions, IMemoryCache memoryCache, ILogger<BlazorMenuProvider> logger, NavigationManager navigationManager) 
            : base(sitemapNodeRepository, matchSitemapNode, menuOptions, memoryCache, logger)
        {
            _navigationManager = navigationManager;

            _navigationManager.LocationChanged += _navigationManager_LocationChanged;
        }

        private async void _navigationManager_LocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            _logger.LogDebug($"Change location: {e.Location}");

            await this.SetActiveNode(e.Location);
        }

        public void Dispose()
        {
            _navigationManager.LocationChanged -= _navigationManager_LocationChanged;
        }
    }
}
