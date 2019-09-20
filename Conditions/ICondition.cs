using AngleSharp.Html.Dom;

namespace WebScraper.Conditions
{
    public interface ICondition
    {
        bool Evaluate(IHtmlDocument document);
        
    }
}