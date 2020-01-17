using System;
using System.Collections.Generic;
using System.Text;

namespace Sagua.Sitemap.Events
{
    public class ChangeBreadcrumbEventArgs : EventArgs
    {
        public Dto.BreadcrumbDto CurrentBreadcrumb { get; set; }
    }
}
