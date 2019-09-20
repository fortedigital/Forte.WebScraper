using System.Linq;
using AngleSharp.Html.Dom;

namespace WebScraper.Conditions
{
    public class ValueEqualCondition : ICondition
    {
        public string ConditionValue { get; set; }

        public ValueEqualCondition(string value)
        {
            ConditionValue = value;
        }
        
        public bool Evaluate(IHtmlDocument document)
        {
            var parts = ConditionValue.Split("==");
            parts = parts.Select(p => p.Trim()).ToArray();
            return document.QuerySelector(parts[0]).InnerHtml == parts[1];
        }
    }
}