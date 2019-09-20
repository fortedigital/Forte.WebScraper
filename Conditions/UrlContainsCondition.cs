using AngleSharp.Html.Dom;
using WebScraper.Models;

namespace WebScraper.Conditions
{
    public class UrlContainsCondition : ICondition
    {
        private readonly string valueToContain;
        private readonly bool isNegated;
        
        public UrlContainsCondition(string value)
        {
            isNegated = value.StartsWith("urlnotcontains");
            valueToContain = isNegated
                ? value.Replace("urlnotcontains:", "")
                : value.Replace("urlcontains:", "");
        }
        
        public bool Evaluate(CrawlResult result)
        {
            var contains = result.RequestUrl.AbsoluteUri.Contains(valueToContain);
            return isNegated ? !contains : contains;
        }
    }
}