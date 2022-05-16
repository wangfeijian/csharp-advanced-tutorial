using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestSnippets
{
    /// <summary>
    /// 使用XmlDocument的方式来操作Xml文件
    /// 需要引用System.Xml命名空间
    /// </summary>
    public static class XmlReadWrite
    {
        /// <summary>
        /// 创建一个xml文件，只指定根节点
        /// 如果文件路径中包含一个目录，需要先判断目录是否存在
        /// 如果不存在就需要先建立一个目录，再传入文件路径
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="root">根节点名称</param>
        public static void CreateXmlFile(string fileName, string root)
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "utf-8", null);
            document.AppendChild(xmlDeclaration);
            XmlElement xmlElement = document.CreateElement(root);
            document.AppendChild(xmlElement);

            SaveFileNoBomHead(fileName, document);
        }

        /// <summary>
        /// 保存文件为utf-8，且不带bom头
        /// </summary>
        /// <param name="fileName"></param>
        private static void SaveFileNoBomHead(string fileName, XmlDocument xml)
        {
            // 这里不能直接保存，直接保存会导致保存的xml文件包含有Bom头，在下一次读取的时候就会报错
            using (StreamWriter sw = new StreamWriter(fileName, false, new UTF8Encoding(false)))
            {
                xml.Save(sw);
            }
        }

        /// <summary>
        /// 往xml文件中添加一个子节点
        /// 并设置子节点中的attribute
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="root">根节点名称</param>
        /// <param name="node">添加的节点</param>
        /// <param name="attributeNode">添加节点的子节点</param>
        public static void AddXmlNodeAndSetAttribute(string fileName, string root, string node, string attributeNode)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            if (!xmlDocument.HasChildNodes)
                throw new XmlException("文件中没有子节点！");

            XmlNode xmlNode = xmlDocument.SelectNodes(root)[0];
            XmlElement xmlElementNode = xmlDocument.CreateElement(node);
            XmlElement xmlElementAttributeNode = xmlDocument.CreateElement(attributeNode);
            xmlElementAttributeNode.SetAttribute("name", "test");
            xmlElementAttributeNode.SetAttribute("version", "1.0");
            xmlElementNode.AppendChild(xmlElementAttributeNode);
            xmlNode.AppendChild(xmlElementNode);
            SaveFileNoBomHead(fileName, xmlDocument);
        }

        /// <summary>
        /// 获取xml文件指定节点中指定的attribute的值
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">节点特性</param>
        public static void GetXmlNodeAttributeValue(string fileName, string node, string attribute)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            if (!xmlDocument.HasChildNodes)
                throw new XmlException("文件中没有子节点！");

            XmlNodeList nodeList = xmlDocument.SelectNodes(node);
            if (nodeList == null || nodeList.Count == 0)
            {
                throw new XmlException($"文件中没有子节点 {node}！");
            }

            nodeList = nodeList.Item(0).ChildNodes;
            foreach (XmlNode childNode in nodeList)
            {
                XmlElement element = childNode as XmlElement;
                Console.WriteLine($"{attribute}: {element.GetAttribute(attribute)}");
            }
        }
    }
}
