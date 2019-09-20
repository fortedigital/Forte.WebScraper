namespace WebScraper.Conditions
{
    public static class ConditionFactory
    {
        public static ICondition GetCondition(string value)
        {
            if (value.StartsWith("value:"))
                return new ValueCondition(value);
            
            if (value.StartsWith("exists:") || value.StartsWith("notexists:"))
                return new ExistsCondition(value);
            
            if(value.StartsWith("urlcontains:") || value.StartsWith("urlnotcontains:"))
                return new UrlContainsCondition(value);
            
            return null;
        } 
    }
}