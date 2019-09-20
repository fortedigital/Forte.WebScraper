using AngleSharp.Html.Dom;

namespace WebScraper.Conditions
{
    public class CssSelectorCondition : ICondition
    {
        public string ConditionValue { get; set; }

        public CssSelectorCondition(string value)
        {
            ConditionValue = value;
        }
        
        public bool Evaluate(IHtmlDocument document)
        {
            return document.QuerySelector(ConditionValue) != null;
        }
        
    }
}