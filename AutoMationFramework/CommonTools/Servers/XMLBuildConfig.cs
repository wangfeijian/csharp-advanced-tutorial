/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-27                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Read write config file use XML           *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using CommonTools.Model;
using Microsoft.Win32;

namespace CommonTools.Servers
{
    public class XmlBuildConfig : IBuildConfig
    {
        public T LoadConfig<T>(string config)
        {
            object tCfg = new object();

            switch (config)
            {
                case "system":
                    //return (T)(object)LoadSystemConfig<SystemCfg>();
                case "point":
                    //return (T)(object)LoadPointConfig<List<StationPoint>>();
                case "systemParam":
                case "systemParamDefault":
                    return (T)(object)LoadParamConfig<Parameters>(config);
                case "otherConfig":
                    //return (T)(object)LoadOtherConfig<OtherConfig>();
                case "dataClass":
                    //return (T)(object)LoadDataClass<AllDataClass>();
                case "dataClassTitle":
                    //return (T)(object)new DataClassTitle();
                case "dataInfo":
                    //return (T)(object)new ObservableCollection<DataInfo>();
                case "dataShow":
                    //return (T)(object)LoadDataShow<DataShowClass>();
                case "dataSave":
                    break;
                    //return (T)(object)LoadDataSave<DataSaveClass>();
            }
            return (T)tCfg;
        }

        //private T LoadDataShow<T>() where T : DataShowClass
        //{
        //    string file = AppDomain.CurrentDomain.BaseDirectory + "systemCfgEx.xml";
        //    DataShowClass allData;

        //    if (!File.Exists(file))
        //    {
        //        allData = new DataShowClass
        //        {
        //            DataIndexes = new ObservableCollection<DataIndex>()
        //        };
        //        return allData as T;
        //    }
        //    else
        //    {
        //        allData = LoadDataShowFromFile<T>(file);

        //        return allData as T;
        //    }
        //}

        //private T LoadDataSave<T>() where T : DataSaveClass
        //{
        //    string file = AppDomain.CurrentDomain.BaseDirectory + "systemCfgEx.xml";
        //    DataSaveClass allData;

        //    if (!File.Exists(file))
        //    {
        //        allData = new DataSaveClass
        //        {
        //            DataIndexes = new ObservableCollection<DataIndex>()
        //        };
        //        return allData as T;
        //    }
        //    else
        //    {
        //        allData = LoadDataSaveFromFile<T>(file);

        //        return allData as T;
        //    }
        //}

        //private DataSaveClass LoadDataSaveFromFile<T>(string file)
        //{
        //    List<string> saveType = new List<string> { "DB", "CSV", "INI" };

        //    DataSaveClass allDataClass = new DataSaveClass
        //    {
        //        DataIndexes = new ObservableCollection<DataIndex>()
        //    };
        //    XDocument doc = XDocument.Load(file);

        //    foreach (var el in doc.Descendants("Data").Elements())
        //    {
        //        if (el.Name.ToString() == "DataSave")
        //        {

        //            allDataClass.IsUpload = el.Attribute("是否保存")?.Value == "True";
        //            allDataClass.SelectSaveType = saveType.IndexOf(el.Attribute("保存类型")?.Value);
        //            allDataClass.ServiceAddress = el.Attribute("Server")?.Value;
        //            allDataClass.Port = el.Attribute("Port")?.Value;
        //            allDataClass.User = el.Attribute("UserID")?.Value;
        //            allDataClass.Password = el.Attribute("Password")?.Value;
        //            allDataClass.DataBase = el.Attribute("Database")?.Value;
        //            allDataClass.DataTable = el.Attribute("TableName")?.Value;
        //            allDataClass.SavePath = el.Attribute("保存路径")?.Value;
        //            var dataInfo = from data in el.Descendants("Item")
        //                           where data.HasAttributes
        //                           select
        //                               new DataIndex
        //                               {
        //                                   Name = data.Attribute("名称")?.Value,
        //                                   Index = data.Attribute("数据索引")?.Value
        //                               };

        //            var listDataInfo = dataInfo.ToList();
        //            var obList = new ObservableCollection<DataIndex>();
        //            listDataInfo.ForEach(item => obList.Add(item));

        //            allDataClass.DataIndexes = obList;
        //        }
        //    }

        //    return allDataClass;
        //}

        //private DataShowClass LoadDataShowFromFile<T>(string file)
        //{
        //    DataShowClass allDataClass = new DataShowClass
        //    {
        //        DataIndexes = new ObservableCollection<DataIndex>()
        //    };
        //    XDocument doc = XDocument.Load(file);

        //    foreach (var el in doc.Descendants("Data").Elements())
        //    {
        //        if (el.Name.ToString() == "DataShow")
        //        {

        //            allDataClass.IsUpload = el.Attribute("是否保存")?.Value == "True";
        //            var dataInfo = from data in el.Descendants("Item")
        //                           where data.HasAttributes
        //                           select
        //                               new DataIndex
        //                               {
        //                                   Name = data.Attribute("名称")?.Value,
        //                                   Index = data.Attribute("数据索引")?.Value
        //                               };

        //            var listDataInfo = dataInfo.ToList();
        //            var obList = new ObservableCollection<DataIndex>();
        //            listDataInfo.ForEach(item => obList.Add(item));

