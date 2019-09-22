using AngleSharp.Dom;
using WebScraper.Models;

namespace WebScraper.PropertyExtractors
{
    public interface IPropertyExtractor
    {
        object ExtractProperties(CrawlResult crawlResult, IHtmlCollection<IElement> elements);
    }
}