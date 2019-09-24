using System;

namespace WebScraper.PropertyExtractors
{
    public static class PropertyExtractorFactory
    {
        public static IPropertyExtractor GetPropertyExtractor(string value, out string selector)
        {
            if (value.StartsWith("innerhtml:"))
            {
                selector = value.Replace("innerhtml:", "");
                return new HtmlPropertyExtractor(e => e.InnerHtml);
            }

            if (value.StartsWith("outerhtml:"))
            {
                selector = value.Replace("outerhtml:", "");
                return new HtmlPropertyExtractor(e => e.OuterHtml);
            }
            
            if (value.StartsWith("innertext:"))
            {
                selector = value.Replace("innertext:", "");
                return new InnerTextPropertyExtractor();
            }

            if (value.StartsWith("download:"))
            {
                selector = value.Replace("download:", "");
                return new DownloadsPropertyExtractor();;
            }

            if (value.StartsWith("image:"))
            {
                selector = value.Replace("image:", "");
                return new ImagePropertyExtractor();
            }

            throw new FormatException(value + " is not a valid extractor");
        }
        
    }
}