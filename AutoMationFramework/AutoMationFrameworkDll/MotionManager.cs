/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    System motion manager class              *
*********************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using CommonTools.Manager;
using CommonTools.Tools;
using MotionIO;

namespace AutoMationFrameworkDll
{
    /// <summary>
    /// 轴配置
    /// </summary>
    public struct AxisConfig
    {
        /// <summary>
        /// 齿轮比，单位脉冲每毫米、脉冲每度
        /// </summary>
        public double GearRatio;

        /// <summary>
        /// 最小速度，单位脉冲
        /// </summary>
        public double HomeSpeedMin;

        /// <summary>
        /// 最大速度，单位脉冲
        /// </summary>
        public double HomeSpeedMax;

        /// <summary>
        /// 加速时间，单位秒
        /// </summary>
        public double HomeAcc;

        /// <summary>
        /// 减速时间，单位秒
        /// </summary>
        public double HomeDec;

        /// <summary>
        /// 回原点的方式
        /// </summary>
        public int HomeMode;

        /// <summary>
        /// 最大速度，单位脉冲
        /// </summary>
        public double SpeedMax;

        /// <summary>
        /// 加速度，单位秒
        /// </summary>
        public double Acc;

        /// <summary>
        /// 减速度，单位秒
        /// </summary>
        public double Dec;

        /// <summary>
        /// 平滑系数，取值0~1，值越大越平滑
        /// </summary>
        public double SFac;

        /// <summary>
        /// 到位后的绝对值误差
        /// </summary>
        public int InPosError;

        /// <summary>
        /// 启用软正限位
        /// </summary>
        public bool EnableSpel;

        /// <summary>
        /// 启用软负限位
        /// </summary>
        public bool EnableSmel;

        /// <summary>
        /// 软正限位，单位pls
        /// </summary>
        public double SpelPos;

        /// <summary>
        /// 软负限位,单位pls
        /// </summary>
        public double SmelPos;
    }

    public class MotionManager : SingletonPattern<MotionManager>
    {
        /// <summary>
        /// 当前运动模式
        /// </summary>
        internal enum MotionMode
        {
            /// <summary>无</summary>
            None,
            /// <summary>回原点</summary>
            Home,
            /// <summary>绝对运动</summary>
            AbsMove,
            /// <summary>相对运动</summary>
            RelativeMove,
            /// <summary>速度运动</summary>
            VelocityMove,
        }

        /// <summary>
        /// 目标位置
        /// </summary>
        internal class TargetPos
        {
            public int Axis;
            public MotionMode Mode;
            public double Pos;
            public double Speed;
        }

        /// <summary>
        /// 板卡列表
        /// </summary>
        public List<Motion> ListCard = new List<Motion>();

        /// <summary>
        /// 运动目标位置
        /// </summary>
        private ConcurrentDictionary<int, TargetPos> _dictTargetPos = new ConcurrentDictionary<int, TargetPos>();

        /// <summary>
        /// 需要暂停的轴
        /// </summary>
        private ConcurrentDictionary<int, bool> _dictPauseAxis = new ConcurrentDictionary<int, bool>();

        /// <summary>
        /// 轴配置
        /// </summary>
        private Dictionary<int, AxisConfig> _dictAxisCfg = new Dictionary<int, AxisConfig>();

        /// <summary>类构造函数</summary>
        public MotionManager()
        {
            _dictTargetPos.Clear();
            _dictPauseAxis.Clear();
        }

        private void AddCard(string strName, int nCardNo, int nAxisMin, int nAxisMax)
        {
            string str = LocationServices.GetLang("NotFoundMotionCard");
            Type type = Assembly.GetAssembly(typeof(Motion)).GetType("MotionIO.Motion" + strName);
            if (type == null)
                throw new Exception(string.Format(str, strName));
            object[] objArray = new object[4]
            {
                 nCardNo,
                 strName,
                 nAxisMin,
                 nAxisMax
            };

            ListCard.Add(Activator.CreateInstance(type, objArray) as Motion);
        }

        /// <summary>
        /// 读取系统配置文件里的运动卡信息
        /// </summary>
        public void ReadCardFromCfg()
        {
            ListCard.Clear();
            _dictAxisCfg.Clear();

            var motionList = IoManager.GetInstance().SystemConfig.MotionCardsList;
            var axisList = IoManager.GetInstance().SystemConfig.AxisConfigList;

            if (motionList.Count > 0)
            {
                foreach (var motionCard in motionList)
                {
                    string index = motionCard.Index;
                    string type = motionCard.CardType;
                    string minAxis = motionCard.MinAxisNum;
                    string maxAxis = motionCard.MaxAxisNum;

                    AddCard(type,Convert.ToInt32(index),Convert.ToInt32(minAxis),Convert.ToInt32(maxAxis));
                }
            }

            if (axisList.Count <= 0)
            {
                return;
            }

            foreach (var axisCfg in axisList)
            {
                int axisNum;
                if (int.TryParse(axisCfg.AxisNum, out axisNum))
                {
                    double gearRatio,homeMinSpeed,homeMaxSpeed,homeAcc,homeDec,speedMax,acc,dec,sFac,smelPos,spelPos;
                    int homeMode,inPosError;
                    bool enableSpel, enableSmel;
                    if (!double.TryParse(axisCfg.GearRatio, out gearRatio)) gearRatio = 1.0;
                    if (!int.TryParse(axisCfg.HomeMode, out homeMode)) homeMode = 1;
                    if (!double.TryParse(axisCfg.HomeSpeedMin, out homeMinSpeed)) homeMinSpeed = 1000.0;
                    if (!double.TryParse(axisCfg.HomeSpeedMax, out homeMaxSpeed)) homeMaxSpeed = 5000.0;
                    if (!double.TryParse(axisCfg.HomeAcc, out homeAcc)) homeAcc = 0.1;
                    if (!double.TryParse(axisCfg.HomeDec, out homeDec)) homeDec = 0.1;
                    if (!double.TryParse(axisCfg.SpeedMax, out speedMax)) speedMax = 200000;
                    if (!double.TryParse(axisCfg.Acc, out acc)) acc = 0.1;
                    if (!double.TryParse(axisCfg.Dec, out dec)) dec = 0.1;
                    if (!double.TryParse(axisCfg.SFac, out sFac)) sFac = 0.0;
                    if (!int.TryParse(axisCfg.InPosError, out inPosError)) inPosError = 1;
                    if (!bool.TryParse(axisCfg.EnableSpel, out enableSpel)) enableSpel = false;
                    if (!bool.TryParse(axisCfg.EnableSmel, out enableSmel)) enableSmel = false;
                    if (!double.TryParse(axisCfg.SpelPos, out spelPos)) spelPos = -10000000.0;
                    if (!double.TryParse(axisCfg.SmelPos, out smelPos)) smelPos = 10000000.0;

                    _dictAxisCfg.Add(axisNum,new AxisConfig
                    {
                        GearRatio = gearRatio,
                        HomeMode = homeMode,
                        HomeSpeedMin = homeMinSpeed,
                        HomeSpeedMax = homeMaxSpeed,
                        HomeAcc = homeAcc,
                        HomeDec = homeDec,
                        SpeedMax = speedMax,
                        Acc = acc,
                        Dec = dec,
                        SFac = sFac,
                        InPosError = inPosError,
                        EnableSpel = enableSpel,
                        SpelPos = spelPos,
                        EnableSmel = enableSmel,
                        SmelPos = smelPos
                    });
                }
            }
        }
    }
}
