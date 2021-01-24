using System;
using System.Collections.Generic;
using System.Text;

namespace Sagua.Sitemap.Models
{
    public class SitemapNode
    {
        public SitemapNode(Guid id, string name, string path, NodeType nodeType)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(nameof(id));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(nameof(path));
            }

            Id = id;
            Name = name;
            Path = path;
            NodeType = nodeType;
        }

        public SitemapNode(Guid id, string name, string path, Guid parentId, NodeType nodeType)
            : this(id, name, path, nodeType)
        {
            if (parentId == Guid.Empty)
            {
                throw new ArgumentException(nameof(parentId));
            }

            ParentId = parentId;
        }

        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public string Path { get; protected set; }
        public NodeType NodeType { get; protected set; }

        public Guid? ParentId { get; protected set; }

        public string Icon { get; protected set; }

        public SitemapNode SetIcon(string icon)
        {
            if (string.IsNullOrEmpty(icon))
            {
                throw new ArgumentException(nameof(icon));
            }

            Icon = icon;

            return this;
        }

        public SitemapNode SetNodeType(NodeType nodeType)
        {
            NodeType = nodeType;

            return this;
        }
    }
}
