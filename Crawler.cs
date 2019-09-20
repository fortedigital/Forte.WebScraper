using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using WebScraper.Models;

namespace WebScraper
{
    public class Crawler
    {
        private static readonly IEnumerable<string> ExludedSchemas = new[]
        {
            "mailto",
            "tel",
            "script"
        };
        
        private readonly List<PageObject> pageObjects;
        private readonly Queue<CrawlRequest> queue;
        private readonly List<TreeNode> parentNodes;

        public Crawler(Options opts, List<PageObject> pageObjects)
        {
            this.pageObjects = pageObjects;
            queue = new Queue<CrawlRequest>();
            parentNodes = new List<TreeNode>();
            foreach (var url in opts.StartUrls)
            {
                queue.Enqueue(new CrawlRequest(new Uri(url), null));
            }
        }

        public void Crawl()
        {
            while (queue.Count != 0)
            {
                var request = queue.Dequeue();
                var document = GetDocument(request.Url);
                foreach (var pageObj in pageObjects)
                {
                    if (pageObj.TestConditions == null)
                        continue;
                    if (pageObj.TestConditions != null && !pageObj.TestConditions.Evaluate(document))
                        continue;
                    
                    var node = new TreeNode(pageObj.PageName);
                    ExtractProperties(document, pageObj, node);
                    
                    ExtractLinksToQueue(document, pageObj, node, request.Url);

                    if (request.ParentNode == null)
                    {
                        parentNodes.Add(node);
                    }
                    else
                    {
                        var parent = parentNodes.Find(n => n.Equals(request.ParentNode));
                        parent?.ChildNodes.Add(node);
                    }
                }
            }

            SavePropertiesToFile("C:\\Users\\Admin\\Desktop\\returnedFile.json");
        }

        private void SavePropertiesToFile(string path)
        {
            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            var serializedJson = JsonConvert.SerializeObject(parentNodes, options);
            using (var writer = File.CreateText(path))
            {
                writer.Write(serializedJson);
                writer.Flush();
            }
        }
        
        private void ExtractLinksToQueue(IHtmlDocument document, PageObject pageObject, TreeNode parent, Uri baseUrl)
        {
            if (pageObject.PageLinks.Count == 0)
                return;

            foreach (var pageLink in pageObject.PageLinks)
            {
                var links = document.QuerySelectorAll(pageLink.Value)
                    .Select(l => l.GetAttribute(AttributeNames.Href))
                    .Select(href => BuildUri(baseUrl, href))
                    .Where(uri => !ExludedSchemas.Contains(uri.Scheme, StringComparer.OrdinalIgnoreCase)).ToList();
                
                links.ForEach(uri => queue.Enqueue(new CrawlRequest(uri, parent)));
            }
            
        }

        private void ExtractProperties(IHtmlDocument document, PageObject pageObject, TreeNode node)
        {
            if (pageObject.Properties.Count == 0)
                return;

            foreach (var property in pageObject.Properties)
            {
                var elements = document.QuerySelectorAll(property.Value);
                if(elements.Length == 0)
                    continue;
                
                var queryResult = elements.Select(e => e.InnerHtml).Aggregate((prod, next) => prod + ";" + next);
                node.Properties.Add(property.Key, queryResult);
            }
        }
        
        private IHtmlDocument GetDocument(Uri url)
        {
            var response = GetResponse(url).Result;
            return ParseResponse(response).Result;
        }

        private static Uri BuildUri(Uri baseUrl, string href)
        {
            try
            {
                return new Uri(baseUrl, new Uri(href, UriKind.RelativeOrAbsolute));
            }
            catch (Exception e)
            {
                throw new FormatException($"Invalid URI format: '{href}'.", e);
            }
        }
        
        private async Task<IHtmlDocument> ParseResponse(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
                return null;
            
            using (var contentStream = await responseMessage.Content.ReadAsStreamAsync())
            {
                var parser = new HtmlParser();
                return await parser.ParseDocumentAsync(contentStream);
            }
        }
        
        private async Task<HttpResponseMessage> GetResponse(Uri url)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                RequestUri = url,
                Method = HttpMethod.Get,
            };
            using (var client = new HttpClient())
            {
                return await client.SendAsync(httpRequestMessage);
            }
        }
    }
}