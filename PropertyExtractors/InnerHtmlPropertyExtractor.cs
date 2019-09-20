using System.Linq;
using AngleSharp.Dom;

namespace WebScraper.PropertyExtractors
{
    public class InnerHtmlPropertyExtractor : IPropertyExtractor
    {
        public object ExtractProperties(IHtmlCollection<IElement> elements)
        {
            return elements.Select(e => e.InnerHtml).Aggregate((prod, next) => prod + ";" + next);
        }
    }
}