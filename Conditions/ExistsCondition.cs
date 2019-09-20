using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace WebScraper.Conditions
{
    public class ExistsCondition : ICondition
    {
        public string ConditionValue { get; set; }
        
        public ExistsCondition(string value)
        {
            ConditionValue = value;
        }
        
        public bool Evaluate(IHtmlDocument document)
        {
            return document.QuerySelector(ConditionValue) != null;
        }
    }
}