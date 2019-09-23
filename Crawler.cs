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
        private readonly List<TreeNode> rootNodes;
        private readonly IBrowsingContext context;
        private readonly Options options;

        public Crawler(Options opts, IBrowsingContext context, List<PageObject> pageObjects)
        {
            this.context = context;
            this.pageObjects = pageObjects;
            options = opts;
            queue = new Queue<CrawlRequest>();
            rootNodes = new List<TreeNode>();
            foreach (var url in opts.StartUrls)
            {
                queue.Enqueue(new CrawlRequest(new Uri(url), null));
            }
        }

        public void Crawl()
        {
            var visitedUrls = new HashSet<Uri>();
            
            while (queue.Count != 0)
            {
                var request = queue.Dequeue();
                
                if (visitedUrls.Contains(request.Url))
                    continue;
                visitedUrls.Add(request.Url);
                
                var result = GetCrawlResult(request.Url);
                foreach (var pageObj in pageObjects)
                {
                    if (pageObj.TestCondition == null)
                        continue;
                    if (pageObj.TestCondition != null && !pageObj.TestCondition(result))
                        continue;
                    
                    var node = new TreeNode(pageObj.PageName, result.RequestUrl);
                    lock (this.rootNodes)
                    {
                        if (request.ParentNode == null)
                        {
                            rootNodes.Add(node);
                        }
                        else
                        {
                            if (request.ParentNode.Languages.ContainsKey(result.Language))
                            {
                                request.ParentNode.Languages[result.Language] = node;
                            }
                            else
                            {
                                request.ParentNode.ChildNodes.Add(node);
                            }
                            node.Parent = request.ParentNode;
                        }
                    }
                    
                    
                    ExtractProperties(result, pageObj, node);

                    lock (this.rootNodes)
                    {
                        if (rootNodes.Contains(node) || !node.Parent.HasLanguagePage(node))
                        {
                            ExtractLinksToQueue(result, pageObj, node);
                            ExtractLanguages(result, pageObj, node);
                        }
                    }

                    ExtractPagination(result, pageObj, node);
                    
                    Console.WriteLine("Scraped " + request.Url.AbsoluteUri + " <- " + pageObj.PageName);
                    break;
                }
            }

            SavePropertiesToFile(options.OutputPath);
        }

        public void SavePropertiesToFile(string path)
        {
            lock (this.rootNodes)
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };
                var serializedJson = JsonConvert.SerializeObject(rootNodes, serializerSettings);
                using (var writer = File.CreateText(path))
                {
                    writer.Write(serializedJson);
                    writer.Flush();
                }
            }
        }
        
        private void ExtractPagination(CrawlResult crawlResult, PageObject pageObject, TreeNode parent)
        {
            if (string.IsNullOrWhiteSpace(pageObject.Pagination))
                return;
            
            Console.WriteLine(crawlResult.RequestUrl + " pagination:");
            var currentDoc = crawlResult.Document;
            var nextPageRef = currentDoc.QuerySelector(pageObject.Pagination).GetAttribute(AttributeNames.Href);
            while (nextPageRef != null)
            {
                var result = GetCrawlResult(BuildUri(crawlResult.RequestUrl, nextPageRef));
                Console.WriteLine("Extracting links from " + result.RequestUrl);
                ExtractLinksToQueue(result, pageObject, parent);
                
                currentDoc = result.Document;
                nextPageRef = currentDoc.QuerySelector(pageObject.Pagination)?.GetAttribute(AttributeNames.Href);
            }
        }

        private void ExtractLanguages(CrawlResult crawlResult, PageObject pageObject, TreeNode parent)
        {
            if (pageObject.Languages.Count == 0)
                return;
            
            foreach (var (lang, selector) in pageObject.Languages.Select(x => (x.Key, x.Value)))
            {
                parent.Languages.Add(lang, null);

                var link = crawlResult.Document.QuerySelector(selector)?.GetAttribute(AttributeNames.Href);
                if (link != null)
                    queue.Enqueue(new CrawlRequest(BuildUri(crawlResult.RequestUrl, link), parent));

            }
        }
        
        private void ExtractLinksToQueue(CrawlResult crawlResult, PageObject pageObject, TreeNode parent)
        {
            if (pageObject.PageLinks.Count == 0)
                return;

            foreach (var (linkName, selector) in pageObject.PageLinks.Select(x => (x.Key, x.Value)))
            {
                var links = crawlResult.Document.QuerySelectorAll(selector)
                    .Select(l => l.GetAttribute(AttributeNames.Href))
                    .Select(href => BuildUri(crawlResult.RequestUrl, href))
                    .Where(uri => !ExludedSchemas.Contains(uri.Scheme, StringComparer.OrdinalIgnoreCase)).ToList();
                
                links.ForEach(uri => queue.Enqueue(new CrawlRequest(uri, parent)));
            }
            
        }

        private void ExtractProperties(CrawlResult crawlResult, PageObject pageObject, TreeNode node)
        {
            if (pageObject.Properties.Count == 0)
                return;

            foreach (var (propertyName, propertyObject) in pageObject.Properties.Select(x => (x.Key, x.Value)))
            {
                var elements = crawlResult.Document.QuerySelectorAll(propertyObject.Selector);
                if(elements.Length == 0)
                    continue;

                var extractorResult = propertyObject.ExtractProperties(crawlResult, elements);
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