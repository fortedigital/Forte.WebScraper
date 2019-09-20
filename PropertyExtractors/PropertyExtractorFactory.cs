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
                return new InnerHtmlPropertyExtractor();
            }

            throw new FormatException(value + " is not a valid extractor");
        }
        
    }
}