﻿using System;
using System.Linq;
using Antix.Html;
using Xunit;

namespace Antix.Tests.Html
{
    public class ParseElementTest
    {
        [Fact]
        public void basic_element()
        {
            Exec("<div>");
        }

        [Fact]
        public void illegal_name_ignored()
        {
            Exec("<div=a>");
        }

        [Fact]
        public void whitespace_in_name_ignored()
        {
            Exec("<div   >");
        }

        [Fact]
        public void with_attributes()
        {
            Exec("<div one='1' two='2'>", expectedAttributeCount: 2);
        }

        [Fact]
        public void with_attributes_no_quotes()
        {
            Exec("<div one=1 two=2>", expectedAttributeCount: 2);
        }

        [Fact]
        public void with_attributes_mixed_quotes()
        {
            Exec("<div one='1' two=\"2\">", expectedAttributeCount: 2);
        }

        [Fact]
        public void with_attributes_no_whitespace_between()
        {
            Exec("<div one='1'two='2'>", expectedAttributeCount: 2);
        }

        [Fact]
        public void with_attributes_unclosed_quote()
        {
            Exec("<div one='1 two='2'>", expectedAttributeCount: 2);
        }

        [Fact]
        public void with_children()
        {
            Exec("<div><br/></div>", expectedChildCount: 1);
        }

        [Fact]
        public void with_nested_children()
        {
            var result = Exec("<div><div><br/></div></div>", expectedChildCount: 1);

            Assert.Equal(1, result.Children.OfType<HtmlElement>().Single().Children.Count);
        }

        [Fact]
        public void missing_closer()
        {
            Exec("<div><br/><br/>", expectedChildCount: 2);
        }

        [Fact]
        public void non_containers_dont_nest()
        {
            Exec("<div><br><br></div>", expectedChildCount: 2);
        }

        [Fact]
        public void nested_text_element()
        {
            var result = Exec("<div>Hello</div>", expectedChildCount: 1);

            Assert.IsType<HtmlTextElement>(result.Children.Single());
        }

        [Fact]
        public void consecutive_nested_elements()
        {
            Exec("<div><p>hello</p><ul><li>one</li><li>two</li></div>", expectedChildCount: 2);
        }

        [Fact]
        public void doctype()
        {
            Exec("<!doctype html>", expectedName: "!doctype", expectedAttributeCount: 1);
        }

        static HtmlElement Exec(
            string html,
            string expectedName = "div",
            int expectedAttributeCount = 0,
            int expectedChildCount = 0)
        {
            var sut = new HtmlParser();

            var result = sut.ParseElement(new HtmlQueue(html));

            Assert.NotNull(result);
            Console.Write(result.ToString());

            Assert.Equal(expectedName, result.Name);
            Assert.Equal(expectedAttributeCount, result.Attributes.Count);
            Assert.Equal(expectedChildCount, result.Children.Count);

            return result;
        }
    }
}