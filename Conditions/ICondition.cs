using WebScraper.Models;

namespace WebScraper.Conditions
{
    public interface ICondition
    {
        bool Evaluate(CrawlResult result);
        
    }
}