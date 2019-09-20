using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebScraper.Models
{
    [JsonObject]
    public class TreeNode
    {
        public string Name { get; set; }
        
        public Dictionary<string, string> Properties { get; set; }
        
        public List<TreeNode> ChildNodes { get; set; }
        
        public TreeNode(string name)
        {
            Name = name;
            Properties = new Dictionary<string, string>();
            ChildNodes = new List<TreeNode>();
        }

    }
}