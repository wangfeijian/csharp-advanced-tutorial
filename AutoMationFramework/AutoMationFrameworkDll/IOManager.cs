/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    System IO manager class                  *
*********************************************************************/
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonTools.Manager;
using CommonTools.Model;
using CommonTools.Servers;
using CommonTools.Tools;
using MotionIO;

namespace AutoMationFrameworkDll
{
    public class IoManager : SingletonPattern<IoManager>
    {
        public SystemCfg SystemConfig { get; set; }

        /// <summary>
        /// IO卡类指针向量
        /// </summary>
        public List<IoControl> ListCard = new List<IoControl>();

        /// <summary>
        /// IO输入点名称与点位映射
        /// </summary>
        public Dictionary<string, long> DicIn = new Dictionary<string, long>();

        /// <summary>
        /// IO输出点名称与点位映射
        /// </summary>
        public Dictionary<string, long> DicOut = new Dictionary<string, long>();

        /// <summary>
        /// 输入IO点名称翻译
        /// </summary>
        public Dictionary<string, string> DicInTranslate = new Dictionary<string, string>();

        /// <summary>
        /// 输出IO点名称翻译
        /// </summary>
        public Dictionary<string, string> DicOutTranslate = new Dictionary<string, string>();

        /// <summary>
        /// 根据板卡名字动态加载对应的板卡类
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="nIndex"></param>
        /// <param name="nCardNo"></param>
        private void AddCard(string strName, int nIndex, int nCardNo)
        {
            string str = LocationServices.GetLang("NotFoundIOCard");

            Type type = Assembly.GetAssembly(typeof(IoControl)).GetType("MotionIO.IoControl" + strName);
            if (type == null)
                throw new Exception(string.Format(str, strName));
            object[] objArray = {
                 nIndex,
                 nCardNo
            };
            ListCard.Add(Activator.CreateInstance(type, objArray) as IoControl);
        }

        /// <summary>
        /// 读取系统配置文件里的IO板卡信息
        /// </summary>
        /// <param name="fileDir">文件目录</param>
        /// <param name="bulidConfig">配置的读取文件对象</param>
        public void ReadCfgFromFile(string fileDir, IBuildConfig bulidConfig)
        {
            ListCard.Clear();

            string fileName = AppDomain.CurrentDomain.BaseDirectory + "Config\\" + fileDir + "\\systemCfg";
            SystemConfig = bulidConfig.LoadConfig<SystemCfg>(fileName);
            foreach (var ioCardInfo in SystemConfig.IoCardsList)
            {
                string cardIndex = ioCardInfo.CardIndex;
                string cardNum = ioCardInfo.CardNum;
                string cardType = ioCardInfo.CardType.Trim();
                AddCard(cardType, Convert.ToInt32(cardIndex), Convert.ToInt32(cardNum));
            }

            ReadIoFromCfg(true);
            ReadIoFromCfg(false);
        }

        /// <summary>
        /// 获取配置文件中的IO点位
        /// </summary>
        /// <param name="flag">true为io输入，false为io输出</param>
        private void ReadIoFromCfg(bool flag)
        {
            if (flag)
            {
                DicIn.Clear();
                DicInTranslate.Clear();
                foreach (var ioInputPoint in SystemConfig.IoInput)
                {
                    string cardIndex = ioInputPoint.CardIndex;
                    string pointIndex = ioInputPoint.PointIndex;
                    string pointName = ioInputPoint.PointName;
                    string pointEngName = ioInputPoint.PointEngName;
                    if (string.IsNullOrEmpty(pointEngName))
                    {
                        pointEngName = pointName;
                    }

                    int card = Convert.ToInt32(cardIndex);
                    int point = Convert.ToInt32(pointIndex);

                    if (pointName != string.Empty)
                    {
                        try
                        {
                            DicIn.Add(pointName, card << 8 | point);
                            DicInTranslate.Add(pointName, pointEngName);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                    }

                    if (card - 1 < ListCard.Count)
                    {
                        string[] pointStrName = ListCard.ElementAt(card - 1).StrArrayIn;
                        if (point - 1 < pointStrName.Length)
                        {
                            pointStrName[point - 1] = pointName;
                        }
                        else
                        {
                            MessageBox.Show(string.Format("配置文件Io点配置出错,卡号{0}, IO点{1}", card, point));
                        }
                    }
                }
            }
            else
            {
                DicOut.Clear();
                DicOutTranslate.Clear();
                foreach (var ioOutputPoint in SystemConfig.IoOutput)
                {
                    string cardIndex = ioOutputPoint.CardIndex;
                    string pointIndex = ioOutputPoint.PointIndex;
                    string pointName = ioOutputPoint.PointName;
                    string pointEngName = ioOutputPoint.PointEngName;
                    if (string.IsNullOrEmpty(pointEngName))
                    {
                        pointEngName = pointName;
                    }

                    int card = Convert.ToInt32(cardIndex);
                    int point = Convert.ToInt32(pointIndex);

                    if (pointName != string.Empty)
                    {
                        try
                        {
                            DicOut.Add(pointName, card << 8 | point);
                            DicOutTranslate.Add(pointName, pointEngName);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                    }

                    if (card - 1 < ListCard.Count)
                    {
                        string[] pointStrName = ListCard.ElementAt(card - 1).StrArrayOut;
                        if (point - 1 < pointStrName.Length)
                        {
                            pointStrName[point - 1] = pointName;
                        }
                        else
                        {
                            MessageBox.Show(string.Format("配置文件Io点配置出错,卡号{0}, IO点{1}", card, point));
                        }
                    }
                }
            }
        }
    }
}
