using System.Collections.Generic;
using WebScraper.Conditions;

namespace WebScraper.Models
{
    public class PageObject
    {
        public string PageName { get; set; }

        public Dictionary<string, string> PageLinks { get; set; }

        public Dictionary<string, PagePropertyObject> Properties { get; set; }

        public ConditionComposite TestConditions { get; set; }
        
        public PageObject(string name)
        {
            PageName = name;
            PageLinks = new Dictionary<string, string>();
            Properties = new Dictionary<string, PagePropertyObject>();
        }
    }
}