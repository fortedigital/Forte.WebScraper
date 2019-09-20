using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace WebScraper.Conditions
{
    public class ExistsCondition : ICondition
    {
        private readonly string selector;
        private readonly bool isNegated;
        
        public ExistsCondition(string value)
        {
            isNegated = value.StartsWith("notexists");
            selector = isNegated 
                ? value.Replace("notexists:", "") 
                : value.Replace("exists:", "");
        }
        
        public bool Evaluate(IHtmlDocument document)
        {
            var element = document.QuerySelector(selector);
            return isNegated ? element == null : element != null;
        }
    }
}