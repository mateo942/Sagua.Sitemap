using Microsoft.AspNetCore.Components;
using Sagua.Sitemap.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Blazor.Components
{
    public class BreadcrumbBase : ComponentBase, IDisposable
    {
        [Inject]
        protected IBreadcrumbProvider _breadcrumbProvider { get; set; }

        protected IEnumerable<Dto.BreadcrumbDto> FlatBreadcrumb { get; private set; }

        protected async override Task OnInitializedAsync()
        {
            _breadcrumbProvider.ChangeActive += _breadcrumbProvider_ChangeActive;

            FlatBreadcrumb = await _breadcrumbProvider.GetFlatAsync();
            this.StateHasChanged();
        }

        private async void _breadcrumbProvider_ChangeActive(object sender, Events.ChangeBreadcrumbEventArgs e)
        {
            FlatBreadcrumb = await _breadcrumbProvider.GetFlatAsync();
            this.StateHasChanged();
        }

        public void Dispose()
        {
            _breadcrumbProvider.ChangeActive -= _breadcrumbProvider_ChangeActive;
        }
    }
}
