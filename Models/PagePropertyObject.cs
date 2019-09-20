using AngleSharp.Dom;
using WebScraper.PropertyExtractors;

namespace WebScraper.Models
{
    public class PagePropertyObject
    {
        public string Selector { get; set; }

        public IPropertyExtractor Extractor { get; set; }

        public PagePropertyObject(string value)
        {
            Extractor = PropertyExtractorFactory.GetPropertyExtractor(value, out var selector);
            Selector = selector;
        }

        public object ExtractProperties(IHtmlCollection<IElement> elements)
        {
            return Extractor.ExtractProperties(elements);
        }

    }
}