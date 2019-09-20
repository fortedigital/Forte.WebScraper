using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WebScraper.Conditions;

namespace WebScraper
{
    public class PageSettings
    {
        public string LinkPattern { get; set; }
        
        public Dictionary<string, string> PageLinks { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public ConditionComposite TestConditions { get; set; }

        public PageSettings()
        {
            PageLinks = new Dictionary<string, string>();
            Properties = new Dictionary<string, string>();
        }
    }
}