        //            allDataClass.DataIndexes = obList;
        //        }
        //    }

        //    return allDataClass;
        //}
        //private T LoadDataClass<T>() where T : AllDataClass
        //{
        //    string file = AppDomain.CurrentDomain.BaseDirectory + "systemCfgEx.xml";
        //    AllDataClass allData;

        //    if (!File.Exists(file))
        //    {
        //        allData = new AllDataClass
        //        {
        //            ComboxStrList = new ObservableCollection<string>(),
        //            DataTitleDictionary = new Dictionary<string, DataClassTitle>(),
        //            DataInfoDictionary = new Dictionary<string, ObservableCollection<DataInfo>>()
        //        };
        //        return allData as T;
        //    }
        //    else
        //    {
        //        allData = LoadAllDataFromFile<T>(file);

        //        return allData as T;
        //    }
        //}

        //private AllDataClass LoadAllDataFromFile<T>(string file)
        //{
        //    AllDataClass allDataClass = new AllDataClass
        //    {
        //        ComboxStrList = new ObservableCollection<string>(),
        //        DataInfoDictionary = new Dictionary<string, ObservableCollection<DataInfo>>(),
        //        DataTitleDictionary = new Dictionary<string, DataClassTitle>()
        //    };
        //    XDocument doc = XDocument.Load(file);

        //    foreach (var el in doc.Descendants("Data").Elements())
        //    {
        //        if (el.Name.ToString() == "Data")
        //        {

        //            var dataTitle = new DataClassTitle
        //            {
        //                Name = el.Attribute("名称")?.Value,
        //                Version = el.Attribute("版本")?.Value,
        //                PdcaEnable = el.Attribute("PDCA")?.Value == "True",
        //                Authority = Convert.ToInt32(el.Attribute("权限")?.Value)
        //            };
        //            var dataInfo = from data in el.Descendants("Item")
        //                           where data.HasAttributes
        //                           select
        //                               new DataInfo
        //                               {
        //                                   Name = data.Attribute("名称")?.Value,
        //                                   DataType = data.Attribute("数据类型")?.Value,
        //                                   DataIndex = data.Attribute("数据索引")?.Value,
        //                                   StandardValue = data.Attribute("标准值")?.Value,
        //                                   UpperValue = data.Attribute("上限")?.Value,
        //                                   LowerValue = data.Attribute("下限")?.Value,
        //                                   OffsetValue = data.Attribute("补偿值")?.Value,
        //                                   Unit = data.Attribute("单位")?.Value,
        //                               };

        //            var listDataInfo = dataInfo.ToList();
        //            var obList = new ObservableCollection<DataInfo>();
        //            listDataInfo.ForEach(item => obList.Add(item));

        //            string name = dataTitle.Name;

        //            if (name != null)
        //            {
        //                allDataClass.ComboxStrList.Add(name);
        //                allDataClass.DataTitleDictionary[name] = dataTitle;
        //                allDataClass.DataInfoDictionary[name] = obList;
        //            }
        //        }
        //    }

        //    return allDataClass;
        //}

        //private T LoadOtherConfig<T>() where T : OtherConfig
        //{
        //    string file = AppDomain.CurrentDomain.BaseDirectory + "systemCfgEx.xml";
        //    OtherConfig otherConfig;

        //    if (!File.Exists(file))
        //    {
        //        otherConfig = new OtherConfig
        //        {
        //            CylinderInfos = new List<CylinderInfo>(),
        //            LightInfos = new List<LightInfo>(),
        //            GrrInfos = new List<ReflectInfo>(),
        //            CalibInfos = new List<ReflectInfo>(),
        //            ServersInfos = new List<ServersInfo>()
        //        };
        //        return otherConfig as T;
        //    }
        //    else
        //    {
        //        otherConfig = LoadOtherConfigFromFile<T>(file);

        //        return otherConfig as T;
        //    }
        //}

