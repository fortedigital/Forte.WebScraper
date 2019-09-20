using AngleSharp.Html.Dom;

namespace WebScraper.Conditions
{
    public interface ICondition
    {
        string ConditionValue { get; set; }
        
        bool Evaluate(IHtmlDocument document);
        
    }
}