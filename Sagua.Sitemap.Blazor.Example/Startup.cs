using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sagua.Sitemap.Blazor.Example.Pages;
using Sagua.Sitemap.Blazor.Provider;
using Sagua.Sitemap.Options;
using Sagua.Sitemap.Providers;
using Sagua.Sitemap.Repository;
using Sagua.Sitemap.Router;
using System;
using System.Linq;

namespace Sagua.Sitemap.Blazor.Example
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddLogging(cfg => {
                cfg.AddConsole();
                cfg.SetMinimumLevel(LogLevel.Trace);
            });

            services.AddScoped<ISitemapNodeRepository, SitemapNodeInMemoryRepository>();
            services.AddScoped<IMatchSitemapNode, DumyMatchSitemapNode>();
            services.AddScoped<IMenuProvider, BlazorMenuProvider>();
            services.AddScoped<IBreadcrumbProvider, BlazorBreadcrumbProvider>();
            services.AddOptions<MatchOptions>().Configure(x =>
            {
                x.BasePath = "https://localhost:44388";
            });
            services.AddOptions<MenuOptions>().Configure(x =>
            {
                x.MaxDeep = 5;
                x.CacheExpiration = TimeSpan.FromSeconds(10);
                x.UseIndexSitemap = true;
            });
            services.AddOptions<BreadcrumbOptions>();

            services.AddScoped<BuildMenuSitemapNode>();
        }

        public async void Configure(IComponentsApplicationBuilder app, BuildMenuSitemapNode buildMenuSitemapNode, IMenuProvider menuProvider)
        {
            app.AddComponent<App>("app");

            var games = await buildMenuSitemapNode.AddAsync("Games", "/games", cfg =>
            {
                cfg.Icon = "oi oi-puzzle-piece";
            });
            await buildMenuSitemapNode.AddAsync("Witcher 1", "/games/witcher-1", games.Id);
            await buildMenuSitemapNode.AddAsync("Witcher 2", "/games/witcher-2", games.Id);
            await buildMenuSitemapNode.AddAsync("Witcher 3", "/games/witcher-3", games.Id);

            await buildMenuSitemapNode.AddAsync<Counter>("Couter", cfg =>
            {
                cfg.Icon = "oi oi-plus";
            });
            await buildMenuSitemapNode.AddAsync<FetchData>("Example data", cfg =>
            {
                cfg.Icon = "oi oi-document";
            });

            await menuProvider.BuidAsync();
        }
    }
}
