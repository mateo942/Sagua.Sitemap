using System;
using System.Collections.Generic;
using System.Text;

namespace Sagua.Sitemap.Events
{
    public class ChangeActiveNodeEventArgs : EventArgs
    {
        public bool FoundActiveNode { get; set; }
        public Dto.MenuNodeDto ActiveNode { get; set; }
    }
}
