using Sagua.Sitemap.Dto;
using Sagua.Sitemap.Events;
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

        event EventHandler<ChangeActiveNodeEventArgs> ChangeActiveNode;
    }
}
