using System;

namespace WebScraper
{
    public class CrawlRequest
    {
        public readonly Uri Url;
        public TreeNode ParentNode;

        public CrawlRequest(Uri url, TreeNode parent)
        {
            this.Url = url;
            this.ParentNode = parent;
        }

    }
}