        //private OtherConfig LoadOtherConfigFromFile<T>(string file) where T : OtherConfig
        //{
        //    XDocument doc = XDocument.Load(file);
        //    var cylinder = from item in doc.Descendants("Cylinder")
        //                   where item.HasAttributes
        //                   select
        //                       new CylinderInfo
        //                       {
        //                           Name = item.Attribute("名称")?.Value,
        //                           EngName = item.Attribute("翻译")?.Value,
        //                           Type = item.Attribute("类型")?.Value,
        //                           ExtendOutput = item.Attribute("伸出输出")?.Value,
        //                           RetractOutput = item.Attribute("缩回输出")?.Value,
        //                           ExtendInput = item.Attribute("伸出输入")?.Value,
        //                           RetractInput = item.Attribute("缩回输入")?.Value,
        //                           ExtendEnable = item.Attribute("伸出启用")?.Value,
        //                           RetractEnable = item.Attribute("缩回启用")?.Value,
        //                           TimeOut = item.Attribute("超时时间")?.Value,
        //                       };
        //    var light = from item in doc.Descendants("light")
        //                where item.HasAttributes
        //                select
        //                    new LightInfo
        //                    {
        //                        Name = item.Attribute("名称")?.Value,
        //                        Type = item.Attribute("类型")?.Value,
        //                        Aisle = item.Attribute("通道")?.Value,
        //                        AisleIndex = item.Attribute("通信索引")?.Value,
        //                    };
        //    var calib = from item in doc.Descendants("Calib").Elements()
        //                where item.HasAttributes
        //                select
        //                    new ReflectInfo
        //                    {
        //                        Name = item.Attribute("名称")?.Value,
        //                        StationName = item.Attribute("站位")?.Value,
        //                        MethodName = item.Attribute("方法")?.Value,
        //                    };
        //    var grr = from item in doc.Descendants("GRR").Elements()
        //              where item.HasAttributes
        //              select
        //                  new ReflectInfo
        //                  {
        //                      Name = item.Attribute("名称")?.Value,
        //                      StationName = item.Attribute("站位")?.Value,
        //                      MethodName = item.Attribute("方法")?.Value,
        //                  };
        //    var server = from item in doc.Descendants("Server")
        //                 where item.HasAttributes
        //                 select
        //                     new ServersInfo
        //                     {
        //                         Index = item.Attribute("序号")?.Value,
        //                         IpAddress = item.Attribute("本地IP地址")?.Value,
        //                         ListenPort = item.Attribute("监听端口")?.Value,
        //                         Enable = item.Attribute("启用")?.Value,
        //                     };
        //    var otherConfig = new OtherConfig
        //    {
        //        CylinderInfos = cylinder.ToList(),
        //        LightInfos = light.ToList(),
        //        CalibInfos = calib.ToList(),
        //        GrrInfos = grr.ToList(),
        //        ServersInfos = server.ToList(),
        //    };
        //    return otherConfig;
        //}

        private T LoadParamConfig<T>(string fileName) where T : Parameters
        {
            string file = AppDomain.CurrentDomain.BaseDirectory+"Config\\"+fileName+".xml";
            Parameters parameters;

            if (!File.Exists(file))
            {
                parameters = new Parameters
                {
                    ParameterInfos = new List<ParamInfo>()
                };
                return parameters as T;
            }
            else
            {
                parameters = LoadParamFromFile<T>(file);

                return parameters as T;
            }
        }

        private Parameters LoadParamFromFile<T>(string file) where T : Parameters
        {
            XDocument doc = XDocument.Load(file);
            var param = from item in doc.Descendants("Param")
                        where item.HasAttributes
                        select
                            new ParamInfo
                            {
                                KeyValue = item.Attribute("键值")?.Value,
                                CurrentValue = item.Attribute("当前值")?.Value,
                                Unit = item.Attribute("单位")?.Value,
                                ParamDesc = item.Attribute("参数描述")?.Value,
                                EnglishDesc = item.Attribute("翻译")?.Value,
                                MinValue = item.Attribute("最小值")?.Value,
                                MaxValue = item.Attribute("最大值")?.Value,
                            };

            var parameters = new Parameters
            {
                ParameterInfos = param.ToList()
            };

            foreach (var parameter in parameters.ParameterInfos)
            {
                parameter.CheckValue();
            }
            return parameters;
        }


        //private T LoadSystemConfig<T>() where T : SystemCfg
        //{
        //    string file = AppDomain.CurrentDomain.BaseDirectory + "systemCfg.xml";
        //    SystemCfg systemCfg;

        //    if (!File.Exists(file))
        //    {
        //        systemCfg = new SystemCfg
        //        {
        //            IoInput = new List<IoInputPoint>(),
        //            IoOutput = new List<IoOutputPoint>(),
        //            IoCardsList = new List<IoCardInfo>(),
        //            SysInput = new List<SysInputPoint>(),
        //            SysOutput = new List<SysOutputPoint>(),
        //            MotionCardsList = new List<MotionCard>(),
        //            EthInfos = new List<EthInfo>(),
        //            StationInfos = new List<StationInfo>()
        //        };
        //        return systemCfg as T;
        //    }
        //    else
        //    {
        //        systemCfg = LoadSystemFromFile<T>(file);

        //        return systemCfg as T;
        //    }
        //}

