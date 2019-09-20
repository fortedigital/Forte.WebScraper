using System;
using AngleSharp;
using CommandLine;

namespace WebScraper
{
    static class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    var pageObjects = new SettingsReader().ReadSettings(o.InputPath);
                    var config = Configuration.Default.WithXPath();
                    var context = BrowsingContext.New(config);
                    var crawler = new Crawler(o, context, pageObjects);
                    crawler.Crawl();
                });
            Console.WriteLine("Finished scraping");
        }
    }
}