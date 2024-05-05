using System.Buffers;
using System.Collections.Generic;
using System.Windows.Forms;
using SCEditor.ScOld;

namespace SCEditor
{
    internal static class ExtensionMethods
    {
        private static Dictionary<int, int> nodeCount;
        private static Dictionary<int, string> dataTypeToName;
        private static HashSet<string> usedIds;
        
        public static void Populate(this TreeView tv, List<ScObject> scd)
        {
            nodeCount = new Dictionary<int, int>();
            dataTypeToName = new Dictionary<int, string>();
            usedIds = new HashSet<string>();

            Dictionary<int, List<TreeNode>> nodes = new Dictionary<int, List<TreeNode>>();
            foreach (var data in scd)
            {
                var dataTypeKey = data.GetDataType();
                var id = data.Id.ToString();

                if (!nodes.TryGetValue(dataTypeKey, out var list))
                {
                    nodes.Add(dataTypeKey, list = new List<TreeNode>());
                    nodeCount.Add(dataTypeKey, 0);
                    dataTypeToName.Add(dataTypeKey, data.GetDataTypeName());
                }
                else
                {
                    nodeCount[dataTypeKey] += 1;
                }
                
                if (dataTypeKey == 7)
                {
                    int i = 1;
                    while (true)
                    {
                        if (usedIds.Contains(id))
                        {
                            id = id + $"_{i}";
                        }
                        else
                        {
                            break;
                        }
                        i++;
                    } 
                }

                usedIds.Add(id);

                TreeNode node = new TreeNode
                {
                    Name = id,
                    Text = data.GetName(),
                    Tag = data
                };
                node.PopulateChildren(data);
                list.Add(node);
            }
            
            foreach (var node in nodes)
            {
                TreeNode treeNode = new TreeNode
                {
                    Name = node.Key.ToString(),
                    Text = dataTypeToName[node.Key]
                };
                treeNode.Nodes.AddRange(node.Value.ToArray());
                tv.Nodes.Add(treeNode);
            }
        }

        public static void PopulateChildren(this TreeNode tn, ScObject sco)
        {
            if (sco.Children == null || sco.Children.Count <= 0)
                return;

            if (sco.Children.Count == 1)
            {
                tn.Nodes.Add(CreateTreeNode(sco.Children[0]));
                return;
            }
            
            TreeNode[] treeNodes = new TreeNode[sco.Children.Count];
            for (int index = 0; index < sco.Children.Count; ++index)
            {
                treeNodes[index] = CreateTreeNode(sco.Children[index]);
            }
            
            tn.Nodes.AddRange(treeNodes);
        }

        private static TreeNode CreateTreeNode(ScObject sco)
        {
            TreeNode treeNode = new TreeNode(sco.GetName())
            {
                Name = sco.Id.ToString(),
                Tag = sco
            };
            PopulateChildren(treeNode, sco);
            return treeNode;
        }
    }
}
