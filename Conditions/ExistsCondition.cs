using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using WebScraper.Models;

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
        
        public bool Evaluate(CrawlResult result)
        {
            var element = result.Document.QuerySelector(selector);
            return isNegated ? element == null : element != null;
        }
    }
}