using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sagua.Sitemap.Options;
using Sagua.Sitemap.Providers;
using Sagua.Sitemap.Repository;
using Sagua.Sitemap.Router;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Example
{
    class Program
    {
        static IServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddLogging(cfg =>
            {
                cfg.AddConsole();
                cfg.SetMinimumLevel(LogLevel.Debug);
            });
            serviceCollection.AddScoped<ISitemapNodeRepository, SitemapNodeInMemoryRepository>();
            serviceCollection.AddScoped<IMatchSitemapNode, DumyMatchSitemapNode>();
            serviceCollection.AddScoped<IMenuProvider, MenuProvider>();
            serviceCollection.AddOptions<MatchOptions>().Configure(x =>
            {
                x.BasePath = "https://sagua.com";
            });
            serviceCollection.AddOptions<MenuOptions>().Configure(x =>
            {
                x.MaxDeep = 5;
                x.CacheExpiration = TimeSpan.FromSeconds(10);
                x.UseIndexSitemap = true;
            });


            serviceProvider = serviceCollection.BuildServiceProvider();

            FillSitemapNodes().GetAwaiter().GetResult();
            GetMenu().GetAwaiter().GetResult();

            Console.ReadKey(true);
        }

        static async Task FillSitemapNodes()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var sitemapNodeRepository = scope.ServiceProvider.GetRequiredService<ISitemapNodeRepository>();

                await sitemapNodeRepository.CreateAsync(new Models.SitemapNode(Guid.NewGuid(), "Home", "/", Models.NodeType.Menu));
                await sitemapNodeRepository.CreateAsync(new Models.SitemapNode(Guid.NewGuid(), "About", "/about", Models.NodeType.Menu));
                await sitemapNodeRepository.CreateAsync(new Models.SitemapNode(Guid.NewGuid(), "Contact", "/contact", Models.NodeType.Menu));
                var games = await sitemapNodeRepository.CreateAsync(new Models.SitemapNode(Guid.NewGuid(), "Games", "/games", Models.NodeType.Menu));
                await sitemapNodeRepository.CreateAsync(new Models.SitemapNode(Guid.NewGuid(), "Witcher 1", "/games/witcher-1", games.Id, Models.NodeType.Menu));
                await sitemapNodeRepository.CreateAsync(new Models.SitemapNode(Guid.NewGuid(), "Witcher 2", "/games/witcher-2", games.Id, Models.NodeType.Menu));
                await sitemapNodeRepository.CreateAsync(new Models.SitemapNode(Guid.NewGuid(), "Witcher 3", "/games/witcher-3", games.Id, Models.NodeType.Menu));
            }
        }

        static async Task GetMenu()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var menuProvider = scope.ServiceProvider.GetRequiredService<IMenuProvider>();
                await menuProvider.BuidAsync();

                await menuProvider.SetActiveNode("/about");
                var nodes = await menuProvider.GetAsync();
                var activeNode = nodes.FirstOrDefault(x => x.IsActive);

                await menuProvider.SetActiveNode("/games/witcher-3");
                var nodes1 = await menuProvider.GetAsync();
                var activeNode1 = nodes1.FirstOrDefault(x => x.IsActive);

                await menuProvider.SetActiveNode("/games/witcher-4");
                var nodes2 = await menuProvider.GetAsync();
                var activeNode2 = nodes1.FirstOrDefault(x => x.IsActive);
            }


        }
    }
}
