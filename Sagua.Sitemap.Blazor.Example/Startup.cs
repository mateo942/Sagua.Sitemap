using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sagua.Sitemap.Blazor.Example.Pages;
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
            services.AddLogging();

            services.AddScoped<ISitemapNodeRepository, SitemapNodeInMemoryRepository>();
            services.AddScoped<IMatchSitemapNode, DumyMatchSitemapNode>();
            services.AddScoped<IMenuProvider, MenuProvider>();
            services.AddOptions<MatchOptions>().Configure(x =>
            {
                x.BasePath = "https://sagua.com";
            });
            services.AddOptions<MenuOptions>().Configure(x =>
            {
                x.MaxDeep = 5;
                x.CacheExpiration = TimeSpan.FromSeconds(10);
                x.UseIndexSitemap = true;
            });

            services.AddScoped<BuildMenuSitemapNode>();
        }

        public async void Configure(IComponentsApplicationBuilder app, BuildMenuSitemapNode buildMenuSitemapNode, IMenuProvider menuProvider)
        {
            app.AddComponent<App>("app");

            var games = await buildMenuSitemapNode.AddAsync("Games", "/games");
            await buildMenuSitemapNode.AddAsync("Witcher 1", "/games/witcher-1", games.Id);
            await buildMenuSitemapNode.AddAsync("Witcher 2", "/games/witcher-2", games.Id);
            await buildMenuSitemapNode.AddAsync("Witcher 3", "/games/witcher-3", games.Id);

            await buildMenuSitemapNode.AddAsync<Counter>("Couter");
            await buildMenuSitemapNode.AddAsync<FetchData>("Example data");

            await menuProvider.BuidAsync();

            var nodes = await menuProvider.GetAsync();
            Console.WriteLine(string.Join(", ", nodes.Select(x => x.ToString())));
        }
    }
}
