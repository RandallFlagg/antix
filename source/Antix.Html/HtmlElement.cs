﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Antix.Html
{
    public class HtmlElement : IHtmlNode
    {
        readonly List<HtmlAttribute> _attributes;
        readonly List<IHtmlNode> _children;
        string _name;

        internal HtmlElement(
            string name)
        {
            Name = name;
            IsClosed = IsNonContainer;

            _attributes = new List<HtmlAttribute>();
            _children = new List<IHtmlNode>();
        }

        public HtmlElement() : this(null)
        {
        }

        public string Name
        {
            get { return _name; }
            set
            {
                Debug.Assert(value != null, "value != null");
                _name = value.ToLower();

                IsNonCloser = HtmlParser.IsNonCloser(_name);
                IsNonContainer = HtmlParser.IsNonContainer(_name);
                IsInline = HtmlParser.IsInline(_name);
            }
        }

        public bool IsClosed { get; set; }

        public List<HtmlAttribute> Attributes
        {
            get { return _attributes; }
        }

        public List<IHtmlNode> Children
        {
            get { return _children; }
        }

        public bool IsNonCloser { get; private set; }

        public bool IsNonContainer { get; private set; }

        public bool IsInline { get; private set; }

        public void ToString(StringBuilder output)
        {
            output.Append("<");
            output.Append(Name);

            foreach (var item in Attributes)
            {
                output.Append(" ");
                item.ToString(output);
            }

            if (IsNonCloser)
            {
                output.Append(">");
                return;
            }

            if (IsNonContainer || IsClosed)
            {
                output.Append("/>");
                return;
            }

            output.Append(">");
            foreach (var item in Children)
            {
                item.ToString(output);
            }

            output.Append("</");
            output.Append(Name);
            output.Append(">");
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            ToString(output);

            return output.ToString();
        }
    }
}