using System;
using System.Collections.Generic;
using System.Text;

namespace Sagua.Sitemap.Options
{
    public class MatchOptions
    {
        public string BasePath { get; set; }
        public TimeSpan SlidingExpiration = TimeSpan.FromMinutes(5);
    }
}
