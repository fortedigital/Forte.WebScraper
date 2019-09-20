using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
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
        private readonly IBrowsingContext context;
        private readonly Options options;

        public Crawler(Options opts, IBrowsingContext context, List<PageObject> pageObjects)
        {
            this.context = context;
            this.pageObjects = pageObjects;
            options = opts;
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
                var result = GetCrawlResult(request.Url);
                foreach (var pageObj in pageObjects)
                {
                    if (pageObj.TestConditions == null)
                        continue;
                    if (pageObj.TestConditions != null && !pageObj.TestConditions.Evaluate(result))
                        continue;
                    
                    var node = new TreeNode(pageObj.PageName);
                    ExtractProperties(result.Document, pageObj, node);
                    
                    ExtractLinksToQueue(result.Document, pageObj, node, request.Url);

                    if (request.ParentNode == null)
                    {
                        parentNodes.Add(node);
                    }
                    else
                    {
                        request.ParentNode.ChildNodes.Add(node);
                    }
                    
                    Console.WriteLine("Scraped " + request.Url.AbsoluteUri + " <- " + pageObj.PageName);
                }
            }

            SavePropertiesToFile(options.OutputPath);
        }

        private void SavePropertiesToFile(string path)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            var serializedJson = JsonConvert.SerializeObject(parentNodes, serializerSettings);
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
                pageLink.Deconstruct(out var linkName, out var selector);
                
                var links = document.QuerySelectorAll(selector)
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
                property.Deconstruct(out var propertyName, out var propertyObject);
                
                var elements = document.QuerySelectorAll(propertyObject.Selector);
                if(elements.Length == 0)
                    continue;

                var extractorResult = propertyObject.ExtractProperties(elements);
                node.Properties.Add(propertyName, extractorResult);
            }
        }
        
        private CrawlResult GetCrawlResult(Uri url)
        {
            var response = GetResponse(url).Result;
            var document = ParseResponse(response).Result;
            return new CrawlResult(url, document);
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
                var parser = new HtmlParser(new HtmlParserOptions(), context);
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