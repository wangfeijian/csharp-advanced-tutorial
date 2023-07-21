#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：MySourceGenerator
 * 唯一标识：396ad2b8-a842-44fb-812c-ccd549eae4b5
 * 文件名：StationGenerator
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/20/2023 4:14:10 PM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>


using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MySourceGenerator
{
    [Generator]
    public class StationGenerator : ISourceGenerator
    {
        const string fileName = "StationInfo.xml";

        private string GetStrForAttribute(string name, XElement element)
        {
            var names = element.Descendants(name).Where(item => item.HasAttributes);
            var nameList = names.Select(item => $"\"{item.Attribute("Name").Value}\"").ToList();
            string strs = string.Join(",\r\n", nameList);
            return strs;
        }

        private string GetStrNonSymForAttribute(string name, XElement element)
        {
            var names = element.Descendants(name).Where(item => item.HasAttributes);
            var nameList = names.Select(item => item.Attribute("Name").Value).ToList();
            string strs = string.Join(",\r\n", nameList);
            return strs;
        }

        private void ReplaceCode(XDocument doc, GeneratorExecutionContext context, string code)
        {
            var stations = doc.Descendants("Station").Where(item => item.HasAttributes);
            foreach (var station in stations)
            {
                string stationName = station.Attribute("TypeName").Value;
                string stationCode = code.Replace("{ClassName}", stationName);

                string ioInStr = GetStrForAttribute("IoIn", station);
                stationCode = stationCode.Replace("{Ins}", ioInStr);
                string ioOutStr = GetStrForAttribute("IoOut", station);
                stationCode = stationCode.Replace("{Outs}", ioOutStr);
                string cylinderStr = GetStrForAttribute("Cylinder", station);
                stationCode = stationCode.Replace("{Cylinders}", cylinderStr);
                string pointStr = GetStrNonSymForAttribute("Point", station);
                stationCode = stationCode.Replace("{Points}", pointStr);

                context.AddSource($"{stationName}.g.cs", stationCode);
            }
        }

        public void Execute(GeneratorExecutionContext context)
        {
            string code = Resource.Station;
            string namespaceStr = context.Compilation.AssemblyName;
            code = code.Replace("{NameSpase}", namespaceStr);

            IEnumerable<AdditionalText> stationTxtFiles = context.AdditionalFiles
           .Where(af => string.Equals(Path.GetFileName(af.Path), fileName, StringComparison.OrdinalIgnoreCase));
            if (!stationTxtFiles.Any())
            {
                return;
            }

            foreach (AdditionalText file in stationTxtFiles)
            {
                try
                {
                    XDocument doc = XDocument.Load(file.Path);
                    ReplaceCode(doc, context, code);
                }
                catch (Exception)
                {
                    return;
                }

            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}