using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using AutoBuildConfig.ViewModel;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AutoBuildConfig.Model
{
    public class XMLBuildConfig : IBuildConfig
    {
        public T LoadConfig<T>(string config)
        {
            object tCfg = new object();

            switch (config)
            {
                case "system":
                    return (T)(object)LoadSystemConfig<SystemCfg>();
                case "point":
                    return (T)(object)LoadPointConfig<List<StationPoint>>();
                    //case "systemParam":
                    //    return (T)(object)LoadParamConfig<Parameters>();
                    //case "otherConfig":
                    //    return (T)(object)LoadOtherConfig<OtherConfig>();
                    //case "dataClass":
                    //    return (T)(object)LoadDataClass<AllDataClass>();
                    //case "dataClassTitle":
                    //    return (T)(object)LoadDataClassTitle<DataClassTitle>();
                    //case "dataInfo":
                    //    return (T)(object)LoadDataClassInfo<ObservableCollection<DataInfo>>();
                    //case "dataShow":
                    //    return (T)(object)LoadDataShow<DataShowClass>();
                    //case "dataSave":
                    //    return (T)(object)LoadDataSave<DataSaveClass>();
            }
            return (T)tCfg;
        }

        private T LoadSystemConfig<T>() where T : SystemCfg
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "systemCfg.xml";
            SystemCfg systemCfg;

            if (!File.Exists(file))
            {
                systemCfg = new SystemCfg
                {
                    IoInput = new List<IoInputPoint>(),
                    IoOutput = new List<IoOutputPoint>(),
                    IoCardsList = new List<IoCardInfo>(),
                    SysInput = new List<SysInputPoint>(),
                    SysOutput = new List<SysOutputPoint>(),
                    MotionCardsList = new List<MotionCard>(),
                    EthInfos = new List<EthInfo>(),
                    StationInfos = new List<StationInfo>()
                };
                return systemCfg as T;
            }
            else
            {
                systemCfg = LoadSystemFromFile<T>(file);

                return systemCfg as T;
            }
        }

        private SystemCfg LoadSystemFromFile<T>(string file) where T : SystemCfg
        {
            SystemCfg systemCfg;
            XDocument doc = XDocument.Load(file);
            var cardList = from item in doc.Descendants("IoCard")
                           where item.HasAttributes
                           select
                               new IoCardInfo
                               {
                                   CardIndex = item.Attribute("卡序号")?.Value,
                                   CardNum = item.Attribute("卡号")?.Value,
                                   CardType = item.Attribute("卡类型")?.Value
                               };
            var ioInput = from item in doc.Descendants("IoIn")
                          where item.HasAttributes
                          select
                              new IoInputPoint
                              {
                                  CardIndex = item.Attribute("卡序号")?.Value,
                                  PointIndex = item.Attribute("点序号")?.Value,
                                  PointName = item.Attribute("点位名称")?.Value,
                                  PointEngName = item.Attribute("点位翻译")?.Value
                              };
            var ioOutput = from item in doc.Descendants("IoOut")
                           where item.HasAttributes
                           select
                               new IoOutputPoint
                               {
                                   CardIndex = item.Attribute("卡序号")?.Value,
                                   PointIndex = item.Attribute("点序号")?.Value,
                                   PointName = item.Attribute("点位名称")?.Value,
                                   PointEngName = item.Attribute("点位翻译")?.Value
                               };
            var systemIoIn = from item in doc.Descendants("IoSystemIn")
                             where item.HasAttributes
                             select
                                 new SysInputPoint
                                 {
                                     FuncDesc = item.Attribute("功能描述")?.Value,
                                     PointIndex = item.Attribute("点序号")?.Value,
                                     CardNum = item.Attribute("卡序号")?.Value,
                                     EffectiveLevel = item.Attribute("有效电平")?.Value
                                 };
            var systemIoOut = from item in doc.Descendants("IoSystemOut")
                              where item.HasAttributes
                              select
                                  new SysOutputPoint
                                  {
                                      FuncDesc = item.Attribute("功能描述")?.Value,
                                      PointIndex = item.Attribute("点序号")?.Value,
                                      CardNum = item.Attribute("卡序号")?.Value,
                                      EffectiveLevel = item.Attribute("有效电平")?.Value
                                  };
            var motion = from item in doc.Descendants("Motion")
                         where item.HasAttributes
                         select
                             new MotionCard
                             {
                                 CardType = item.Attribute("卡类型")?.Value,
                                 Index = item.Attribute("序号")?.Value,
                                 MinAxisNum = item.Attribute("最小轴号")?.Value,
                                 MaxAxisNum = item.Attribute("最大轴号")?.Value
                             };
            var station = from item in doc.Descendants("Station")
                          where item.HasAttributes
                          select
                              new StationInfo
                              {
                                  StationIndex = item.Attribute("站序号")?.Value,
                                  StationName = item.Attribute("站名定义")?.Value,
                                  AxisX = item.Attribute("X轴号")?.Value,
                                  AxisY = item.Attribute("Y轴号")?.Value,
                                  AxisZ = item.Attribute("Z轴号")?.Value,
                                  AxisU = item.Attribute("U轴号")?.Value,
                                  AxisA = item.Attribute("A轴号")?.Value,
                                  AxisB = item.Attribute("B轴号")?.Value,
                                  AxisC = item.Attribute("C轴号")?.Value,
                                  AxisD = item.Attribute("D轴号")?.Value
                              };
            var eth = from item in doc.Descendants("Eth")
                      where item.HasAttributes
                      select
                          new EthInfo
                          {
                              EthNum = item.Attribute("网口号")?.Value,
                              EthDefine = item.Attribute("网口定义")?.Value,
                              IpAddress = item.Attribute("对方IP地址")?.Value,
                              Port = item.Attribute("端口号")?.Value,
                              TimeOut = item.Attribute("超时时间")?.Value,
                              Command = item.Attribute("命令分隔")?.Value
                          };
            systemCfg = new SystemCfg
            {
                IoInput = ioInput.ToList(),
                IoOutput = ioOutput.ToList(),
                IoCardsList = cardList.ToList(),
                SysInput = systemIoIn.ToList(),
                SysOutput = systemIoOut.ToList(),
                MotionCardsList = motion.ToList(),
                EthInfos = eth.ToList(),
                StationInfos = station.ToList()
            };
            return systemCfg;
        }

        public void SaveConfig<T>(T tCfg, string fileName)
        {
            switch (fileName)
            {
                case "systemCfg":
                    SaveSystemCfg(tCfg, fileName);
                    break;
                case "systemCfgEx":
                    break;
                case "point":
                    SavePointCfg(tCfg, fileName);
                    break;
                case "systemParam":
                    break;
            }

        }

        private void SavePointCfg<T>(T tCfg, string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string fileFullName = path + fileName + ".xml";

            List<StationPoint> stationPoints = null;

            if (tCfg is List<StationPoint>)
            {
                stationPoints = (List<StationPoint>)(object)tCfg;

            }

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("PointCfg",
                    from station in stationPoints
                    select
                        new XElement(station.Name,
                        from item in station.PointInfos
                        select
                            new XElement("Point",
                                new XAttribute("index", item.Index),
                                new XAttribute("name", item.Name),
                                new XAttribute("x", item.XPos ?? ""),
                                new XAttribute("y", item.YPos ?? ""),
                                new XAttribute("z", item.ZPos ?? ""),
                                new XAttribute("u", item.UPos ?? ""),
                                new XAttribute("a", item.APos ?? ""),
                                new XAttribute("b", item.BPos ?? ""),
                                new XAttribute("c", item.CPos ?? ""),
                                new XAttribute("d", item.DPos ?? "")
            ))));
            doc.Save(fileFullName);
        }

        private void SaveSystemCfg<T>(T tCfg, string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string fileFullName = path + fileName + ".xml";

            SystemCfg systemCfg = null;
            if (tCfg is SystemCfg)
            {
                systemCfg = (SystemCfg)(object)tCfg;

            }

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("SystemCfg",
                    new XElement("IoCard",
                        from item in systemCfg.IoCardsList
                        select
                            new XElement("IoCard",
                                new XAttribute("卡序号", item.CardIndex),
                                new XAttribute("卡号", item.CardNum),
                                new XAttribute("卡类型", item.CardType))),
                    new XElement("IoIn",
                        from item in systemCfg.IoInput
                        select
                            new XElement("IoIn",
                                new XAttribute("卡序号", item.CardIndex),
                                new XAttribute("点序号", item.PointIndex),
                                new XAttribute("点位名称", item.PointName),
                                new XAttribute("点位翻译", item.PointEngName))),
                    new XElement("IoOut",
                        from item in systemCfg.IoOutput
                        select
                            new XElement("IoOut",
                                new XAttribute("卡序号", item.CardIndex),
                                new XAttribute("点序号", item.PointIndex),
                                new XAttribute("点位名称", item.PointName),
                                new XAttribute("点位翻译", item.PointEngName))),
                    new XElement("IoSystemIn",
                        from item in systemCfg.SysInput
                        select
                            new XElement("IoSystemIn",
                                new XAttribute("功能描述", item.FuncDesc),
                                new XAttribute("卡序号", item.CardNum),
                                new XAttribute("点序号", item.PointIndex),
                                new XAttribute("有效电平", item.EffectiveLevel))),
                    new XElement("IoSystemOut",
                        from item in systemCfg.SysOutput
                        select
                            new XElement("IoSystemOut",
                                new XAttribute("功能描述", item.FuncDesc),
                                new XAttribute("卡序号", item.CardNum),
                                new XAttribute("点序号", item.PointIndex),
                                new XAttribute("有效电平", item.EffectiveLevel))),
                    new XElement("Motion",
                        from item in systemCfg.MotionCardsList
                        select
                            new XElement("Motion",
                                new XAttribute("序号", item.Index),
                                new XAttribute("卡类型", item.CardType),
                                new XAttribute("最小轴号", item.MinAxisNum),
                                new XAttribute("最大轴号", item.MaxAxisNum))),
                    new XElement("Station",
                        from item in systemCfg.StationInfos
                        select
                            new XElement("Station",
                                new XAttribute("站序号", item.StationIndex),
                                new XAttribute("站名定义", item.StationName),
                                new XAttribute("X轴号", item.AxisX ?? ""),
                                new XAttribute("Y轴号", item.AxisY ?? ""),
                                new XAttribute("Z轴号", item.AxisZ ?? ""),
                                new XAttribute("U轴号", item.AxisU ?? ""),
                                new XAttribute("A轴号", item.AxisA ?? ""),
                                new XAttribute("B轴号", item.AxisB ?? ""),
                                new XAttribute("C轴号", item.AxisC ?? ""),
                                new XAttribute("D轴号", item.AxisD ?? ""))),
                    new XElement("Eth",
                        from item in systemCfg.EthInfos
                        select
                            new XElement("Eth",
                                new XAttribute("网口号", item.EthNum),
                                new XAttribute("网口定义", item.EthDefine),
                                new XAttribute("对方IP地址", item.IpAddress),
                                new XAttribute("端口号", item.Port),
                                new XAttribute("超时时间", item.TimeOut),
                                new XAttribute("命令分隔", item.Command)))
            ));

            doc.Save(fileFullName);
        }

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

                if (tCfg is SystemCfg)
                {
                    SaveSystemCfg(tCfg, file);
                }
                else if (tCfg is List<StationPoint>)
                {
                    SavePointCfg(tCfg, file);
                }
                MessageBox.Show("保存成功");
            }
            else
            {
                MessageBox.Show("未指定xml文件");
            }
        }

        private T LoadPointConfig<T>() where T : List<StationPoint>
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "point.xml";
            List<StationPoint> stationPoints;

            if (!File.Exists(file))
            {
                string sysFile = AppDomain.CurrentDomain.BaseDirectory + "systemCfg.xml";

                var systemCfg = LoadSystemConfig<SystemCfg>();

                stationPoints = new List<StationPoint>();

                foreach (var station in systemCfg.StationInfos)
                {
                    stationPoints.Add(new StationPoint { Name = station.StationName, PointInfos = new List<PointInfo>() });
                }
                return stationPoints as T;
            }
            else
            {
                stationPoints = LoadPointFromFile<T>(file);

                return stationPoints as T;
            }
        }

        private List<StationPoint> LoadPointFromFile<T>(string file) where T : List<StationPoint>
        {
            var stationPoints = new List<StationPoint>();
            XDocument doc = XDocument.Load(file);
            foreach (var item in doc.Elements().Elements())
            {
                string name = item.Name.ToString();
                var station = from points in item.Descendants("Point")
                              where points.HasAttributes
                              select
                                  new PointInfo
                                  {
                                      Index = points.Attribute("index")?.Value,
                                      Name = points.Attribute("name")?.Value,
                                      XPos = points.Attribute("x")?.Value,
                                      YPos = points.Attribute("y")?.Value,
                                      ZPos = points.Attribute("z")?.Value,
                                      UPos = points.Attribute("u")?.Value,
                                      APos = points.Attribute("a")?.Value,
                                      BPos = points.Attribute("b")?.Value,
                                      CPos = points.Attribute("c")?.Value,
                                      DPos = points.Attribute("d")?.Value
                                  };
                var stationPoint = new StationPoint
                {
                    Name = name,
                    PointInfos = station.ToList()
                };
                stationPoints.Add(stationPoint);
            }

            return stationPoints;
        }

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
                            tCfg = (T)(object)LoadSystemFromFile<SystemCfg>(file);
                            break;
                        case "point":
                            tCfg = (T)(object)LoadPointFromFile<List<StationPoint>>(file);
                            break;
                            //case "systemParam":
                            //    return (T)(object)LoadParamConfig<Parameters>();
                            //case "otherConfig":
                            //    return (T)(object)LoadOtherConfig<OtherConfig>();
                            //case "dataClass":
                            //    return (T)(object)LoadDataClass<AllDataClass>();
                            //case "dataClassTitle":
                            //    return (T)(object)LoadDataClassTitle<DataClassTitle>();
                            //case "dataInfo":
                            //    return (T)(object)LoadDataClassInfo<ObservableCollection<DataInfo>>();
                            //case "dataShow":
                            //    return (T)(object)LoadDataShow<DataShowClass>();
                            //case "dataSave":
                            //    return (T)(object)LoadDataSave<DataSaveClass>();
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show("文件错误" + e, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    return (T)tCfg;
                }
                MessageBox.Show("加载成功");
                return (T)tCfg;
            }
            else
            {
                MessageBox.Show("请选择json文件");
                return (T)tCfg;
            }

        }
    }
}
