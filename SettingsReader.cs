using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebScraper.Conditions;
using WebScraper.Models;

namespace WebScraper
{
    public class SettingsReader
    {
        public List<PageObject> ReadSettings()
        {
            var jsonFile =
                File.ReadAllText("C:\\Users\\Admin\\Desktop\\smokeTesterConditionalTestsExtensionExampleJSON.json");
            
            var jObject = JsonConvert.DeserializeObject<JObject>(jsonFile);
            return GetPageObjects(jObject);
        }

        private List<PageObject> GetPageObjects(JObject jsonObject)
        {
            var pageObjects = new List<PageObject>();
            foreach (var property in jsonObject.Properties())
            {
                var pageObj = new PageObject(property.Name);
                var childProperties = property.Value.Children<JProperty>().ToList();
                GetPageSettings(childProperties, pageObj);
                pageObjects.Add(pageObj);
            }

            return pageObjects;
        }

        private void GetPageSettings(List<JProperty> properties, PageObject pageObject)
        {
            properties.ForEach(p =>
            {
                switch (p.Name)
                {
                    case "test":
                        ExtractTestConditions(pageObject, (JArray) p.Value);
                        break;
                    /*case "linkPattern":
                        ExtractLinkPattern(pageObject, p.Value);
                        break;*/
                    case "pageLinks":
                        ExtractPageLinks(pageObject, p.Value.Children<JProperty>().ToList());
                        break;
                    case "properties":
                        ExtractProperties(pageObject, p.Value.Children<JProperty>().ToList());
                        break;
                }
            });
        }

        private static void ExtractPageLinks(PageObject pageObject, List<JProperty> properties)
        {
            properties.ForEach(prop =>
            {
                pageObject.PageLinks.Add(prop.Name, prop.Value.Value<string>());
            });
        }
        
        private static void ExtractProperties(PageObject pageObject, List<JProperty> properties)
        {
            properties.ForEach(prop =>
            {
                pageObject.Properties.Add(prop.Name, prop.Value.Value<string>());
            });
        }

        private static void ExtractLinkPattern(PageObject pageObject, JToken pattern)
        {
            //pageObject.LinkPattern = pattern.Value<string>();
        }
        
        private static void ExtractTestConditions(PageObject pageObject, JArray conditions)
        {
            pageObject.TestConditions = new ConditionComposite(conditions.Select(t => t.ToString()).ToArray());
        }
    }
}