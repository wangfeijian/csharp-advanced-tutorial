using System;
using System.Collections.Generic;
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
    public class JsonBuildConfig :IBuildConfig
    {
        public void SaveConfig<T>(T tCfg, string fileName)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + fileName;
            string value = JsonConvert.SerializeObject(tCfg);
            File.WriteAllText(file, value);

            MessageBox.Show("保存成功！", "提示");
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
                MessageBox.Show("未指定json文件");
            }
        }

        public T LoadConfigFromFile<T>()
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
                    MessageBox.Show("文件错误" + e, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    return (T)tCfg;
                }
                MessageBox.Show("加载成功");
                return (T)tCfg;
            }
            else
            {
                MessageBox.Show("请选择json文件");
                return (T) tCfg;
            }
        }

        public T LoadConfig<T>(string config)
        {
            Object tCfg = new object();

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

            return (T) tCfg;
        }

        private T LoadSystemConfig<T>() where T:SystemCfg
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

        private T LoadPointConfig<T>() where T:List<StationPoint>
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
    }
}
