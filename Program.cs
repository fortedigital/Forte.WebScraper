using System;
using System.Linq;
using AngleSharp;
using AngleSharp.Css.Parser;
using CommandLine;

namespace WebScraper
{
    static class Program
    {
        static void Main(string[] args)
        {
            var pageObjects = new SettingsReader().ReadSettings();
            var config = Configuration.Default.WithXPath();
            var context = BrowsingContext.New(config);
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    var crawler = new Crawler(o, context, pageObjects);
                    crawler.Crawl();
                });
            Console.WriteLine("Finished scraping");
        }
    }
}