using System.Linq;
using System.Text.RegularExpressions;
using WebScraper.Models;

namespace WebScraper.Conditions
{
    public class ValueCondition : ICondition
    {
        private readonly string selector;
        private readonly string valueToCompare;
        private readonly bool isNegated;
        
        public ValueCondition(string value)
        {
            value = value.Replace("value:","");
            var parts = Regex.Split(value, @"(==)|(!=)");
            parts = parts.Select(p => p.Trim()).ToArray();
            selector = parts[0];
            isNegated = parts[1] == "!=";
            valueToCompare = parts[2];
        }
        
        public bool Evaluate(CrawlResult result)
        {
            return isNegated ? result.Document.QuerySelector(this.selector)?.InnerHtml != valueToCompare 
                : result.Document.QuerySelector(this.selector)?.InnerHtml == valueToCompare;
        }
    }
}