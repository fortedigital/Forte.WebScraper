using System;
using CommandLine;

namespace WebScraper
{
    static class Program
    {
        static void Main(string[] args)
        {
            var pageObjects = new SettingsReader().ReadSettings();
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    var crawler = new Crawler(o, pageObjects);
                    crawler.Crawl();
                });
            Console.WriteLine("Finished scraping");
        }
    }
}