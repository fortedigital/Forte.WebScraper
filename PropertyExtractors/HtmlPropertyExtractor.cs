using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using AngleSharp.Dom;
using WebScraper.Models;

namespace WebScraper.PropertyExtractors
{
    public class HtmlPropertyExtractor : IPropertyExtractor
    {
        private readonly Func<IElement, string> htmlSelector;

        public HtmlPropertyExtractor(Func<IElement, string> htmlSelector)
        {
            this.htmlSelector = htmlSelector;
        }

        public object ExtractProperties(CrawlResult crawlResult, IHtmlCollection<IElement> elements)
        {
            Directory.CreateDirectory(@"c:\temp\WebScraper");

            using (var client = new HttpClient())
            {
                foreach (var img in elements.SelectMany(e => e.QuerySelectorAll("img")))
                {
                    var src = new Uri(crawlResult.RequestUrl, new Uri(img.Attributes["src"].Value, UriKind.RelativeOrAbsolute));

                    var fileName = Path.Combine(@"c:\temp\WebScraper", Path.GetFileName(src.LocalPath));
                    
                    if (File.Exists(fileName) == false)
                    {
                        Console.WriteLine($"Downloading {src} to {fileName}");

                        using (var f = File.OpenWrite(fileName))
                        using (var s = client.GetStreamAsync(src).Result)
                        {
                            s.CopyTo(f);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Skipping download of {src} to {fileName}");
                    }

                    img.SetAttribute("data-local-src", fileName);
                }
            }
            
            return elements.Select(this.htmlSelector).Aggregate((prod, next) => prod + "\n" + next);
        }
    }
}