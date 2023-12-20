#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：SosoMS.SingletonServices
 * 唯一标识：776cc141-264b-47d3-80d1-5ac8695e58f2
 * 文件名：ConfigServices
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：8/25/2023 8:50:31 AM
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

using Newtonsoft.Json;
using Soso.Common.FileHelp;
using Soso.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Soso.Services
{
    public sealed class ConfigServices : SingletonInstance<ConfigServices>
    {
        private static readonly string _configPath = AppContext.BaseDirectory + $"Config\\system.json";
        private Dictionary<string, string> _configs;
        private List<SocketServerParameter> _socketServerParameters;
        private List<SocketClientParameter> _socketClientParameters;
        public List<SocketServerParameter> SocketServerParameters => _socketServerParameters;
        public List<SocketClientParameter> SocketClientParameters => _socketClientParameters;
        public string[] AllProjects
        {
            get
            {
                if (_configs.ContainsKey(nameof(AllProjects)))
                {
                    return _configs[nameof(AllProjects)].Split(',');
                }

                return new string[] { "Default" };
            }
        }

        public string CurrentProject
        {
            get
            {
                if (_configs.ContainsKey(nameof(CurrentProject)))
                {
                    return _configs[nameof(CurrentProject)];
                }

                return "Default";
            }
        }

        public string SystemParameterPath => AppContext.BaseDirectory + $"Config\\{CurrentProject}\\SystemParameters.xml";
        public string CommunicateParameterPath => AppContext.BaseDirectory + $"Config\\{CurrentProject}\\CommunicateParameters.xml";
        public string DataParameterPath => AppContext.BaseDirectory + $"Config\\{CurrentProject}\\DataParameters.xml";
        public string MotionParameterPath => AppContext.BaseDirectory + $"Config\\{CurrentProject}\\MotionParameters.xml";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ConfigServices()
        {
            try
            {
                _configs = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_configPath));
            }
            catch (Exception ex)
            {
                throw new Exception($"File {CommunicateParameterPath} Load Error!!", ex);
            }
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void InitAllParameters()
        {
            InitSocketParameters();
        }

        public void InitSocketParameters()
        {
            InitObjectFromXml(ref _socketServerParameters, CommunicateParameterPath, "Server");
            InitObjectFromXml(ref _socketClientParameters, CommunicateParameterPath, "Client");
        }

        private void InitObjectFromXml<T>(ref List<T> socketList, string filePath, string elementName) where T : new()
        {
            socketList = new List<T>();

            XmlHelp xmlHelp = new XmlHelp(filePath);
            var xelements = xmlHelp.GetXElements(elementName, true);

            if (xelements.Count() <= 0)
            {

                throw new Exception($"File {filePath}, Not Exist Server Node!");
            }

            foreach (var xElement in xelements)
            {
                var result = xmlHelp.GetKeyValuesFromXElementAttributes(xElement);
                var param = new T();
                if (ConvertDataToT(result, elementName, out param))
                {
                    socketList.Add(param);
                }
            }
        }

        private bool ConvertDataToT<T>(Dictionary<string, string> data, string node, out T instance) where T : new()
        {
            bool result = false;
            instance = (T)Activator.CreateInstance(typeof(T));

            foreach (var item in typeof(T).GetProperties())
            {
                string value;
                if (!data.TryGetValue(item.Name, out value))
                {
                    result = false;
                    throw new Exception($"File {CommunicateParameterPath} {node} Node Not Exist {item.Name} Attribute!");
                }

                Type t = item.PropertyType;
                if (t == typeof(int))
                {
                    int intValue;
                    if (!int.TryParse(value, out intValue))
                    {
                        result = false;
                        throw new Exception($"File {CommunicateParameterPath} {node} Node {item.Name} Attribute's Value Error!");
                    }
                    item.SetValue(instance, intValue);
                }
                else if (t == typeof(bool))
                {
                    bool boolValue;
                    if (!bool.TryParse(value, out boolValue))
                    {
                        result = false;
                        throw new Exception($"File {CommunicateParameterPath} {node} Node {item.Name} Attribute's Value Error!");
                    }
                    item.SetValue(instance, boolValue);
                }
                else
                {
                    item.SetValue(instance, value);
                }
            }
            result = true;

            return result;
        }
    }
}