using Microsoft.AspNetCore.Components;
using Sagua.Sitemap.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Blazor.Components
{
    public class SaguaMenuBase : ComponentBase
    {
        [Inject]
        protected IMenuProvider _menuProvider { get; set; }

        public IEnumerable<Dto.MenuNodeDto> MenuNodes = Enumerable.Empty<Dto.MenuNodeDto>();

        protected async override Task OnInitializedAsync()
        {
            MenuNodes = await _menuProvider.GetAsync();
            this.StateHasChanged();
        }
    }
}
