﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutoBuildConfig.ViewModel;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AutoBuildConfig.Model
{
    public class JsonBuildConfig : IBuildConfig
    {
        public void SaveConfig<T>(T tCfg, string fileName)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + fileName+".json";
            string value = JsonConvert.SerializeObject(tCfg);
            File.WriteAllText(file, value);
        }

        public void SaveAsConfig<T>(T tCfg)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                DefaultExt = ".json",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            var result = sfd.ShowDialog();
            if (result == true)
            {
                var file = sfd.FileName;
                string value = JsonConvert.SerializeObject(tCfg);

                File.WriteAllText(file, value);
                MessageBox.Show("保存成功");
            }
            else
            {
                MessageBox.Show("未指定配置文件");
            }
        }

        public T LoadConfigFromFile<T>(string config)
        {
            Object tCfg = new object();
            OpenFileDialog ofd = new OpenFileDialog
            {
                DefaultExt = ".json",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            var result = ofd.ShowDialog();

            if (result == true)
            {
                var file = ofd.FileName;
                try
                {
                    tCfg = JsonConvert.DeserializeObject<T>(File.ReadAllText(file));

                }
                catch (Exception e)
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

        public T LoadConfig<T>(string config)
        {
            object tCfg = new object();

            switch (config)
            {
                case "system":
                    return (T)(object)LoadSystemConfig<SystemCfg>();
                case "point":
                    return (T)(object)LoadPointConfig<List<StationPoint>>();
                case "systemParam":
                    return (T)(object)LoadParamConfig<Parameters>();
                case "otherConfig":
                    return (T)(object)LoadOtherConfig<OtherConfig>();
                case "dataClass":
                    return (T)(object)LoadDataClass<AllDataClass>();
                case "dataClassTitle":
                    return (T)(object)LoadDataClassTitle<DataClassTitle>();
                case "dataInfo":
                    return (T)(object)LoadDataClassInfo<ObservableCollection<DataInfo>>();
                case "dataShow":
                    return (T)(object)LoadDataShow<DataShowClass>();
                case "dataSave":
                    return (T)(object)LoadDataSave<DataSaveClass>();
            }
            #region IfElse表示

            /*
            if (config.Equals("system"))
            {
                return (T)(object)LoadSystemConfig<SystemCfg>();
                // 将具体类转为T类型时，需要先将具体类转成object，再转换成T类型
            }
            else if (config.Equals("point"))
            {
                return (T)(object)LoadPointConfig<List<StationPoint>>();
            }
            else if (config.Equals("systemParam"))
            {
                return (T)(object)LoadParamConfig<Parameters>();
            }
            else if (config.Equals("otherConfig"))
            {
                return (T)(object)LoadOtherConfig<OtherConfig>();
            }
            else if(config.Equals("dataClass"))
            {
                return (T)(object)LoadDataClass<AllDataClass>();
            }
            else if (config.Equals("dataClassTitle"))
            {
                return (T)(object)LoadDataClassTitle<DataClassTitle>();
            }
            else if (config.Equals("dataInfo"))
            {
                return (T)(object)LoadDataClassInfo<ObservableCollection<DataInfo>>();
            }
            else if (config.Equals("dataShow"))
            {
                return (T)(object)LoadDataShow<DataShowClass>();
            }
            else if (config.Equals("dataSave"))
            {
                return (T)(object)LoadDataSave<DataSaveClass>();
            }
            */

            #endregion
            return (T)tCfg;
        }

        private T LoadSystemConfig<T>() where T : SystemCfg
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "systemCfg.json";
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
                systemCfg = JsonConvert.DeserializeObject<SystemCfg>(File.ReadAllText(file));
                return systemCfg as T;
            }
        }

        private T LoadPointConfig<T>() where T : List<StationPoint>
        {
            string filePoint = AppDomain.CurrentDomain.BaseDirectory + "point.json";
            List<StationPoint> stationPoints;

            if (!File.Exists(filePoint))
            {
                string sysFile = AppDomain.CurrentDomain.BaseDirectory + "systemCfg.json";

                var systemCfg = JsonConvert.DeserializeObject<SystemCfg>(File.ReadAllText(sysFile));

                stationPoints = new List<StationPoint>();

                foreach (var station in systemCfg.StationInfos)
                {
                    stationPoints.Add(new StationPoint { Name = station.StationName, PointInfos = new List<PointInfo>() });
                }
                return stationPoints as T;
            }
            else
            {
                stationPoints = JsonConvert.DeserializeObject<List<StationPoint>>(File.ReadAllText(filePoint));
                return stationPoints as T;
            }
        }

        private T LoadParamConfig<T>() where T : Parameters
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "systemParam.json";
            Parameters paramCfg;

            if (!File.Exists(file))
            {
                paramCfg = new Parameters
                {
                    ParameterInfos = new List<ParameterInfo>()
                };
                return paramCfg as T;
            }
            else
            {
                paramCfg = JsonConvert.DeserializeObject<Parameters>(File.ReadAllText(file));
                return paramCfg as T;
            }
        }

        private T LoadOtherConfig<T>() where T : OtherConfig
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "systemCfgEx.json";
            OtherConfig otherConfig;

            if (!File.Exists(file))
            {
                otherConfig = new OtherConfig
                {
                    CylinderInfos = new List<CylinderInfo>(),
                    LightInfos = new List<LightInfo>(),
                    GrrInfos = new List<ReflectInfo>(),
                    CalibInfos = new List<ReflectInfo>(),
                    ServersInfos = new List<ServersInfo>()
                };
                return otherConfig as T;
            }
            else
            {
                otherConfig = JsonConvert.DeserializeObject<OtherConfig>(File.ReadAllText(file));
                return otherConfig as T;
            }
        }

        private T LoadDataClass<T>() where T : AllDataClass
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "dataType.json";
            AllDataClass allDataClass;

            if (!File.Exists(file))
            {
                allDataClass = new AllDataClass
                {
                    ComboxStrList = new ObservableCollection<string>(),
                    DataTitleDictionary = new Dictionary<string, DataClassTitle>(),
                    DataInfoDictionary = new Dictionary<string, ObservableCollection<DataInfo>>(),
                };
                return allDataClass as T;
            }
            else
            {
                allDataClass = JsonConvert.DeserializeObject<AllDataClass>(File.ReadAllText(file));
                return allDataClass as T;
            }
        }

        private T LoadDataClassTitle<T>() where T : DataClassTitle
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "dataTitle.json";
            DataClassTitle dataClassTitle;

            if (!File.Exists(file))
            {
                dataClassTitle = new DataClassTitle();
                return dataClassTitle as T;
            }
            else
            {
                dataClassTitle = JsonConvert.DeserializeObject<DataClassTitle>(File.ReadAllText(file));
                return dataClassTitle as T;
            }
        }

        private T LoadDataClassInfo<T>() where T : ObservableCollection<DataInfo>
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "dataInfo.json";
            ObservableCollection<DataInfo> dataInfos;

            if (!File.Exists(file))
            {
                dataInfos = new ObservableCollection<DataInfo>();
                return dataInfos as T;
            }
            else
            {
                dataInfos = JsonConvert.DeserializeObject<ObservableCollection<DataInfo>>(File.ReadAllText(file));
                return dataInfos as T;
            }
        }
        private T LoadDataShow<T>() where T : DataShowClass
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "dataShow.json";
            DataShowClass dataShow;

            if (!File.Exists(file))
            {
                dataShow = new DataShowClass
                {
                    DataIndexes = new ObservableCollection<DataIndex>()
                };
                return dataShow as T;
            }
            else
            {
                dataShow = JsonConvert.DeserializeObject<DataShowClass>(File.ReadAllText(file));
                return dataShow as T;
            }
        }

        private T LoadDataSave<T>() where T : DataSaveClass
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "dataSave.json";
            DataSaveClass dataSave;

            if (!File.Exists(file))
            {
                dataSave = new DataSaveClass
                {
                    DataIndexes = new ObservableCollection<DataIndex>()
                };
                return dataSave as T;
            }
            else
            {
                dataSave = JsonConvert.DeserializeObject<DataSaveClass>(File.ReadAllText(file));
                return dataSave as T;
            }
        }
    }
}
