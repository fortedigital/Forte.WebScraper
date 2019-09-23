using System;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace WebScraper.Models
{
    public class CrawlResult
    {
        public Uri RequestUrl { get; set; }
        
        public IHtmlDocument Document { get; set; }

        public CrawlResult(Uri requestUrl, IHtmlDocument document)
        {
            this.Document = document;
            this.RequestUrl = requestUrl;
        }

        public Element Css(string selector)
        {
            return new Element(this.Document.QuerySelector(selector));
        }

        public Element XPath(string selector)
        {
            return new Element(this.Document.QuerySelector("*[xpath>'" + selector + "']"));
        }

        public string Language => this.Document.QuerySelector("html")?.GetAttribute("lang");

        public class Element
        {
            private readonly IElement innerElement;

            public string InnerText => this.innerElement?.TextContent ?? "";
            public string Language => this.innerElement?.GetAttribute("lang") ?? "";
            public Element(IElement innerElement)
            {
                this.innerElement = innerElement;
            }
        }
    }
}