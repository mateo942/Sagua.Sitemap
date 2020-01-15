using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sagua.Sitemap.Blazor.Components
{
    public class SaguaNavLinkBase : ComponentBase
    {
        [Parameter]
        public Sagua.Sitemap.Dto.MenuNodeDto MenuNode { get; set; }

        protected bool HasChildren
            => MenuNode.Children != null && MenuNode.Children.Any();

        protected bool IsOpen { get; set; }

        protected void OpenCloseClick()
        {
            IsOpen = !IsOpen;
        }
    }
}
