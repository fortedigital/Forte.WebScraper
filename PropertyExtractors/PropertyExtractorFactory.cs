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

            selector = null;
            return null;
        }
        
    }
}