        //private SystemCfg LoadSystemFromFile<T>(string file) where T : SystemCfg
        //{
        //    SystemCfg systemCfg;
        //    XDocument doc = XDocument.Load(file);
        //    var cardList = from item in doc.Descendants("IoCard")
        //                   where item.HasAttributes
        //                   select
        //                       new IoCardInfo
        //                       {
        //                           CardIndex = item.Attribute("卡序号")?.Value,
        //                           CardNum = item.Attribute("卡号")?.Value,
        //                           CardType = item.Attribute("卡类型")?.Value
        //                       };
        //    var ioInput = from item in doc.Descendants("IoIn")
        //                  where item.HasAttributes
        //                  select
        //                      new IoInputPoint
        //                      {
        //                          CardIndex = item.Attribute("卡序号")?.Value,
        //                          PointIndex = item.Attribute("点序号")?.Value,
        //                          PointName = item.Attribute("点位名称")?.Value,
        //                          PointEngName = item.Attribute("点位翻译")?.Value
        //                      };
        //    var ioOutput = from item in doc.Descendants("IoOut")
        //                   where item.HasAttributes
        //                   select
        //                       new IoOutputPoint
        //                       {
        //                           CardIndex = item.Attribute("卡序号")?.Value,
        //                           PointIndex = item.Attribute("点序号")?.Value,
        //                           PointName = item.Attribute("点位名称")?.Value,
        //                           PointEngName = item.Attribute("点位翻译")?.Value
        //                       };
        //    var systemIoIn = from item in doc.Descendants("IoSystemIn")
        //                     where item.HasAttributes
        //                     select
        //                         new SysInputPoint
        //                         {
        //                             FuncDesc = item.Attribute("功能描述")?.Value,
        //                             PointIndex = item.Attribute("点序号")?.Value,
        //                             CardNum = item.Attribute("卡序号")?.Value,
        //                             EffectiveLevel = item.Attribute("有效电平")?.Value
        //                         };
        //    var systemIoOut = from item in doc.Descendants("IoSystemOut")
        //                      where item.HasAttributes
        //                      select
        //                          new SysOutputPoint
        //                          {
        //                              FuncDesc = item.Attribute("功能描述")?.Value,
        //                              PointIndex = item.Attribute("点序号")?.Value,
        //                              CardNum = item.Attribute("卡序号")?.Value,
        //                              EffectiveLevel = item.Attribute("有效电平")?.Value
        //                          };
        //    var motion = from item in doc.Descendants("Motion")
        //                 where item.HasAttributes
        //                 select
        //                     new MotionCard
        //                     {
        //                         CardType = item.Attribute("卡类型")?.Value,
        //                         Index = item.Attribute("序号")?.Value,
        //                         MinAxisNum = item.Attribute("最小轴号")?.Value,
        //                         MaxAxisNum = item.Attribute("最大轴号")?.Value
        //                     };
        //    var station = from item in doc.Descendants("Station")
        //                  where item.HasAttributes
        //                  select
        //                      new StationInfo
        //                      {
        //                          StationIndex = item.Attribute("站序号")?.Value,
        //                          StationName = item.Attribute("站名定义")?.Value,
        //                          AxisX = item.Attribute("X轴号")?.Value,
        //                          AxisY = item.Attribute("Y轴号")?.Value,
        //                          AxisZ = item.Attribute("Z轴号")?.Value,
        //                          AxisU = item.Attribute("U轴号")?.Value,
        //                          AxisA = item.Attribute("A轴号")?.Value,
        //                          AxisB = item.Attribute("B轴号")?.Value,
        //                          AxisC = item.Attribute("C轴号")?.Value,
        //                          AxisD = item.Attribute("D轴号")?.Value
        //                      };
        //    var eth = from item in doc.Descendants("Eth")
        //              where item.HasAttributes
        //              select
        //                  new EthInfo
        //                  {
        //                      EthNum = item.Attribute("网口号")?.Value,
        //                      EthDefine = item.Attribute("网口定义")?.Value,
        //                      IpAddress = item.Attribute("对方IP地址")?.Value,
        //                      Port = item.Attribute("端口号")?.Value,
        //                      TimeOut = item.Attribute("超时时间")?.Value,
        //                      Command = item.Attribute("命令分隔")?.Value
        //                  };
        //    systemCfg = new SystemCfg
        //    {
        //        IoInput = ioInput.ToList(),
        //        IoOutput = ioOutput.ToList(),
        //        IoCardsList = cardList.ToList(),
        //        SysInput = systemIoIn.ToList(),
        //        SysOutput = systemIoOut.ToList(),
        //        MotionCardsList = motion.ToList(),
        //        EthInfos = eth.ToList(),
        //        StationInfos = station.ToList()
        //    };
        //    return systemCfg;
        //}

        public void SaveConfig<T>(T tCfg, string fileName)
        {
            switch (fileName)
            {
                case "systemCfg":
                    //SaveSystemCfg(tCfg, fileName);
                    break;
                case "systemCfgEx":
                case "dataType":
                case "dataTitle":
                case "dataInfo":
                case "dataShow":
                case "dataSave":
                    //SaveSystemCfgEx(tCfg, fileName);
                    break;
                case "point":
                    //SavePointCfg(tCfg, fileName);
                    break;
                case "systemParam":
                    SaveParamCfg(tCfg, fileName);
                    break;
            }

        }

        //private void SaveSystemCfgEx<T>(T tCfg, string fileName)
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory;
        //    string fileFullName = path + "systemCfgEx.xml";

        //    if (File.Exists(fileFullName))
        //    {
        //        XDocument doc = XDocument.Load(fileFullName);
        //        if (tCfg is OtherConfig)
        //        {
        //            var otherCfg = (OtherConfig)(object)tCfg;
        //            doc.Descendants("Light").Remove();
        //            doc.Descendants("Cylinder").Remove();
        //            doc.Descendants("RunMode").Remove();
        //            doc.Descendants("Server").Remove();

        //            SaveOtherConfig(doc, otherCfg, fileFullName);
        //        }
        //        else if (tCfg is AllDataClass)
        //        {
        //            var allData = (AllDataClass)(object)tCfg;
        //            doc.Descendants("Data").Elements().Where(item => item.Name == "Data").Remove();

        //            SaveAllDataConfig(doc, allData, fileFullName);
        //        }
        //        else if (tCfg is DataShowClass)
        //        {
        //            var allData = (DataShowClass)(object)tCfg;
        //            doc.Descendants("Data").Elements().Where(item => item.Name == "DataShow").Remove();

