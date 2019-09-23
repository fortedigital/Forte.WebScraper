using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebScraper.Models
{
    [JsonObject]
    public class TreeNode
    {
        public string Name { get; set; }
        
        public Dictionary<string, object> Properties { get; set; }
        
        public List<TreeNode> ChildNodes { get; set; }
        
        public Dictionary<string, TreeNode> Languages { get; set; }
        
        [JsonIgnore]
        public TreeNode Parent { get; set; }
        
        public TreeNode(string name)
        {
            Name = name;
            Properties = new Dictionary<string, object>();
            ChildNodes = new List<TreeNode>();
            Languages = new Dictionary<string, TreeNode>();
        }

        public bool HasLanguagePage(TreeNode node)
        {
            return this.Languages.ContainsValue(node);
        }

        public bool ShouldSerializeLanguages()
        {
            return Languages.Count != 0;
        }
        
        public bool ShouldSerializeProperties()
        {
            return Properties.Count != 0;
        }

        public bool ShouldSerializeChildNodes()
        {
            return ChildNodes.Count != 0;
        }
    }
}