using System;
using System.Collections.Generic;
using System.Text;

namespace Sagua.Sitemap.Dto
{
    public class MenuNodeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }

        public bool IsActive { get; set; }

        public Guid? ParentId { get; set; }
        public MenuNodeDto Parent { get; set; }
        public IEnumerable<MenuNodeDto> Children { get; set; }

        public void SetActive(bool value)
        {
            IsActive = value;

            if (value && Parent != null)
            {
                Parent.SetActive(value);
            }
        }

        public override string ToString()
        {
            return $"{Title} ({Path})";
        }
    }
}
