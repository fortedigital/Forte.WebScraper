using AngleSharp.Dom;

namespace WebScraper.PropertyExtractors
{
    public interface IPropertyExtractor
    {
        object ExtractProperties(IHtmlCollection<IElement> elements);
    }
}