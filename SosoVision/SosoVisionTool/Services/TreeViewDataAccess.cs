using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SosoVisionTool.Services
{
    /// <summary>
    /// TreeView操作类
    /// </summary>
    public class TreeViewDataAccess
    {

        public TreeViewDataAccess()
        {
        }

        /// <summary>
        /// TreeViewData
        /// </summary>
        [Serializable()]
        public struct TreeViewData
        {
            public TreeNodeData[] Nodes;

            /// <summary>
            /// 递归初始化TreeViewItem数据
            /// </summary>
            /// <param name="treeview"></param>
            public TreeViewData(TreeViewItem treeview)
            {
                Nodes = new TreeNodeData[treeview.Items.Count];
                if (treeview.Items.Count == 0)
                {
                    return;
                }
                for (int i = 0; i <= treeview.Items.Count - 1; i++)
                {
                    Nodes[i] = new TreeNodeData(treeview.Items[i] as TreeViewItem);
                }
            }

            /// <summary>
            /// 通过TreeViewData弹出TreeView
            /// </summary>
            /// <param name="treeview"></param>
            public void PopulateTree(TreeViewItem treeview)
            {
                if (this.Nodes == null || this.Nodes.Length == 0)
                {
                    return;
                }
                //treeview.BeginUpdate();
                for (int i = 0; i <= this.Nodes.Length - 1; i++)
                {
                    treeview.Items.Add(this.Nodes[i].ToTreeNode());
                }
                //treeview.EndUpdate();
            }
        }

        /// <summary>
        /// TreeNodeData
        /// </summary>
        [Serializable()]
        public struct TreeNodeData
        {
            public string Text;
            public string Name;
            public object Tag;
            public TreeNodeData[] Nodes;

            /// <summary>
            /// TreeNode构造函数
            /// </summary>
            /// <param name="node"></param>
            public TreeNodeData(TreeViewItem node)
            {
                this.Text = node.Header.ToString();
                this.Name = node.Name;
                this.Nodes = new TreeNodeData[node.Items.Count];

                if ((!(node.Tag == null)) && node.Tag.GetType().IsSerializable)
                {
                    this.Tag = node.Tag;
                }
                else
                {
                    this.Tag = null;
                }
                if (node.Items.Count == 0)
                {
                    return;
                }
                for (int i = 0; i <= node.Items.Count - 1; i++)
                {
                    Nodes[i] = new TreeNodeData(node.Items[i] as TreeViewItem);
                }
            }

            /// <summary>
            /// TreeNodeData返回TreeNode
            /// </summary>
            /// <returns></returns>
            public TreeViewItem ToTreeNode()
            {
                TreeViewItem ToTreeNode = new TreeViewItem();
                ToTreeNode.Header = this.Text;
                ToTreeNode.Tag = this.Tag;
                ToTreeNode.Name = this.Name;

                if (this.Nodes == null && this.Nodes.Length == 0)
                {
                    return null;
                }
                if (ToTreeNode != null && this.Nodes.Length == 0)
                {
                    return ToTreeNode;
                }
                for (int i = 0; i <= this.Nodes.Length - 1; i++)
                {
                    ToTreeNode.Items.Add(this.Nodes[i].ToTreeNode());
                }
                return ToTreeNode;
            }
        }
        /// <summary>
        /// 加载TreeViewItem
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="path"></param>
        public static void LoadTreeViewData(TreeViewItem treeView, string path)
        {
            BinaryFormatter ser = new BinaryFormatter();
            Stream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            TreeViewData treeData = ((TreeViewData)(ser.Deserialize(file)));
            treeData.PopulateTree(treeView);
            file.Close();

        }

        /// <summary>
        /// 保存TreeViewItem 到文件
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="path"></param>
        public static void SaveTreeViewData(TreeViewItem treeView, string path)
        {
            BinaryFormatter ser = new BinaryFormatter();
            Stream file = new FileStream(path, FileMode.Create);
            ser.Serialize(file, new TreeViewData(treeView));
            file.Close();

        }

        

       
    }
}
