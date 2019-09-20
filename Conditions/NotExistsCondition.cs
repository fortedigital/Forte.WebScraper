using AngleSharp.Html.Dom;

namespace WebScraper.Conditions
{
    public class NotExistsCondition : ICondition
    {
        public string ConditionValue { get; set; }
        
        public NotExistsCondition(string value)
        {
            ConditionValue = value;
        }
        
        public bool Evaluate(IHtmlDocument document)
        {
            return document.QuerySelector(ConditionValue) == null;
        }
    }
}