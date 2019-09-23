using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Flee.PublicTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebScraper.Conditions;
using WebScraper.Models;

namespace WebScraper
{
    public class SettingsReader
    {
        public List<PageObject> ReadSettings(string path)
        {
            var jsonFile = File.ReadAllText(path);
            
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
                        ExtractTestConditions(pageObject, p.Value);
                        break;
                    case "languages":
                        ExtractLanguages(pageObject, p.Value.Children<JProperty>().ToList());
                        break;
                    case "pageLinks":
                        ExtractPageLinks(pageObject, p.Value.Children<JProperty>().ToList());
                        break;
                    case "properties":
                        ExtractProperties(pageObject, p.Value.Children<JProperty>().ToList());
                        break;
                }
            });
        }

        private static void ExtractLanguages(PageObject pageObject, List<JProperty> languages)
        {
            languages.ForEach(lang =>
            {
                pageObject.Languages.Add(lang.Name, lang.Value.Value<string>());
            });
        }
        
        private static void ExtractPageLinks(PageObject pageObject, List<JProperty> pageLinks)
        {
            pageLinks.ForEach(prop =>
            {
                pageObject.PageLinks.Add(prop.Name, prop.Value.Value<string>());
            });
        }
        
        private static void ExtractProperties(PageObject pageObject, List<JProperty> properties)
        {
            properties.ForEach(prop =>
            {
                var pagePropObj = new PagePropertyObject(prop.Value.Value<string>());
                pageObject.Properties.Add(prop.Name, pagePropObj);
            });
        }

        private static void ExtractLinkPattern(PageObject pageObject, JToken pattern)
        {
            //pageObject.LinkPattern = pattern.Value<string>();
        }
        
        private static void ExtractTestConditions(PageObject pageObject, JToken conditions)
        {
            var expressionContext = new ExpressionContext();
            expressionContext.Variables["doc"] = new CrawlResult(null, null);
            var expression = expressionContext.CompileGeneric<bool>(conditions.Value<string>());

            bool Condition(CrawlResult result)
            {
                lock (expressionContext)
                {
                    expressionContext.Variables["doc"] = result;
                    return expression.Evaluate();
                }
            }

            pageObject.TestCondition = Condition;
        }
    }
}