        //            SaveDataShowConfig(doc, allData, fileFullName);
        //        }
        //        else if (tCfg is DataSaveClass)
        //        {
        //            var allData = (DataSaveClass)(object)tCfg;
        //            doc.Descendants("Data").Elements().Where(item => item.Name == "DataSave").Remove();

        //            SaveDataSaveConfig(doc, allData, fileFullName);
        //        }
        //    }
        //    else
        //    {
        //        if (tCfg is OtherConfig)
        //        {
        //            XDocument doc = new XDocument(
        //                new XDeclaration("1.0", "utf-8", "yes"),
        //                new XElement("SystemCfg"));
        //            SaveOtherConfig(doc, (OtherConfig)(object)tCfg, fileFullName);
        //        }
        //        else if (tCfg is AllDataClass)
        //        {
        //            XDocument doc = new XDocument(
        //                new XDeclaration("1.0", "utf-8", "yes"),
        //                new XElement("SystemCfg"));
        //            SaveAllDataConfig(doc, (AllDataClass)(object)tCfg, fileFullName);
        //        }
        //        else if (tCfg is DataShowClass)
        //        {
        //            XDocument doc = new XDocument(
        //                new XDeclaration("1.0", "utf-8", "yes"),
        //                new XElement("SystemCfg"),
        //                new XElement("Data"));
        //            SaveDataShowConfig(doc, (DataShowClass)(object)tCfg, fileFullName);
        //        }
        //        else if (tCfg is DataSaveClass)
        //        {
        //            XDocument doc = new XDocument(
        //                new XDeclaration("1.0", "utf-8", "yes"),
        //                new XElement("SystemCfg"),
        //                new XElement("Data"));
        //            SaveDataSaveConfig(doc, (DataSaveClass)(object)tCfg, fileFullName);
        //        }
        //    }

        //}

        //private void SaveDataSaveConfig(XDocument doc, DataSaveClass allData, string fileFullName)
        //{
        //    List<string> saveType = new List<string> { "DB", "CSV", "INI" };

        //    XElement root = doc.Descendants("Data").First();

        //    XElement dataIn = new XElement("DataSave",
        //        new XAttribute("保存类型", saveType[allData.SelectSaveType] ?? ""),
        //        new XAttribute("Server", allData.ServiceAddress ?? ""),
        //        new XAttribute("Port", allData.Port ?? ""),
        //        new XAttribute("UserID", allData.User ?? ""),
        //        new XAttribute("Password", allData.Password ?? ""),
        //        new XAttribute("Database", allData.DataBase ?? ""),
        //        new XAttribute("TableName", allData.DataTable ?? ""),
        //        new XAttribute("保存路径", allData.SavePath ?? ""),
        //        new XAttribute("是否保存", allData.IsUpload.ToString()));
        //    foreach (var dataInfo in allData.DataIndexes)
        //    {
        //        XElement itemInfo = new XElement("Item",
        //            new XAttribute("名称", dataInfo.Name ?? ""),
        //            new XAttribute("数据索引", dataInfo.Index ?? ""));
        //        dataIn.Add(itemInfo);
        //    }


        //    root.Add(dataIn);
        //    doc.Save(fileFullName);
        //}

        //private void SaveDataShowConfig(XDocument doc, DataShowClass allData, string fileFullName)
        //{
        //    XElement root = doc.Descendants("Data").First();

        //    XElement dataIn = new XElement("DataShow",
        //        new XAttribute("是否保存", allData.IsUpload.ToString()));
        //    foreach (var dataInfo in allData.DataIndexes)
        //    {
        //        XElement itemInfo = new XElement("Item",
        //            new XAttribute("名称", dataInfo.Name ?? ""),
        //            new XAttribute("数据索引", dataInfo.Index ?? ""));
        //        dataIn.Add(itemInfo);
        //    }

        //    root.Add(dataIn);
        //    doc.Save(fileFullName);
        //}

        //private void SaveAllDataConfig(XDocument doc, AllDataClass allData, string fileFullName)
        //{
        //    XElement root = doc.Descendants("SystemCfg").First();
        //    XElement data = new XElement("Data");

        //    foreach (var item in allData.ComboxStrList)
        //    {
        //        XElement dataIn = new XElement("Data",
        //            new XAttribute("名称", allData.DataTitleDictionary[item].Name ?? ""),
        //            new XAttribute("版本", allData.DataTitleDictionary[item].Version ?? ""),
        //            new XAttribute("PDCA", allData.DataTitleDictionary[item].PdcaEnable.ToString()),
        //            new XAttribute("权限", allData.DataTitleDictionary[item].Authority.ToString()));
        //        foreach (var dataInfo in allData.DataInfoDictionary[item])
        //        {
        //            XElement itemInfo = new XElement("Item",
        //            new XAttribute("名称", dataInfo.Name ?? ""),
        //            new XAttribute("数据类型", dataInfo.DataType ?? ""),
        //            new XAttribute("数据索引", dataInfo.DataIndex ?? ""),
        //            new XAttribute("标准值", dataInfo.StandardValue ?? ""),
        //            new XAttribute("上限", dataInfo.UpperValue ?? ""),
        //            new XAttribute("下限", dataInfo.LowerValue ?? ""),
        //            new XAttribute("补偿值", dataInfo.OffsetValue ?? ""),
        //            new XAttribute("单位", dataInfo.Unit ?? ""));
        //            dataIn.Add(itemInfo);
        //        }

