using System.Linq;
using AngleSharp.Dom;
using WebScraper.Models;

namespace WebScraper.PropertyExtractors
{
    public class InnerTextPropertyExtractor : IPropertyExtractor
    {
        public object ExtractProperties(CrawlResult crawlResult, IHtmlCollection<IElement> elements)
        {
            return string.Join(" ", elements.Select(e => e.TextContent));
        }
    }
}