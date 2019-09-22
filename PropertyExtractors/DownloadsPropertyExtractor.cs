using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using AngleSharp.Dom;
using WebScraper.Models;

namespace WebScraper.PropertyExtractors
{
    public class DownloadsPropertyExtractor : IPropertyExtractor
    {
        public object ExtractProperties(CrawlResult crawlResult, IHtmlCollection<IElement> elements)
        {
            Directory.CreateDirectory(@"c:\temp\WebScraper");
            
            var result = new List<object>();
            
            using (var client = new HttpClient())
            {
                foreach (var element in elements.Where(e => e.TagName.Equals("a", StringComparison.OrdinalIgnoreCase)))
                {
                    var href = new Uri(crawlResult.RequestUrl, new Uri(element.Attributes["href"].Value, UriKind.RelativeOrAbsolute));
                    var fileName = Path.Combine(@"c:\temp\WebScraper", Path.GetFileName(href.LocalPath));
                    
                    if (File.Exists(fileName) == false)
                    {
                        Console.WriteLine($"Downloading {href} to {fileName}");

                        using (var f = File.OpenWrite(fileName))
                        using (var s = client.GetStreamAsync(href).Result)
                        {
                            s.CopyTo(f);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Skipping download of {href} to {fileName}");
                    }

                    result.Add(new { FileName = fileName, Title = element.TextContent});
                }
                
                return result;
            }            
        }
    }
}