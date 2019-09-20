using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using WebScraper.Models;

namespace WebScraper.Conditions
{
    public class ConditionComposite : ICondition
    {
        private readonly IEnumerable<ICondition> conditions;
        
        public ConditionComposite(string[] testConditions)
        {
            conditions = testConditions.Select(ConditionFactory.GetCondition).Where(c => c != null);
        }

        public bool Evaluate(CrawlResult result)
        {
            return conditions.All(c => c.Evaluate(result));
        }
    }
}