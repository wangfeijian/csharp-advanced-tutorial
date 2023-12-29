#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Services
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
 * 修改人：王飞箭
 * 时间：2023/12/25
 * 修改说明：
 * 1、增加参数集合来转换配置文件中的内容
 * 2、添加从xml文件加载参数到程序的方法
 * 3、添加通用方法，使用反射来初始化对象，避免出现冗余代码
 * 版本：V1.0.2
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using Newtonsoft.Json;
using Soso.Common.FileHelp;
using Soso.Contract;
using Soso.Contract.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Soso.Services
{
    public sealed class ConfigServices : SingletonInstance<ConfigServices>
    {
        private static readonly string _configPath = AppContext.BaseDirectory + $"Config\\system.json";
        private Dictionary<string, string> _configs = new Dictionary<string, string>();
        private List<SocketServerParameter> _socketServerParameters;
        private List<SocketClientParameter> _socketClientParameters;
        private List<SerialPortParameter> _serialPortParameters;
        private List<SystemParameter> _systemParameters;
        public List<SocketServerParameter> SocketServerParameters => _socketServerParameters;
        public List<SocketClientParameter> SocketClientParameters => _socketClientParameters;
        public List<SerialPortParameter> SerialPortParameters => _serialPortParameters;
        public List<SystemParameter> SystemParameters => _systemParameters;
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
            InitSystemParameters();
            InitSocketParameters();
            InitSerialPortParameters();
        }

        public void InitSystemParameters()
        {
            InitObjectFromXml(ref _systemParameters, SystemParameterPath, "Param");
        }

        public void InitSocketParameters()
        {
            InitObjectFromXml(ref _socketServerParameters, CommunicateParameterPath, "Server");
            InitObjectFromXml(ref _socketClientParameters, CommunicateParameterPath, "Client");
        }

        public void InitSerialPortParameters()
        {
            InitObjectFromXml(ref _serialPortParameters, CommunicateParameterPath, "SerialPort");
        }

        private void InitObjectFromXml<T>(ref List<T> paramList, string filePath, string elementName) where T : new()
        {
            XmlHelp xmlHelp = new XmlHelp(filePath);
            var xelements = xmlHelp.GetXElements(elementName, true);

            if (xelements.Count() <= 0)
            {

                throw new Exception($"File {filePath}, Not Exist Server Node!");
            }

            if (paramList != null && paramList.Count() != xelements.Count())
            {
                return;
            }
            else
            {
                paramList = new List<T>();
            }

            foreach (var xElement in xelements)
            {
                var result = xmlHelp.GetKeyValuesFromXElementAttributes(xElement);
                T param;// = new T();
                if (ConvertDataToT(result, elementName, out param))
                {
                    paramList.Add(param);
                }
            }
        }

        private bool ConvertDataToT<T>(Dictionary<string, string> data, string node, out T instance) where T : new()
        {
            instance = new T();// (T)Activator.CreateInstance(typeof(T));

            foreach (var item in typeof(T).GetProperties())
            {
                if (!item.CanWrite)
                {
                    continue;
                }

                string value;
                if (!data.TryGetValue(item.Name, out value))
                {
                    throw new Exception($"File {CommunicateParameterPath} {node} Node Not Exist {item.Name} Attribute!");
                }

                Type t = item.PropertyType;
                if (t == typeof(int))
                {
                    int intValue;
                    if (!int.TryParse(value, out intValue))
                    {
                        throw new Exception($"File {CommunicateParameterPath} {node} Node {item.Name} Attribute's Value Error!");
                    }
                    item.SetValue(instance, intValue);
                }
                else if (t == typeof(bool))
                {
                    bool boolValue;
                    if (!bool.TryParse(value, out boolValue))
                    {
                        throw new Exception($"File {CommunicateParameterPath} {node} Node {item.Name} Attribute's Value Error!");
                    }
                    item.SetValue(instance, boolValue);
                }
                else
                {
                    item.SetValue(instance, value);
                }
            }

            return true;
        }

        public void SaveSystemParameterXml(string key, string value)
        {
            XmlHelp xmlHelp = new XmlHelp(SystemParameterPath);
            var element = xmlHelp.GetXElements("Param", true, "Key", key).First();
            var keyValue = xmlHelp.GetKeyValuesFromXElementAttributes(element);
            keyValue["Value"] = value;

            xmlHelp.UpdateXElementAttribute(element, keyValue);
        }
    }
}