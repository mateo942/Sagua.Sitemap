using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sagua.Sitemap.Dto;
using Sagua.Sitemap.Options;
using Sagua.Sitemap.Repository;
using Sagua.Sitemap.Router;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Providers
{
    public class MenuProvider : IMenuProvider
    {
        private const string MENU_PROVIDER_CACHE_NAME = "MenuProvider_Sitemap";
        private const string MENU_PROVIDER_CACHE_INDEX_NAME = "MenuProvider_Sitemap_Index";

        private readonly ISitemapNodeRepository _sitemapNodeRepository;
        private readonly IMatchSitemapNode _matchSitemapNode;
        private readonly MenuOptions _menuOptions;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MenuProvider> _logger;

        public MenuProvider(ISitemapNodeRepository sitemapNodeRepository, IMatchSitemapNode matchSitemapNode, IOptions<MenuOptions> menuOptions,
            IMemoryCache memoryCache, ILogger<MenuProvider> logger)
        {
            _sitemapNodeRepository = sitemapNodeRepository;
            _matchSitemapNode = matchSitemapNode;
            _menuOptions = menuOptions.Value;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task BuidAsync()
        {
            _memoryCache.Remove(MENU_PROVIDER_CACHE_NAME);

            var nodes = await InternalBuildAsync();

            _memoryCache.Set(MENU_PROVIDER_CACHE_NAME, nodes);
        }

        internal async Task<IEnumerable<MenuNodeDto>> InternalBuildAsync()
        {
            _logger.LogDebug("The reconstruction of the sitemap was started...");

            var nodes = await _sitemapNodeRepository.GetListAsync(new Queries.SitemapNodeQuery
            {
                NodeType = Models.NodeType.Menu
            });

            _memoryCache.Remove(MENU_PROVIDER_CACHE_INDEX_NAME);

            var mapNodes = nodes.Select(x => new Dto.MenuNodeDto
            {
                Id = x.Id,
                ParentId = x.ParentId,
                Title = x.Name,
                Path = x.Path
            }).ToList();

            //Index all node
            var indexSitemap = new Dictionary<Guid, MenuNodeDto>();
            foreach (var node in mapNodes)
            {
                indexSitemap.Add(node.Id, node);
            }
            _memoryCache.Set(MENU_PROVIDER_CACHE_INDEX_NAME, indexSitemap);

            var rootNodes = mapNodes.Where(x => x.ParentId == null);
            foreach (var node in rootNodes)
            {
                SetChildren(node, mapNodes);
            }

            _logger.LogDebug("The reconstruction of the sitemap has been completed");

            return rootNodes;
        }

        internal void SetChildren(Dto.MenuNodeDto currentNode, IEnumerable<Dto.MenuNodeDto> allNodes)
        {
            var children = allNodes.Where(x => x.ParentId == currentNode.Id);
            if (children.Any() == false)
                return;

            currentNode.Children = children;
            foreach (var child in children)
            {
                child.Parent = currentNode;
                SetChildren(child, allNodes);
            }
        }


        public async Task<IEnumerable<MenuNodeDto>> GetAsync()
        {
            var nodes = await _memoryCache.GetOrCreateAsync(MENU_PROVIDER_CACHE_NAME, async cache =>
            {
                var tempNodes = await InternalBuildAsync();
                return tempNodes;
            });

            return nodes.ToList();
        }

        public async Task SetActiveNode(string path)
        {
            _logger.LogDebug("Try set active node");

            _memoryCache.TryGetValue(MENU_PROVIDER_CACHE_INDEX_NAME, out IDictionary<Guid, Dto.MenuNodeDto> indexSitemap);
            if (indexSitemap == null)
                return;

            foreach (var indexNode in indexSitemap)
            {
                indexNode.Value.SetActive(false);
            }

            var currentActiveSitemapNodes = await _matchSitemapNode.FindByPath(path);
            if (currentActiveSitemapNodes == null || currentActiveSitemapNodes.Any() == false)
                return;

            var currentFirstNode = currentActiveSitemapNodes.First();
            if (indexSitemap.ContainsKey(currentFirstNode.Id))
            {
                indexSitemap[currentFirstNode.Id].SetActive(true);
            } else if(currentFirstNode.ParentId.HasValue && indexSitemap.ContainsKey(currentFirstNode.ParentId.Value))
            {
                indexSitemap[currentFirstNode.ParentId.Value].SetActive(true);
            }
        }
    }
}
