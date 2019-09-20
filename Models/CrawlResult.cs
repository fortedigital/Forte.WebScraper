using System;
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
        
    }
}