        //        data.Add(dataIn);
        //    }

        //    root.Add(data);
        //    doc.Save(fileFullName);
        //}

        //private void SaveOtherConfig(XDocument doc, OtherConfig otherCfg, string fileFullName)
        //{
        //    XElement root = doc.Descendants("SystemCfg").First();
        //    XElement lightElement = new XElement("Light",
        //        from item in otherCfg.LightInfos
        //        select
        //            new XElement("light",
        //                new XAttribute("名称", item.Name ?? ""),
        //                new XAttribute("类型", item.Type ?? ""),
        //                new XAttribute("通道", item.Aisle ?? ""),
        //                new XAttribute("通信索引", item.AisleIndex ?? "")
        //            ));
        //    XElement cylinderElement = new XElement("Cylinder",
        //        from item in otherCfg.CylinderInfos
        //        select
        //            new XElement("Cylinder",
        //                new XAttribute("名称", item.Name ?? ""),
        //                new XAttribute("翻译", item.EngName ?? ""),
        //                new XAttribute("类型", item.Type ?? ""),
        //                new XAttribute("伸出输出", item.ExtendOutput ?? ""),
        //                new XAttribute("缩回输出", item.RetractOutput ?? ""),
        //                new XAttribute("伸出输入", item.ExtendInput ?? ""),
        //                new XAttribute("缩回输入", item.RetractInput ?? ""),
        //                new XAttribute("伸出启用", item.ExtendEnable ?? ""),
        //                new XAttribute("缩回启用", item.RetractEnable ?? ""),
        //                new XAttribute("超时时间", item.TimeOut ?? "")
        //            ));
        //    XElement runModeElement = new XElement("RunMode",
        //        new XElement("Calib",
        //            from item in otherCfg.CalibInfos
        //            select
        //                new XElement("Item",
        //                    new XAttribute("名称", item.Name ?? ""),
        //                    new XAttribute("站位", item.StationName ?? ""),
        //                    new XAttribute("方法", item.MethodName ?? ""))),
        //        new XElement("GRR",
        //            from it in otherCfg.GrrInfos
        //            select
        //                new XElement("Item",
        //                    new XAttribute("名称", it.Name ?? ""),
        //                    new XAttribute("站位", it.StationName ?? ""),
        //                    new XAttribute("方法", it.MethodName ?? ""))
        //        ));
        //    XElement serverElement = new XElement("Server",
        //        from item in otherCfg.ServersInfos
        //        select
        //            new XElement("Server",
        //                new XAttribute("序号", item.Index ?? ""),
        //                new XAttribute("本地IP地址", item.IpAddress ?? ""),
        //                new XAttribute("监听端口", item.ListenPort ?? ""),
        //                new XAttribute("启用", item.Enable ?? "")
        //            ));
        //    root.Add(lightElement);
        //    root.Add(cylinderElement);
        //    root.Add(runModeElement);
        //    root.Add(serverElement);
        //    doc.Save(fileFullName);
        //}

        private void SaveParamCfg<T>(T tCfg, string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string fileFullName = path + fileName + ".xml";

            Parameters parameters = null;

            if (tCfg is Parameters)
            {
                parameters = (Parameters)(object)tCfg;

            }

            if (parameters != null)
            {
                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("SystemParam",
                        new XElement("ParamDsecribe",
                            new XElement("ParamDescribe",
                                new XAttribute("修改者", "Admin"),
                                new XAttribute("文件描述", "系统配置参数-运行用"))),
                        new XElement("Param",
                            from param in parameters.ParameterInfos
                            select
                                new XElement("Param",
                                    new XAttribute("键值", param.KeyValue),
                                    new XAttribute("当前值", param.CurrentValue ?? ""),
                                    new XAttribute("单位", param.Unit ?? ""),
                                    new XAttribute("参数描述", param.ParamDesc ?? ""),
                                    new XAttribute("翻译", param.EnglishDesc ?? ""),
                                    new XAttribute("最小值", param.MinValue ?? ""),
                                    new XAttribute("最大值", param.MaxValue ?? "")
                                ))));
                doc.Save(fileFullName);
            }
        }
        //private void SavePointCfg<T>(T tCfg, string fileName)
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory;
        //    string fileFullName = path + fileName + ".xml";

        //    List<StationPoint> stationPoints = null;

        //    if (tCfg is List<StationPoint>)
        //    {
        //        stationPoints = (List<StationPoint>)(object)tCfg;

        //    }

        //    XDocument doc = new XDocument(
        //        new XDeclaration("1.0", "utf-8", "yes"),
        //        new XElement("PointCfg",
        //            from station in stationPoints
        //            select
        //                new XElement(station.Name,
        //                from item in station.PointInfos
        //                select
        //                    new XElement("Point",
        //                        new XAttribute("index", item.Index),
        //                        new XAttribute("name", item.Name),
        //                        new XAttribute("x", item.XPos ?? ""),
        //                        new XAttribute("y", item.YPos ?? ""),
        //                        new XAttribute("z", item.ZPos ?? ""),
        //                        new XAttribute("u", item.UPos ?? ""),
        //                        new XAttribute("a", item.APos ?? ""),
        //                        new XAttribute("b", item.BPos ?? ""),
        //                        new XAttribute("c", item.CPos ?? ""),
        //                        new XAttribute("d", item.DPos ?? "")
        //    ))));
        //    doc.Save(fileFullName);
        //}

