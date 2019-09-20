using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace WebScraper
{
    public class Options
    {
        [Option('u', "url", Required = true, HelpText = "Start urls where the crawling will begin. You can pass sitemap url as well For multiple urls separate url with space. E.g. -u https://bbc.co.uk https://msn.com")]
        public IEnumerable<string> StartUrls { get; set; }

        [Option('d', "depth", Default = 3, HelpText = "Maximum depth of url to extract.")]
        public int MaxDepth { get; set; }

        [Option("maxErrors", Default = 100, HelpText = "Number of errors after which the crawler is stopped.")]
        public int MaxErrors { get; set; }

        [Option('w', "workers", Default = 3, HelpText = "Number of workers.")]
        public int NumberOfWorkers { get; set; }

        [Option('t', "timeout", Default = null, HelpText = "Request timeout in seconds.")]
        public int? RequestTimeout { get; set; }

        [Option("maxUrls", Default = 1000, HelpText = "Number of urls after which the crawler is stopped.")]
        public int MaxUrls { get; set; }

        [Option("minUrls", Default = 1, HelpText = "Minimum number of urls that is expected.")]
        public int MinUrls { get; set; }

        [Option('r',"retries", Default = 0, HelpText = "Maximum number of retries (use '0' for no retry).")]
        public int MaxRetries { get; set; }

        [Option("no-robots", HelpText = "Set this flag to disable parsing robots.txt in order to search for sitemaps")]
        public bool NoRobots { get; set; }

        [Option('h', "headers", HelpText = "HTTP headers sent with request. Format: header_name:header_value|header_name2:header_value2.")]
        public string RequestHeadersRawValue { get; set; }

        public IReadOnlyDictionary<string, string> RequestHeaders
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.RequestHeadersRawValue))
                {
                    return new Dictionary<string, string>();
                }

                return this.RequestHeadersRawValue
                    .Split(new[] {'|', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => string.IsNullOrEmpty(x) == false)
                    .Select(x => x.Split(':', 2))
                    .ToDictionary(x => x.ElementAt(0), x => x.ElementAtOrDefault(1));
            }
        }
    }
}