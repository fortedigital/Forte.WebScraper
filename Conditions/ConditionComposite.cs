using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace WebScraper.Conditions
{
    public class ConditionComposite : ICondition
    {
        private readonly IEnumerable<ICondition> conditions;
        
        public ConditionComposite(string[] testConditions)
        {
            conditions = testConditions.Select(ConditionFactory.GetCondition);
        }


        public bool Evaluate(IHtmlDocument document)
        {
            return conditions.All(c => c.Evaluate(document));
        }
    }
}