        //private void SaveSystemCfg<T>(T tCfg, string fileName)
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory;
        //    string fileFullName = path + fileName + ".xml";

        //    SystemCfg systemCfg = null;
        //    if (tCfg is SystemCfg)
        //    {
        //        systemCfg = (SystemCfg)(object)tCfg;

        //    }

        //    XDocument doc = new XDocument(
        //        new XDeclaration("1.0", "utf-8", "yes"),
        //        new XElement("SystemCfg",
        //            new XElement("IoCard",
        //                from item in systemCfg.IoCardsList
        //                select
        //                    new XElement("IoCard",
        //                        new XAttribute("卡序号", item.CardIndex),
        //                        new XAttribute("卡号", item.CardNum),
        //                        new XAttribute("卡类型", item.CardType))),
        //            new XElement("IoIn",
        //                from item in systemCfg.IoInput
        //                select
        //                    new XElement("IoIn",
        //                        new XAttribute("卡序号", item.CardIndex),
        //                        new XAttribute("点序号", item.PointIndex),
        //                        new XAttribute("点位名称", item.PointName),
        //                        new XAttribute("点位翻译", item.PointEngName))),
        //            new XElement("IoOut",
        //                from item in systemCfg.IoOutput
        //                select
        //                    new XElement("IoOut",
        //                        new XAttribute("卡序号", item.CardIndex),
        //                        new XAttribute("点序号", item.PointIndex),
        //                        new XAttribute("点位名称", item.PointName),
        //                        new XAttribute("点位翻译", item.PointEngName))),
        //            new XElement("IoSystemIn",
        //                from item in systemCfg.SysInput
        //                select
        //                    new XElement("IoSystemIn",
        //                        new XAttribute("功能描述", item.FuncDesc),
        //                        new XAttribute("卡序号", item.CardNum),
        //                        new XAttribute("点序号", item.PointIndex),
        //                        new XAttribute("有效电平", item.EffectiveLevel))),
        //            new XElement("IoSystemOut",
        //                from item in systemCfg.SysOutput
        //                select
        //                    new XElement("IoSystemOut",
        //                        new XAttribute("功能描述", item.FuncDesc),
        //                        new XAttribute("卡序号", item.CardNum),
        //                        new XAttribute("点序号", item.PointIndex),
        //                        new XAttribute("有效电平", item.EffectiveLevel))),
        //            new XElement("Motion",
        //                from item in systemCfg.MotionCardsList
        //                select
        //                    new XElement("Motion",
        //                        new XAttribute("序号", item.Index),
        //                        new XAttribute("卡类型", item.CardType),
        //                        new XAttribute("最小轴号", item.MinAxisNum),
        //                        new XAttribute("最大轴号", item.MaxAxisNum))),
        //            new XElement("Station",
        //                from item in systemCfg.StationInfos
        //                select
        //                    new XElement("Station",
        //                        new XAttribute("站序号", item.StationIndex),
        //                        new XAttribute("站名定义", item.StationName),
        //                        new XAttribute("X轴号", item.AxisX ?? ""),
        //                        new XAttribute("Y轴号", item.AxisY ?? ""),
        //                        new XAttribute("Z轴号", item.AxisZ ?? ""),
        //                        new XAttribute("U轴号", item.AxisU ?? ""),
        //                        new XAttribute("A轴号", item.AxisA ?? ""),
        //                        new XAttribute("B轴号", item.AxisB ?? ""),
        //                        new XAttribute("C轴号", item.AxisC ?? ""),
        //                        new XAttribute("D轴号", item.AxisD ?? ""))),
        //            new XElement("Eth",
        //                from item in systemCfg.EthInfos
        //                select
        //                    new XElement("Eth",
        //                        new XAttribute("网口号", item.EthNum),
        //                        new XAttribute("网口定义", item.EthDefine),
        //                        new XAttribute("对方IP地址", item.IpAddress),
        //                        new XAttribute("端口号", item.Port),
        //                        new XAttribute("超时时间", item.TimeOut),
        //                        new XAttribute("命令分隔", item.Command)))
        //    ));

        //    doc.Save(fileFullName);
        //}

