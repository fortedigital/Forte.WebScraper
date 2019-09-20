using System.Collections.Generic;
using CommandLine;

namespace WebScraper
{
    public class Options
    {
        [Option('u', "url", Required = true, HelpText = "Start urls where the crawling will begin. You can pass sitemap url as well For multiple urls separate url with space. E.g. -u https://bbc.co.uk https://msn.com")]
        public IEnumerable<string> StartUrls { get; set; }

        [Option('i', "input", Required = true, HelpText = "Path to JSON file with configuration.")]
        public string InputPath { get; set; }
        
        [Option('o', "output", Required = true, HelpText = "Path to output JSON file.")]
        public string OutputPath { get; set; }
    }
}