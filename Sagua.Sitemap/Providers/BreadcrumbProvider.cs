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
    public class BreadcrumbProvider : IBreadcrumbProvider
    {
        private const string BREADCRUMB_PROVIDER_CACHE = "BreadcrumbProvider_{0}";
        protected string GetCacheNameForPath(string path)
            => string.Format(BREADCRUMB_PROVIDER_CACHE, path);

        protected readonly ISitemapNodeRepository _sitemapNodeRepository;
        protected readonly IMatchSitemapNode _matchSitemapNode;
        protected readonly BreadcrumbOptions _breadcrumbOptions;
        protected readonly IMemoryCache _memoryCache;
        protected readonly ILogger<BreadcrumbProvider> _logger;

        protected BreadcrumbDto CurrentBreadcrumb = BreadcrumbDto.Default;

        public BreadcrumbProvider(ISitemapNodeRepository sitemapNodeRepository, IMatchSitemapNode matchSitemapNode,
            IOptions<BreadcrumbOptions> breadcrumbOptions, IMemoryCache memoryCache, ILogger<BreadcrumbProvider> logger)
        {
            _sitemapNodeRepository = sitemapNodeRepository;
            _matchSitemapNode = matchSitemapNode;
            _breadcrumbOptions = breadcrumbOptions.Value;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<BreadcrumbDto> GetAsync()
        {
            return Task.FromResult(CurrentBreadcrumb);
        }

        public async Task<IEnumerable<BreadcrumbDto>> GetFlatAsync()
        {
            var list = new List<BreadcrumbDto>();

            var current = CurrentBreadcrumb;
            list.Add(new BreadcrumbDto
            {
                Id = current.Id,
                Name = current.Name,
                Parent = null,
                Path = current.Path
            });

            while(current.Parent != null)
            {
                current = current.Parent;

                var item = new BreadcrumbDto
                {
                    Id = current.Id,
                    Name = current.Name,
                    Parent = null,
                    Path = current.Path
                };

                list.Add(item);
            }

            list.Reverse();

            return await Task.FromResult(list);
        }

        public async Task SetActiveAsync(string path)
        {
            _logger.LogDebug("Try set breadcrumb");

            CurrentBreadcrumb = await _memoryCache.GetOrCreateAsync(GetCacheNameForPath(path), async entry =>
            {
                var currentNodes = await _matchSitemapNode.FindByPath(path);
                var firstCurrentNode = currentNodes.FirstOrDefault();
                if(firstCurrentNode != null)
                {
                    //Build breadcrumb
                    var breadcrumb = new BreadcrumbDto
                    {
                        Id = firstCurrentNode.Id,
                        Name = firstCurrentNode.Name,
                        Path = firstCurrentNode.Path, //TODO build path
                    };

                    var node = firstCurrentNode;
                    var currentBreadcrumb = breadcrumb;
                    while (node.ParentId.HasValue)
                    {
                        var parent = await _sitemapNodeRepository.GetAsync(node.ParentId.Value);

                        var parentBreadcrumb = new BreadcrumbDto
                        {
                            Id = parent.Id,
                            Name = parent.Name,
                            Path = parent.Path, //TODO build path
                        };

                        currentBreadcrumb.Parent = parentBreadcrumb;

                        currentBreadcrumb = parentBreadcrumb;
                        node = parent;
                    }

                    return breadcrumb;
                }

                return BreadcrumbDto.Default;
            });
        }
        
    }
}
