using Sagua.Sitemap.Dto;
using Sagua.Sitemap.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sagua.Sitemap.Providers
{
    public interface IBreadcrumbProvider
    {
        Task SetActiveAsync(string path);
        Task<BreadcrumbDto> GetAsync();
        Task<IEnumerable<BreadcrumbDto>> GetFlatAsync();

        event EventHandler<ChangeBreadcrumbEventArgs> ChangeActive;
    }
}
