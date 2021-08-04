/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Read write config file use XML           *
*********************************************************************/

using System.Collections.Generic;

namespace AutoMationFrameworkModel
{
    public class IoInputPoint
    {
        public string CardIndex { get; set; }
        public string PointIndex { get; set; }
        public string PointName { get; set; }
        public string PointEngName { get; set; }
    }

    public class IoOutputPoint
    {
        public string CardIndex { get; set; }
        public string PointIndex { get; set; }
        public string PointName { get; set; }
        public string PointEngName { get; set; }
    }

    public class IoCardInfo
    {
        public string CardIndex { get; set; }
        public string CardNum { get; set; }
        public string CardType { get; set; }
    }

    public class SysInputPoint
    {
        public string FuncDesc { get; set; }
        public string CardNum { get; set; }
        public string PointIndex { get; set; }
        public string EffectiveLevel { get; set; }
    }
    public class SysOutputPoint
    {
        public string FuncDesc { get; set; }
        public string CardNum { get; set; }
        public string PointIndex { get; set; }
        public string EffectiveLevel { get; set; }
    }

    public class MotionCard
    {
        public string Index { get; set; }
        public string CardType { get; set; }
        public string MinAxisNum { get; set; }
        public string MaxAxisNum { get; set; }
    }

    public class AxisCfg
    {
        /// <summary>
        /// 轴号
        /// </summary>
        public string AxisNum { get; set; }

        /// <summary>
        /// 齿轮比，单位脉冲每毫米、脉冲每度
        /// </summary>
        public string GearRatio { get; set; }

        /// <summary>
        /// 最小速度，单位脉冲
        /// </summary>
        public string HomeSpeedMin { get; set; }

        /// <summary>
        /// 最大速度，单位脉冲
        /// </summary>
        public string HomeSpeedMax { get; set; }

        /// <summary>
        /// 加速时间，单位秒
        /// </summary>
        public string HomeAcc { get; set; }

        /// <summary>
        /// 减速时间，单位秒
        /// </summary>
        public string HomeDec { get; set; }

        /// <summary>
        /// 回原点的方式
        /// </summary>
        public int HomeMode { get; set; }

        /// <summary>
        /// 最大速度，单位脉冲
        /// </summary>
        public string SpeedMax { get; set; }

        /// <summary>
        /// 加速度，单位秒
        /// </summary>
        public string Acc { get; set; }

        /// <summary>
        /// 减速度，单位秒
        /// </summary>
        public string Dec { get; set; }

        /// <summary>
        /// 平滑系数，取值0~1，值越大越平滑
        /// </summary>
        public string SFac { get; set; }

        /// <summary>
        /// 到位后的绝对值误差
        /// </summary>
        public string InPosError { get; set; }

        /// <summary>
        /// 启用软正限位
        /// </summary>
        public string EnableSpel { get; set; }

        /// <summary>
        /// 启用软负限位
        /// </summary>
        public string EnableSmel { get; set; }

        /// <summary>
        /// 软负限位，单位pls
        /// </summary>
        public string SpelPos { get; set; }

        /// <summary>
        /// 软正限位,单位pls
        /// </summary>
        public string SmelPos { get; set; }
    }

    public class EthInfo
    {
        public string EthNum { get; set; }
        public string EthDefine { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public string TimeOut { get; set; }
        public string Command { get; set; }
    }

    public class StationInfo
    {
        public string StationIndex { get; set; }
        public string StationName { get; set; }
        public string AxisX { get; set; }
        public string AxisY { get; set; }
        public string AxisZ { get; set; }
        public string AxisU { get; set; }
        public string AxisA { get; set; }
        public string AxisB { get; set; }
        public string AxisC { get; set; }
        public string AxisD { get; set; }
    }

    public class SystemCfg
    {
        private List<string> _boolVallue = new List<string> { "True", "False" };

        public List<string> BoolValue => _boolVallue;

        private List<IoInputPoint> _ioInput;

        public List<IoInputPoint> IoInput
        {
            get { return _ioInput; }
            set { _ioInput = value; }
        }

        private List<IoOutputPoint> _ioOutput;

        public List<IoOutputPoint> IoOutput
        {
            get { return _ioOutput; }
            set { _ioOutput = value; }
        }

        private List<IoCardInfo> _ioCards;

        public List<IoCardInfo> IoCardsList
        {
            get { return _ioCards; }
            set { _ioCards = value; }
        }

        private List<SysInputPoint> _sysInput;

        public List<SysInputPoint> SysInput
        {
            get { return _sysInput; }
            set { _sysInput = value; }
        }

        private List<SysOutputPoint> _sysOutput;

        public List<SysOutputPoint> SysOutput
        {
            get { return _sysOutput; }
            set { _sysOutput = value; }
        }

        private List<MotionCard> _motionCards;

        public List<MotionCard> MotionCardsList
        {
            get { return _motionCards; }
            set { _motionCards = value; }
        }
        private List<AxisCfg> _axisConfigList;

        public List<AxisCfg> AxisConfigList
        {
            get { return _axisConfigList; }
            set { _axisConfigList = value; }
        }

        private List<EthInfo> _ethInfos;

        public List<EthInfo> EthInfos
        {
            get { return _ethInfos; }
            set { _ethInfos = value; }
        }

        private List<StationInfo> _stationInfos;

        public List<StationInfo> StationInfos
        {
            get { return _stationInfos; }
            set { _stationInfos = value; }
        }
    }
}
