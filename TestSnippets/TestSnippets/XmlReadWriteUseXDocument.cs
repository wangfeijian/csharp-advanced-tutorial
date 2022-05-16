using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestSnippets
{
    /// <summary>
    /// 使用XDocument进行xml的读写操作
    /// 需要引用System.Xml.Linq命名空间
    /// </summary>
    public static class XmlReadWriteUseXDocument
    {
        /// <summary>
        /// 使用XDocument来创建一个xml文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="root">根节点</param>
        public static void CreateXmlFile(string fileName, string root)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(root));

            doc.Save(fileName);
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
            XDocument doc = XDocument.Load(fileName);
            var rootElement = doc.Descendants(root).FirstOrDefault();

            if (rootElement == null)
                throw new Exception("没有指定的根节点！");

            XElement nodeElement = new XElement(node,
                new XElement(attributeNode,
                new XAttribute("name","TestX"),
                new XAttribute("version","1.0")));

            rootElement.Add(nodeElement);

            doc.Save(fileName);
        }

        /// <summary>
        /// 获取xml文件指定节点中指定的attribute的值
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">节点特性</param>
        public static void GetXmlNodeAttributeValue(string fileName, string node, string attribute)
        {
            XDocument doc = XDocument.Load(fileName);

            var elements = doc.Descendants(node).Where(item => item.HasAttributes);
            foreach (var element in elements)
            {
                Console.WriteLine($"{attribute}: {element.Attribute(attribute).Value}"); 
            }
        }
    }
}
