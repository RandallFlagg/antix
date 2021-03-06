﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Antix.Html
{
    public class HtmlParser
    {
        readonly Func<string, IHtmlReader> _createReader;

        public HtmlParser(Func<string, IHtmlReader> createReader)
        {
            _createReader = createReader;
        }

        public static HtmlParser Create()
        {
            return new HtmlParser(s => new HtmlReader(s));
        }

        public IEnumerable<IHtmlNode> Parse(string html)
        {
            return ParseElements(_createReader(html));
        }

        public static HtmlAttribute ParseAttribute(IHtmlReader html)
        {
            var name = ParseAttributeName(html);
            if (string.IsNullOrWhiteSpace(name)) return null;

            return new HtmlAttribute
            {
                Name = name,
                Value = ParseAttributeValue(html)
            };
        }

        static string ParseAttributeName(IHtmlReader html)
        {
            var word = new List<char>();
            var hasWhitespace = false;

            while (html.Any())
            {
                var c = html.Peek();
                if (char.IsWhiteSpace(c))
                {
                    hasWhitespace = word.Any();
                    html.Consume();
                    continue;
                }
                if (!char.IsLetterOrDigit(c)
                    || hasWhitespace) break;

                html.Consume();
                word.Add(c);
            }

            return string.Join("", word);
        }

        static string ParseAttributeValue(IHtmlReader html)
        {
            if (html.Peek() != '=') return null;

            html.Consume();

            var word = new List<char>();
            var quote = (char) 0;
            var switched = false;

            while (html.Any())
            {
                var c = html.Peek();
                if (char.IsWhiteSpace(c)
                    || c == '>')
                {
                    if (quote == 0)
                    {
                        html.Consume();
                        if (word.Any()) break;

                        continue;
                    }
                }
                else if (c == '\'' || c == '"')
                {
                    if (!switched)
                    {
                        if (quote == c)
                        {
                            html.Consume();
                            break;
                        }

                        if (quote == 0)
                        {
                            quote = c;
                            html.Consume();
                            continue;
                        }
                    }
                }
                else if (c == '\\' && !switched)
                {
                    switched = true;
                    html.Consume();
                    continue;
                }
                else
                {
                    switched = false;
                }

                html.Consume();
                word.Add(c);
            }

            return string.Join("", word);
        }

        public static HtmlElement ParseElement(IHtmlReader html)
        {
            html.Consume(char.IsWhiteSpace);

            var name = ParseElementOpener(html);
            if (string.IsNullOrWhiteSpace(name)) return null;
            var element = new HtmlElement(name);

            if (element.IsDeclaration)
            {
                var closer = DeclarationCloser(name);

                var textElement = ParseTextElement(html, string.Concat(closer, ">"), true);
                element.Children.Add(textElement);
            }
            else
            {
                var attributes = ParseAttributes(html);
                element.Attributes.AddRange(attributes);

                html.Consume(char.IsWhiteSpace);

                if (html.TryConsume("/>", false, true))
                {
                    element.IsClosed = true;
                    return element;
                }

                html.TryConsume(">", false, true);
                if (element.IsTextOnlyContainer)
                {
                    var textElement = ParseTextElement(
                        html, string.Concat("</", name, ">"), true);
                    if (textElement != null) element.Children.Add(textElement);
                }
                else if (!element.IsNonContainer)
                {
                    var children = ParseElements(html);
                    element.Children.AddRange(children);

                    TryConsumeElementCloser(name, html);
                }
            }

            return element;
        }

        static IEnumerable<IHtmlNode> ParseElements(IHtmlReader html)
        {
            var items = new List<IHtmlNode>();

            IHtmlNode item;
            while ((item = ParseElement(html)
                           ?? ParseTextElement(html, "<", false)) != null)
            {
                items.Add(item);
            }

            return items;
        }

        static IHtmlNode ParseTextElement(
            IHtmlReader html,
            string upto, bool consumeTarget)
        {
            string text;

            if (!html.TryConsume(upto, true, consumeTarget, out text)
                || text == string.Empty) return null;

            return new HtmlTextElement {Value = CollapseWhiteSpace(text)};
        }

        static string CollapseWhiteSpace(string text)
        {
            if (text == null
                || text.Length < 2) return text;

            return string.Concat(
                char.IsWhiteSpace(text.First()) ? " " : "",
                text.Trim(),
                char.IsWhiteSpace(text.Last()) ? " " : ""
                );
        }

        static IEnumerable<HtmlAttribute> ParseAttributes(IHtmlReader html)
        {
            var items = new List<HtmlAttribute>();

            HtmlAttribute item;
            while ((item = ParseAttribute(html)) != null)
            {
                items.Add(item);
            }

            return items;
        }

        static string ParseElementOpener(IHtmlReader html)
        {
            if (html.Peek() != '<') return null;

            if (string.Join("", html.Peek(2)) == "</") return null;
            html.Consume();

            var word = new List<char>();
            while (html.Any())
            {
                var c = html.Peek();
                if (!char.IsLetterOrDigit(c)
                    && c != '!'
                    && c != '-') break;

                html.Consume();
                word.Add(c);
            }

            return string.Join("", word);
        }

        static void TryConsumeElementCloser(string name, IHtmlReader html)
        {
            var closer = string.Format("</{0}>", name);

            html.TryConsume(closer, false, true);
        }

        static readonly string[] InlineElements;
        static readonly string[] NonContainers;
        static readonly string[][] Declarations;
        static readonly string[] TextOnlyContainers;

        static HtmlParser()
        {
            InlineElements = HtmlSettings.Default.HtmlInlineElements.Split(',');
            NonContainers = HtmlSettings.Default.HtmlNonContainers.Split(',');
            Declarations = HtmlSettings.Default.HtmlDeclarations.Split(',').Select(i => i.Split('|')).ToArray();
            TextOnlyContainers = HtmlSettings.Default.HtmlTextOnlyContainers.Split(',');
        }

        public static bool IsDeclaration(string name)
        {
            return Declarations.Any(d => d[0] == name);
        }

        public static string DeclarationCloser(string name)
        {
            return (from d in Declarations
                where d.Length > 1 && d[0] == name
                select d[1]).SingleOrDefault();
        }

        public static bool IsNonContainer(string name)
        {
            return NonContainers.Contains(name)
                   || IsDeclaration(name);
        }

        public static bool IsInline(string name)
        {
            return InlineElements.Contains(name);
        }

        public static bool IsTextOnlyContainer(string name)
        {
            return TextOnlyContainers.Contains(name);
        }
    }
}