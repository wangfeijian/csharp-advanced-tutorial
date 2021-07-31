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

namespace CommonTools.Model
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