        public void SaveAsConfig<T>(T tCfg)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                DefaultExt = ".xml",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            var result = sfd.ShowDialog();
            if (result == true)
            {
                var file = sfd.FileName;
                int starNum = file.LastIndexOf("\\", StringComparison.Ordinal) + 1;
                int nums = file.LastIndexOf(".", StringComparison.Ordinal) - file.LastIndexOf("\\", StringComparison.Ordinal) - 1;
                file = file.Substring(starNum, nums);

                //if (tCfg is SystemCfg)
                //{
                //    SaveSystemCfg(tCfg, file);
                //}
                //else if (tCfg is List<StationPoint>)
                //{
                //    SavePointCfg(tCfg, file);
                //}
                //else 
                if (tCfg is Parameters)
                {
                    SaveParamCfg(tCfg, file);
                }
                //else if (tCfg is OtherConfig)
                //{
                //    XDocument doc = new XDocument(
                //        new XDeclaration("1.0", "utf-8", "yes"),
                //        new XElement("SystemCfg"));
                //    SaveOtherConfig(doc, (OtherConfig)(object)tCfg, file + ".xml");
                //}
                //else if (tCfg is AllDataClass)
                //{
                //    XDocument doc = new XDocument(
                //        new XDeclaration("1.0", "utf-8", "yes"),
                //        new XElement("SystemCfg"));
                //    SaveAllDataConfig(doc, (AllDataClass)(object)tCfg, file + ".xml");
                //}
                //else if (tCfg is DataShowClass)
                //{
                //    XDocument doc = new XDocument(
                //        new XDeclaration("1.0", "utf-8", "yes"),
                //        new XElement("SystemCfg",
                //        new XElement("Data")));
                //    SaveDataShowConfig(doc, (DataShowClass)(object)tCfg, file + ".xml");
                //}
                //else if (tCfg is DataSaveClass)
                //{
                //    XDocument doc = new XDocument(
                //        new XDeclaration("1.0", "utf-8", "yes"),
                //        new XElement("SystemCfg",
                //        new XElement("Data")));
                //    SaveDataSaveConfig(doc, (DataSaveClass)(object)tCfg, file + ".xml");
                //}
                MessageBox.Show("保存成功");
            }
            else
            {
                MessageBox.Show("未指定xml文件");
            }
        }

        //private T LoadPointConfig<T>() where T : List<StationPoint>
        //{
        //    string file = AppDomain.CurrentDomain.BaseDirectory + "point.xml";
        //    List<StationPoint> stationPoints;

        //    if (!File.Exists(file))
        //    {
        //        string sysFile = AppDomain.CurrentDomain.BaseDirectory + "systemCfg.xml";

        //        var systemCfg = LoadSystemConfig<SystemCfg>();

        //        stationPoints = new List<StationPoint>();

        //        foreach (var station in systemCfg.StationInfos)
        //        {
        //            stationPoints.Add(new StationPoint { Name = station.StationName, PointInfos = new List<PointInfo>() });
        //        }
        //        return stationPoints as T;
        //    }
        //    else
        //    {
        //        stationPoints = LoadPointFromFile<T>(file);

        //        return stationPoints as T;
        //    }
        //}

        //private List<StationPoint> LoadPointFromFile<T>(string file) where T : List<StationPoint>
        //{
        //    var stationPoints = new List<StationPoint>();
        //    XDocument doc = XDocument.Load(file);
        //    foreach (var item in doc.Elements().Elements())
        //    {
        //        string name = item.Name.ToString();
        //        var station = from points in item.Descendants("Point")
        //                      where points.HasAttributes
        //                      select
        //                          new PointInfo
        //                          {
        //                              Index = points.Attribute("index")?.Value,
        //                              Name = points.Attribute("name")?.Value,
        //                              XPos = points.Attribute("x")?.Value,
        //                              YPos = points.Attribute("y")?.Value,
        //                              ZPos = points.Attribute("z")?.Value,
        //                              UPos = points.Attribute("u")?.Value,
        //                              APos = points.Attribute("a")?.Value,
        //                              BPos = points.Attribute("b")?.Value,
        //                              CPos = points.Attribute("c")?.Value,
        //                              DPos = points.Attribute("d")?.Value
        //                          };
        //        var stationPoint = new StationPoint
        //        {
        //            Name = name,
        //            PointInfos = station.ToList()
        //        };
        //        stationPoints.Add(stationPoint);
        //    }

        //    return stationPoints;
        //}

        public T LoadConfigFromFile<T>(string config)
        {
            object tCfg = new object();
            OpenFileDialog ofd = new OpenFileDialog
            {
                DefaultExt = ".xml",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            var result = ofd.ShowDialog();

            if (result == true)
            {
                var file = ofd.FileName;
                try
                {
                    switch (config)
                    {
                        case "system":
                            //tCfg = (T)(object)LoadSystemFromFile<SystemCfg>(file);
                            break;
                        case "point":
                            //tCfg = (T)(object)LoadPointFromFile<List<StationPoint>>(file);
                            break;
                        case "systemParam":
                            tCfg = (T)(object)LoadParamFromFile<Parameters>(file);
                            break;
                        case "otherConfig":
                            //tCfg = (T)(object)LoadOtherConfigFromFile<OtherConfig>(file);
                            break;
                        case "dataClass":
                            //return (T)(object)LoadAllDataFromFile<AllDataClass>(file);
                        case "dataClassTitle":
                            //return (T)(object)new DataClassTitle();
                        case "dataInfo":
                            //return (T)(object)new ObservableCollection<DataInfo>();
                        case "dataShow":
                            //return (T)(object)LoadDataShowFromFile<DataShowClass>(file);
                        case "dataSave":
                            break;
                            //return (T)(object)LoadDataSaveFromFile<DataSaveClass>(file);
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show("文件错误", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    return (T)tCfg;
                }
                MessageBox.Show("加载成功");
                return (T)tCfg;
            }
            else
            {
                MessageBox.Show("请选择配置文件");
                return (T)tCfg;
            }

        }
    }
}
