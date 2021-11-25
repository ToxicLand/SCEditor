using System.Collections.Generic;
using System.Windows.Forms;
using SCEditor.ScOld;

namespace SCEditor
{
    internal static class ExtensionMethods
    {
        private static Dictionary<string, int> nodeCount;
        public static void Populate(this TreeView tv, List<ScObject> scd)
        {
            nodeCount = new Dictionary<string, int>();

            foreach (var data in scd)
            {
                var dataTypeKey = data.GetDataType().ToString();
                var dataTypeName = data.GetDataTypeName();
                var id = data.Id.ToString();

                if (!tv.Nodes.ContainsKey(dataTypeKey))
                {
                    tv.Nodes.Add(dataTypeKey, dataTypeName);
                    nodeCount.Add(dataTypeKey, 0);
                }
                else
                {
                    nodeCount[dataTypeKey] = tv.Nodes[dataTypeKey].Nodes.Count;
                }

                if (dataTypeKey == "7")
                {
                    int i = 1;
                    while (true)
                    {
                        if (tv.Nodes[dataTypeKey].Nodes.ContainsKey(id))
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

                tv.Nodes[dataTypeKey].Nodes.Add(id, data.GetName());
                tv.Nodes[dataTypeKey].Nodes[id].Tag = data;
                tv.Nodes[dataTypeKey].Nodes[id].PopulateChildren(data);
            }
        }

        public static void PopulateChildren(this TreeNode tn, ScObject sco)
        {
            if (sco.Children == null || sco.Children.Count <= 0)
                return;

            foreach (var child in sco.Children)
            {
                tn.Nodes.Add(child.Id.ToString(), child.GetName());
                tn.Nodes[child.Id.ToString()].Tag = child;

                if (child != null)
                    PopulateChildren(tn.Nodes[child.Id.ToString()], child);
            }
        }
    }
}
