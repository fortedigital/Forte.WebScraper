namespace WebScraper.Conditions
{
    public static class ConditionFactory
    {
        public static ICondition GetCondition(string value)
        {
            if (value.StartsWith("css:"))
                return new CssSelectorCondition(value.Replace("css:",""));
            
            if (value.StartsWith("value:"))
                return new ValueEqualCondition(value.Replace("value:",""));
            
            if (value.StartsWith("exists:"))
                return new ExistsCondition(value.Replace("exists:", ""));

            if(value.StartsWith("notexists:"))
                return new NotExistsCondition(value.Replace("notexists:",""));
            
            return null;
        } 
    }
}