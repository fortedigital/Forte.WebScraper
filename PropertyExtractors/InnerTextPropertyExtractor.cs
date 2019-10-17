using System;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using WebScraper.Models;

namespace WebScraper.PropertyExtractors
{
    public class InnerTextPropertyExtractor : IPropertyExtractor
    {
        public object ExtractProperties(CrawlResult crawlResult, IHtmlCollection<IElement> elements)
        {
            return string.Join(" ", elements.Select(e => {
                if (!string.IsNullOrWhiteSpace(e.TextContent))
                {
                    return e.TextContent;
                }
                else if(e is IHtmlMetaElement casted)
                {
                    return casted.Content;
                }
                return string.Empty;
            }));
        }
    }
}