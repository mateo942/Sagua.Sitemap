using Sagua.Sitemap.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Providers
{
    public interface IMenuProvider
    {
        Task BuidAsync();
        Task<IEnumerable<MenuNodeDto>> GetAsync();
        Task SetActiveNode(string path);
    }
}
