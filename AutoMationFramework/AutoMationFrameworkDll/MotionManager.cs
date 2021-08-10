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
using System.Threading;
using System.Windows;
using AutoMationFrameworkModel;
using AutoMationFrameworkSystemDll;
using CommonTools.Tools;
using GalaSoft.MvvmLight.Messaging;
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

        #region 属性

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

        #endregion

        /// <summary>
        /// 类构造函数
        /// </summary>
        public MotionManager()
        {
            _dictTargetPos.Clear();
            _dictPauseAxis.Clear();
            Messenger.Default.Register<SystemCfg>(this, "axisCfg", UpdateAxisCfg);
        }

        #region 轴状态

        /// <summary>
        /// 暂停
        /// </summary>
        public void OnPause()
        {
            foreach (TargetPos targetPos in _dictTargetPos.Values)
            {
                bool flag;
                if (!_dictPauseAxis.TryGetValue(targetPos.Axis, out flag))
                    flag = true;

                if (flag)
                    PauseAxis(targetPos.Axis);
            }
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void OnResume()
        {
            foreach (TargetPos targetPos in _dictTargetPos.Values)
            {
                bool flag;
                if (!_dictPauseAxis.TryGetValue(targetPos.Axis, out flag))
                    flag = true;
                if (flag)
                {
                    switch (targetPos.Mode)
                    {
                        case MotionMode.Home:
                            Home(targetPos.Axis);
                            break;
                        case MotionMode.AbsMove:
                        case MotionMode.RelativeMove:
                            AbsMove(targetPos.Axis, (int)targetPos.Pos, (int)targetPos.Speed);
                            break;
                        case MotionMode.VelocityMove:
                            VelocityMove(targetPos.Axis, (int)targetPos.Speed);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 轴暂停
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        private bool PauseAxis(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);

            if (motionCard != null)
                return motionCard.StopAxis(nAxisNo - motionCard.GetMinAxisNo());

            return false;
        }

        #endregion

        #region 轴运动

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nParam">针对不同类型的卡，此参数含义不同，可以是方向，也可以是模式,也可以不使用</param>
        /// <returns></returns>
        public bool Home(int nAxisNo, int nParam)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            UpdateTarget(nAxisNo, MotionMode.Home);
            return motionCard.Home(nAxisNo - motionCard.GetMinAxisNo(), nParam);
        }

        /// <summary>回原点</summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">回原点模式</param>
        /// <param name="vm">最大速度</param>
        /// <param name="vo">爬行速度</param>
        /// <param name="acc">加速时间</param>
        /// <param name="dec">减速时间</param>
        /// <param name="offset">原点偏移</param>
        /// <param name="sFac">平滑系数</param>
        /// <returns></returns>
        public bool Home(int nAxisNo, int nParam, double vm, double vo, double acc, double dec, double offset = 0.0, double sFac = 0.0)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            UpdateTarget(nAxisNo, MotionMode.Home);
            return motionCard.Home(nAxisNo - motionCard.GetMinAxisNo(), nParam, vm, vo, acc, dec);
        }

        /// <summary>根据轴配置回原点</summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public bool Home(int nAxisNo)
        {
            AxisConfig cfg;
            if (!GetAxisCfg(nAxisNo, out cfg))
                return false;
            double dbHomeSpeedMax = cfg.HomeSpeedMax;
            double dbHomeSpeedMin = cfg.HomeSpeedMin;
            double dbHomeAcc      = cfg.HomeAcc;
            double dbHomeDec      = cfg.HomeDec;
            int    nHomeMode      = cfg.HomeMode;
            UpdateTarget(nAxisNo, MotionMode.Home);
            return Home(nAxisNo, nHomeMode, dbHomeSpeedMax, dbHomeSpeedMin, dbHomeAcc, dbHomeDec);
        }

        /// <summary>
        /// 带所有速度参数的绝对位置移动,以脉冲位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="fPos"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public bool AbsMove(int nAxisNo, double fPos, double vm, double acc, double dec, double vs = 0.0, double ve = 0.0, double sFac = 0.0)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            UpdateTarget(nAxisNo, MotionMode.AbsMove, fPos, vm);
            return motionCard.AbsMove(nAxisNo - motionCard.GetMinAxisNo(), fPos, vm * GetSpeedPercent(), acc, dec, vs, ve, sFac);
        }

        /// <summary>
        /// 绝对位置移动,以脉冲位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool AbsMove(int nAxisNo, int nPos, int nSpeed)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            UpdateTarget(nAxisNo, MotionMode.AbsMove, nPos, nSpeed);
            return motionCard.AbsMove(nAxisNo - motionCard.GetMinAxisNo(), nPos, (int)(nSpeed * GetSpeedPercent()));
        }

        /// <summary>
        /// 绝对位置移动，以mm or deg位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="dPos"></param>
        /// <param name="speedpercent">速度百分比</param>
        /// <returns></returns>
        public bool AbsMoveWithCfg(int nAxisNo, double dPos, double speedpercent = 100.0)
        {
            AxisConfig cfg;

            if (!GetAxisCfg(nAxisNo, out cfg))
                throw new Exception($"轴{nAxisNo}未配置");

            dPos *= cfg.GearRatio;
            double speed      = cfg.SpeedMax * speedpercent / 100.0;
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;

            UpdateTarget(nAxisNo, MotionMode.AbsMove, dPos, speed);
            return motionCard.AbsMove(nAxisNo - motionCard.GetMinAxisNo(), dPos, speed * GetSpeedPercent(), cfg.Acc, cfg.Dec, 0.0, 0.0, cfg.SFac);
        }

        /// <summary>
        /// 相对位置移动，以脉冲位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool RelativeMove(int nAxisNo, int nPos, int nSpeed)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;

            UpdateTarget(nAxisNo, MotionMode.RelativeMove, nPos, nSpeed);
            return motionCard.RelativeMove(nAxisNo - motionCard.GetMinAxisNo(), nPos, (int)(nSpeed * GetSpeedPercent()));
        }

        /// <summary>
        /// 相对位置移动，以mm or deg位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="dOffset"></param>
        /// <param name="speedpercent">速度百分比</param>
        /// <returns></returns>
        public bool RelativeMoveWithCfg(int nAxisNo, double dOffset, double speedpercent = 100.0)
        {
            AxisConfig cfg;
            if (!GetAxisCfg(nAxisNo, out cfg))
                throw new Exception($"轴{nAxisNo}未配置");

            dOffset *= cfg.GearRatio;
            double speed      = cfg.SpeedMax * speedpercent / 100.0;
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;

            UpdateTarget(nAxisNo, MotionMode.RelativeMove, dOffset, speed);
            return motionCard.RelativeMove(nAxisNo - motionCard.GetMinAxisNo(), dOffset, speed * GetSpeedPercent(), cfg.Acc, cfg.Dec, 0.0, 0.0, cfg.SFac);
        }

        /// <summary>
        /// 带所有速度参数的相对位置移动，以脉冲位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="fOffset"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public bool RelativeMove(int nAxisNo, double fOffset, double vm, double acc, double dec, double vs = 0.0, double ve = 0.0, double sFac = 0.0)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;

            UpdateTarget(nAxisNo, MotionMode.RelativeMove, fOffset, vm);
            return motionCard.RelativeMove(nAxisNo - motionCard.GetMinAxisNo(), fOffset, vm * GetSpeedPercent(), acc, dec, vs, ve, sFac);
        }

        /// <summary>
        /// jog运动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bPositive"></param>
        /// <param name="bStrat"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool JogMove(int nAxisNo, bool bPositive, int bStrat, int nSpeed)
        {
            Motion motionCard = GetMotionCard(nAxisNo);

            if (motionCard != null && (uint)nAxisNo > 0U)
                return motionCard.JogMove(nAxisNo - motionCard.GetMinAxisNo(), bPositive, bStrat, (int)(nSpeed * GetSpeedPercent()));
            return false;
        }

        /// <summary>
        /// 相对运行
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool VelocityMove(int nAxisNo, int nSpeed)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            UpdateTarget(nAxisNo, MotionMode.VelocityMove, 0.0, nSpeed);
            return motionCard.VelocityMove(nAxisNo - motionCard.GetMinAxisNo(), (int)(nSpeed * GetSpeedPercent()));
        }

        #endregion

        #region 获取设置轴信息

        /// <summary>
        /// 获取轴卡运动状态
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public long GetMotionState(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.GetMotionState(nAxisNo - motionCard.GetMinAxisNo());
            return -1;
        }

        /// <summary>
        /// 获取轴卡运动IO信号
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public long GetMotionIoState(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.GetMotionIoState(nAxisNo - motionCard.GetMinAxisNo());
            return -1;
        }

        /// <summary>
        /// 获取轴的当前位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public double GetAixsPos(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return uint.MaxValue;

            double aixsPos = motionCard.GetAixsPos(nAxisNo - motionCard.GetMinAxisNo());

            AxisConfig cfg;
            if (GetAxisCfg(nAxisNo, out cfg) && cfg.GearRatio > 0.0)
                aixsPos /= cfg.GearRatio;
            return aixsPos;
        }

        /// <summary>
        /// 得到轴所在的板卡对象
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public Motion GetMotionCard(int nAxisNo)
        {
            foreach (Motion motion in ListCard)
            {
                if (motion.AxisInRang(nAxisNo))
                    return motion;
            }

            string format = "轴号索引值{0}错误,无法找到对应的运动控制卡";
            if (LocationServices.GetLangType() == "en-us")
                format = "The axis number index value {0} is wrong, unable to find the corresponding motion control card";

            SingletonPattern<RunInforManager>.GetInstance().Error(ErrorType.ErrSystem, nameof(GetMotionCard) + nAxisNo, string.Format(format, nAxisNo));

            return null;
        }

        /// <summary>
        /// 轴是否正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public int IsAxisNormalStop(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return -1;
            int num = motionCard.IsAxisNormalStop(nAxisNo - motionCard.GetMinAxisNo());
            if (num != -1)
                UpdateTarget(nAxisNo, MotionMode.None);

            return num;
        }

        /// <summary>
        /// 判定轴是否到位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nInPosError">到位误差</param>
        /// <returns></returns>
        public int IsAxisInPos(int nAxisNo, int nInPosError = 1000)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return -1;
            int num = motionCard.IsAxisInPos(nAxisNo - motionCard.GetMinAxisNo(), nInPosError);

            if (num != -1)
                UpdateTarget(nAxisNo, MotionMode.None);
            return num;
        }

        /// <summary>
        /// 是否在正限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public int IsAxisPel(int nAxisNo)
        {
            long motionIoState = GetMotionIoState(nAxisNo);
            if (motionIoState == -1L)
                return -1;
            if (((ulong)motionIoState & 1UL) > 0UL)
                return 2;
            if (((ulong)motionIoState & 16UL) > 0UL)
                return 3;
            return ((ulong)motionIoState & 2UL) > 0UL ? 1 : 0;
        }

        /// <summary>
        /// 是否在负限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public int IsAxisMel(int nAxisNo)
        {
            long motionIoState = GetMotionIoState(nAxisNo);
            if (motionIoState == -1L)
                return -1;
            if (((ulong)motionIoState & 1UL) > 0UL)
                return 2;
            if (((ulong)motionIoState & 16UL) > 0UL)
                return 3;
            return ((ulong)motionIoState & 4UL) > 0UL ? 1 : 0;
        }

        /// <summary>
        /// 是否在原点
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public int IsAxisOrg(int nAxisNo)
        {
            long motionIoState = GetMotionIoState(nAxisNo);
            if (motionIoState == -1L)
                return -1;
            if (((ulong)motionIoState & 1UL) > 0UL)
                return 2;
            if (((ulong)motionIoState & 16UL) > 0UL)
                return 3;
            return ((ulong)motionIoState & 8UL) > 0UL ? 1 : 0;
        }

        /// <summary>
        /// 位置置零
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool SetPosZero(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.SetPosZero(nAxisNo - motionCard.GetMinAxisNo());
            return false;
        }

        /// <summary>
        /// 清除错误
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool ClearError(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.ClearError(nAxisNo - motionCard.GetMinAxisNo());
            return false;
        }

        /// <summary>
        /// 设置单轴的某一运动参数
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">参数:1:加速度 2:减速度 3:梯形或S曲线(8254) 3:起跳速度(S曲线) 4:结束速度(S曲线) 5:平滑时间(固高卡S曲线) 其它：自定义扩展</param>
        /// <param name="nData">参数值</param>
        /// <returns></returns>
        public bool SetAxisParam(int nAxisNo, int nParam, int nData)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.SetAxisParam(nAxisNo, nParam, nData);
            return false;
        }

        /// <summary>
        /// 设置单轴的某一运动参数(浮点)
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">参数:自定义扩展</param>
        /// <param name="fData">参数值</param>
        /// <returns></returns>
        public bool SetAxisParam(int nAxisNo, int nParam, double fData)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.SetAxisParam(nAxisNo, nParam, fData);
            return false;
        }

        /// <summary>
        /// 开启使能
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool ServoOn(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            UpdateTarget(nAxisNo, MotionMode.None);
            return motionCard.ServoOn(nAxisNo - motionCard.GetMinAxisNo());
        }

        /// <summary>
        /// 断开使能
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool ServoOff(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            UpdateTarget(nAxisNo, MotionMode.None);
            return motionCard.ServoOff(nAxisNo - motionCard.GetMinAxisNo());
        }

        /// <summary>
        /// 读取使能状态
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool GetServoState(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.GetServoState(nAxisNo - motionCard.GetMinAxisNo());
            return false;
        }

        /// <summary>
        /// 轴正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool StopAxis(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            UpdateTarget(nAxisNo, MotionMode.None);
            return motionCard.StopAxis(nAxisNo - motionCard.GetMinAxisNo());
        }

        /// <summary>
        /// 急停
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool StopEmg(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            UpdateTarget(nAxisNo, MotionMode.None);
            return motionCard.StopEmg(nAxisNo - motionCard.GetMinAxisNo());
        }

        /// <summary>
        /// 回原点过程中检测是否正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public int IsHomeNormalStop(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return 0;
            int num = motionCard.IsHomeNormalStop(nAxisNo - motionCard.GetMinAxisNo());
            if (num != -1)
                UpdateTarget(nAxisNo, MotionMode.None);
            return num;
        }

        /// <summary>
        /// 获取运动控制卡名称
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        private string GetCardName(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.GetCardName();

            return null;
        }

        /// <summary>
        /// 获取轴数组对应的卡号，多轴运动时需要用到,对轴数组的合理性进行判定
        /// </summary>
        /// <param name="nAixsArray"></param>
        /// <returns></returns>
        public Motion GetMotionCard(ref int[] nAixsArray)
        {
            if (nAixsArray.Length < 2)
            {
                SingletonPattern<RunInforManager>.GetInstance().Error(ErrorType.ErrMotion, nameof(GetMotionCard), "motion Card nAixsArray < 2 ,CheckAxisArray Error");
                return null;
            }
            Motion motionCard = GetMotionCard(nAixsArray[0]);

            if (motionCard == null)
            {
                SingletonPattern<RunInforManager>.GetInstance().Error(ErrorType.ErrMotion, nameof(GetMotionCard),
                    $"motion Card Aixs {nAixsArray[0]} error ,CheckAxisArray Error");
                return null;
            }
            for (int index = 1; index < nAixsArray.Length; ++index)
            {
                if (motionCard != GetMotionCard(nAixsArray[index]))
                {
                    SingletonPattern<RunInforManager>.GetInstance().Error(ErrorType.ErrMotion, nameof(GetMotionCard),
                        $"motion Card AbsLinearMove axis {nAixsArray[index]}  Error");
                    return null;
                }
            }
            return motionCard;
        }

        /// <summary>
        /// 启用软件正限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public virtual bool SetSpelEnable(int nAxisNo, bool bEnable)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.SetSpelEnable(nAxisNo - motionCard.GetMinAxisNo(), bEnable);
            return false;
        }

        /// <summary>
        /// 启用软件负限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public virtual bool SetSmelEnable(int nAxisNo, bool bEnable)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.SetSmelEnable(nAxisNo - motionCard.GetMinAxisNo(), bEnable);
            return false;
        }

        /// <summary>
        /// 设置软件正限位位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual bool SetSpelPos(int nAxisNo, double pos)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            AxisConfig cfg;
            if (GetAxisCfg(nAxisNo, out cfg))
                pos = cfg.GearRatio * pos;
            return motionCard.SetSpelPos(nAxisNo - motionCard.GetMinAxisNo(), pos);
        }

        /// <summary>
        /// 设置软件负限位位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual bool SetSmelPos(int nAxisNo, double pos)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard == null)
                return false;
            AxisConfig cfg;
            if (GetAxisCfg(nAxisNo, out cfg))
                pos = cfg.GearRatio * pos;
            return motionCard.SetSmelPos(nAxisNo - motionCard.GetMinAxisNo(), pos);
        }

        /// <summary>
        /// 设置轴是否启用暂停
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bEnable"></param>
        public void EnableAxisPause(int nAxisNo, bool bEnable)
        {
            _dictPauseAxis.AddOrUpdate(nAxisNo, bEnable, (key, value) => bEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool IsEnbaleAxisPause(int nAxisNo)
        {
            bool flag;
            if (!_dictPauseAxis.TryGetValue(nAxisNo, out flag))
                flag = false;
            return flag;
        }

        /// <summary>
        /// 获取系统速度
        /// </summary>
        /// <returns></returns>
        public double GetSpeedPercent()
        {
            int paramInt = SingletonPattern<SystemManager>.GetInstance().GetParamInt("SystemSpeed");
            if (paramInt == 0)
                return 1.0;
            return paramInt / 100.0;
        }

        /// <summary>
        /// 更新目标位置
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="mode">运动模式</param>
        /// <param name="pos">运动位置</param>
        /// <param name="speed">速度</param>
        private void UpdateTarget(int nAxisNo, MotionMode mode, double pos = 0.0, double speed = 0.0)
        {
            TargetPos target;
            bool flag;
            if (_dictPauseAxis.TryGetValue(nAxisNo, out flag) && !flag)
                return;
            switch (mode)
            {
                case MotionMode.None:
                    _dictTargetPos.TryRemove(nAxisNo, out target);
                    break;
                case MotionMode.Home:
                case MotionMode.VelocityMove:
                    target = new TargetPos();
                    target.Axis = nAxisNo;
                    target.Mode = mode;
                    target.Pos = pos;
                    target.Speed = speed;
                    _dictTargetPos.AddOrUpdate(nAxisNo, target, (key, value) => target);
                    break;
                case MotionMode.AbsMove:
                    target = new TargetPos();
                    target.Axis = nAxisNo;
                    target.Mode = mode;
                    target.Pos = pos;
                    target.Speed = speed;
                    _dictTargetPos.AddOrUpdate(nAxisNo, target, (key, value) => target);
                    break;
                case MotionMode.RelativeMove:
                    target = new TargetPos();
                    target.Axis = nAxisNo;
                    target.Mode = MotionMode.AbsMove;
                    target.Pos = pos + GetAixsPls(nAxisNo);
                    target.Speed = speed;
                    _dictTargetPos.AddOrUpdate(nAxisNo, target, (key, value) => target);
                    break;
            }
        }

        /// <summary>
        /// 获取轴的当前脉冲位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public double GetAixsPls(int nAxisNo)
        {
            Motion motionCard = GetMotionCard(nAxisNo);
            if (motionCard != null)
                return motionCard.GetAixsPos(nAxisNo - motionCard.GetMinAxisNo());
            return uint.MaxValue;
        }

        private void ClearTarget()
        {
            _dictTargetPos.Clear();
        }

        /// <summary>
        /// 获取轴配置
        /// </summary>
        /// <param name="nAxis"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public bool GetAxisCfg(int nAxis, out AxisConfig cfg)
        {
            if (_dictAxisCfg.ContainsKey(nAxis))
            {
                cfg = _dictAxisCfg[nAxis];
                return true;
            }
            cfg = new AxisConfig();
            return false;
        }

        #endregion

        #region 初始化配置

        /// <summary>
        /// 初始化所有轴卡
        /// </summary>
        /// <returns></returns>
        public bool InitAllCard()
        {
            bool flag = true;
            foreach (Motion motion in ListCard)
            {
                if (!motion.Init())
                    flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 反初化所有轴卡
        /// </summary>
        public void DeinitAllCard()
        {
            foreach (Motion motion in ListCard)
            {
                if (motion.IsEnable())
                    motion.DeInit();
            }
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        public override void ThreadMonitor()
        {
            while (BRunThread)
                Thread.Sleep(100);
        }

        private void UpdateAxisCfg(SystemCfg systemCfg)
        {
            _dictAxisCfg.Clear();
            var axisList = systemCfg.AxisConfigList;

            foreach (var axisCfg in axisList)
            {
                int axisNum;
                if (int.TryParse(axisCfg.AxisNum, out axisNum))
                {
                    double gearRatio, homeMinSpeed, homeMaxSpeed, homeAcc, homeDec, speedMax, acc, dec, sFac, smelPos, spelPos;
                    int inPosError;
                    bool enableSpel, enableSmel;
                    int homeMode = axisCfg.HomeMode;
                    if (!double.TryParse(axisCfg.GearRatio, out gearRatio)) gearRatio = 1.0;
                    //if (!int.TryParse(axisCfg.HomeMode, out homeMode)) homeMode = 1;
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
                    if (!double.TryParse(axisCfg.SpelPos, out spelPos)) spelPos = 10000000.0;
                    if (!double.TryParse(axisCfg.SmelPos, out smelPos)) smelPos = -10000000.0;

                    _dictAxisCfg.Add(axisNum, new AxisConfig
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

            MessageBox.Show(LocationServices.GetLang("ApplySucceed"));
        }

        private void AddCard(string strName, int nCardNo, int nAxisMin, int nAxisMax)
        {
            string str = LocationServices.GetLang("NotFoundMotionCard");
            Type type = Assembly.GetAssembly(typeof(Motion)).GetType("MotionIO.Motion" + strName);
            if (type == null)
                throw new Exception(string.Format(str, strName));
            object[] objArray = {
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
        public void ReadMotionCardFromCfg()
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

                    AddCard(type, Convert.ToInt32(index), Convert.ToInt32(minAxis), Convert.ToInt32(maxAxis));
                }
            }

            if (axisList == null)
            {
                IoManager.GetInstance().SystemConfig.AxisConfigList = new List<AxisCfg>();
                axisList = IoManager.GetInstance().SystemConfig.AxisConfigList;
            }

            if (axisList.Count <= 0)
            {
                foreach (MotionCard motionCard in motionList)
                {
                    int min = Convert.ToInt32(motionCard.MinAxisNum);
                    int max = Convert.ToInt32(motionCard.MaxAxisNum);

                    for (int i = min; i <= max; i++)
                    {
                        AxisConfig cfg;
                        if (!GetAxisCfg(i, out cfg))
                        {
                            cfg.GearRatio = 1.0;
                            cfg.HomeMode = 1;
                            cfg.HomeSpeedMin = 1000;
                            cfg.HomeSpeedMax = 5000;
                            cfg.HomeAcc = 0.1;
                            cfg.HomeDec = 0.1;
                            cfg.SpeedMax = 200000;
                            cfg.Acc = 0.1;
                            cfg.Dec = 0.1;
                            cfg.SFac = 0.0;
                            cfg.InPosError = 1;
                            cfg.EnableSpel = false;
                            cfg.SpelPos = 10000000.0;
                            cfg.EnableSmel = false;
                            cfg.SmelPos = -10000000.0;
                        }

                        _dictAxisCfg.Add(i, cfg);

                        axisList.Add(new AxisCfg
                        {
                            AxisNum = i.ToString(),
                            GearRatio = "1.0",
                            HomeMode = 1,
                            HomeSpeedMin = "1000",
                            HomeSpeedMax = "5000",
                            HomeAcc = "0.1",
                            HomeDec = "0.1",
                            SpeedMax = "200000",
                            Acc = "0.1",
                            Dec = "0.1",
                            SFac = "0.0",
                            InPosError = "1",
                            EnableSpel = "False",
                            SpelPos = "10000000.0",
                            EnableSmel = "False",
                            SmelPos = "-10000000.0",
                        });
                    }
                }
                return;
            }

            foreach (var axisCfg in axisList)
            {
                int axisNum;
                if (int.TryParse(axisCfg.AxisNum, out axisNum))
                {
                    double gearRatio, homeMinSpeed, homeMaxSpeed, homeAcc, homeDec, speedMax, acc, dec, sFac, smelPos, spelPos;
                    int inPosError;
                    bool enableSpel, enableSmel;
                    int homeMode = axisCfg.HomeMode;
                    if (!double.TryParse(axisCfg.GearRatio, out gearRatio)) gearRatio = 1.0;
                    //if (!int.TryParse(axisCfg.HomeMode, out homeMode)) homeMode = 1;
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
                    if (!double.TryParse(axisCfg.SpelPos, out spelPos)) spelPos = 10000000.0;
                    if (!double.TryParse(axisCfg.SmelPos, out smelPos)) smelPos = -10000000.0;

                    _dictAxisCfg.Add(axisNum, new AxisConfig
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
        #endregion

        #region 插补运动

        /// <summary>
        /// 以当前位置为起始点进行多轴直线插补
        /// </summary>
        /// <param name="nAixsArray">轴数组</param>
        /// <param name="fPosArray">目标点的绝对座标位置</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public bool AbsLinearMove(ref int[] nAixsArray, ref double[] fPosArray, double vm, double acc, double dec, double vs = 0.0, double ve = 0.0, double sFac = 0.0)
        {
            Motion motionCard = GetMotionCard(ref nAixsArray);
            if (motionCard == null)
                return false;
            int[] nAixsArray1 = new int[nAixsArray.Length];
            for (int index = 0; index < nAixsArray.Length; ++index)
                nAixsArray1[index] = nAixsArray[index] - motionCard.GetMinAxisNo();
            return motionCard.AbsLinearMove(ref nAixsArray1, ref fPosArray, vm, acc, dec, vs, ve, sFac);
        }

        /// <summary>
        /// 以当前位置为起始点进行多轴直线插补
        /// </summary>
        /// <param name="nAixsArray">轴数组,第一个轴为主轴，设定加速度等参数以主轴为基准</param>
        /// <param name="fPosOffsetArray">目标点的相对座标位置</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public bool RelativeLinearMove(ref int[] nAixsArray, ref double[] fPosOffsetArray, double vm, double acc, double dec, double vs = 0.0, double ve = 0.0, double sFac = 0.0)
        {
            Motion motionCard = GetMotionCard(ref nAixsArray);
            if (motionCard == null)
                return false;
            int[] nAixsArray1 = new int[nAixsArray.Length];
            for (int index = 0; index < nAixsArray.Length; ++index)
                nAixsArray1[index] = nAixsArray[index] - motionCard.GetMinAxisNo();
            return motionCard.RelativeLinearMove(ref nAixsArray1, ref fPosOffsetArray, vm, acc, dec, vs, ve, sFac);
        }

        /// <summary>
        /// 以当前点位为起始点进行两轴圆弧插补运动
        /// </summary>
        /// <param name="nAixsArray">轴数组,第一个轴为主轴，设定加速度等参数以主轴为基准</param>
        /// <param name="fCenterArray">圆心的绝对座标位置</param>
        /// <param name="fEndArray">终点的绝对座标位置</param>
        /// <param name="dir">圆弧的方向，　0:正向，　１：负向</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public bool AbsArcMove(ref int[] nAixsArray, ref double[] fCenterArray, ref double[] fEndArray, int dir, double vm, double acc, double dec, double vs = 0.0, double ve = 0.0, double sFac = 0.0)
        {
            Motion motionCard = GetMotionCard(ref nAixsArray);
            if (motionCard == null)
                return false;
            int[] nAixsArray1 = new int[nAixsArray.Length];
            for (int index = 0; index < nAixsArray.Length; ++index)
                nAixsArray1[index] = nAixsArray[index] - motionCard.GetMinAxisNo();
            return motionCard.AbsArcMove(ref nAixsArray1, ref fCenterArray, ref fEndArray, dir, vm, acc, dec, vs, ve, sFac);
        }

        /// <summary>
        /// 以当前点位为起始点，以偏差位置为圆心，进行多轴圆弧插补运动
        /// </summary>
        /// <param name="nAixsArray">轴数组,第一个轴为主轴，设定加速度等参数以主轴为基准</param>
        /// <param name="fCenterOffsetArray">圆心的相对座标位置</param>
        /// <param name="fEndOffsetArray">终点的相对座标位置</param>
        /// <param name="dir">圆弧的方向，　0:正向，　１：负向</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public bool RelativeArcMove(ref int[] nAixsArray, ref double[] fCenterOffsetArray, ref double[] fEndOffsetArray, int dir, double vm, double acc, double dec, double vs = 0.0, double ve = 0.0, double sFac = 0.0)
        {
            Motion motionCard = GetMotionCard(ref nAixsArray);
            if (motionCard == null)
                return false;
            int[] nAixsArray1 = new int[nAixsArray.Length];
            for (int index = 0; index < nAixsArray.Length; ++index)
                nAixsArray1[index] = nAixsArray[index] - motionCard.GetMinAxisNo();
            return motionCard.RelativeArcMove(ref nAixsArray1, ref fCenterOffsetArray, ref fEndOffsetArray, dir, vm, acc, dec, vs, ve, sFac);
        }

        #endregion

        #region 配置运动参数

        /// <summary>
        /// 配置一个连续运动的缓冲表(2000点的buff需要更新fimeware)
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="nAixsArray">轴号数组</param>
        /// <param name="bAbsolute">true:绝对位置模式，　false:相对位置模式</param>
        /// <returns></returns>
        public bool ConfigPointTable(int nPointTableIndex, ref int[] nAixsArray, bool bAbsolute)
        {
            Motion motionCard = GetMotionCard(ref nAixsArray);
            if (motionCard == null)
                return false;
            int[] nAixsArray1 = new int[nAixsArray.Length];
            for (int index = 0; index < nAixsArray.Length; ++index)
                nAixsArray1[index] = nAixsArray[index] - motionCard.GetMinAxisNo();
            return motionCard.ConfigPointTable(nPointTableIndex, ref nAixsArray1, bAbsolute);
        }

        /// <summary>
        /// 获得连续运动BUFF区所对应的板卡
        /// </summary>
        /// <param name="nPointTableIndex"></param>
        /// <returns></returns>
        public Motion GetPointTableCard(int nPointTableIndex)
        {
            foreach (Motion motion in ListCard)
            {
                int num;
                if (motion.DicBoard.TryGetValue(nPointTableIndex, out num))
                    return motion;
            }
            SingletonPattern<RunInforManager>.GetInstance().Error(ErrorType.ErrMotion, nameof(GetPointTableCard), "PointTable buffer not exist, GetPointTableCard error");
            return null;
        }

        /// <summary>
        /// 向连续运动的缓冲表插入一个直线插补运动（多轴）
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="positionArray">目标位置数组，需要轴号数组匹配</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="vm">最大速度</param>
        /// <param name="ve">终点速度</param>
        /// <param name="sf">S曲线因子</param>
        /// <returns></returns>
        public virtual bool PointTable_Line_Move(int nPointTableIndex, ref double[] positionArray, double acc, double dec, double vs, double vm, double ve, double sf)
        {
            Motion pointTableCard = GetPointTableCard(nPointTableIndex);
            if (pointTableCard != null)
                return pointTableCard.PointTable_Line_Move(nPointTableIndex, ref positionArray, acc, dec, vs, vm, ve, sf);
            return false;
        }

        /// <summary>
        /// 向连续运动的缓冲表插入一个圆弧插补运动（两轴）
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="centerArray">圆弧中心点位置</param>
        /// <param name="endArray">圆弧结束点位置</param>
        /// <param name="dir">圆弧的方向, 0:顺时针，-1:逆时针</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="vm">最大速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sf">S曲线因子</param>
        /// <returns></returns>
        public bool PointTable_ArcE_Move(int nPointTableIndex, ref double[] centerArray, ref double[] endArray, short dir, double acc, double dec, double vs, double vm, double ve, double sf)
        {
            Motion pointTableCard = GetPointTableCard(nPointTableIndex);
            if (pointTableCard != null)
                return pointTableCard.PointTable_ArcE_Move(nPointTableIndex, ref centerArray, ref endArray, dir, acc, dec, vs, vm, ve, sf);
            return false;
        }

        /// <summary>
        /// 向连续运动缓冲表插入一个与运动同步的IO控制(IO控制在运动之前)
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="nChannel">IO点的序号</param>
        /// <param name="bOn">1：on, 0: off</param>
        /// <returns></returns>
        public bool PointTable_IO(int nPointTableIndex, int nChannel, int bOn)
        {
            Motion pointTableCard = GetPointTableCard(nPointTableIndex);
            if (pointTableCard != null)
                return pointTableCard.PointTable_IO(nPointTableIndex, nChannel, bOn);
            return false;
        }

        /// <summary>
        /// 向缓冲区插入一个延时指令
        /// </summary>
        /// <param name="nPointTableIndex"></param>
        /// <param name="nMillsecond">需要延时的毫秒值</param>
        /// <returns></returns>
        public bool PointTable_Delay(int nPointTableIndex, int nMillsecond)
        {
            Motion pointTableCard = GetPointTableCard(nPointTableIndex);
            if (pointTableCard != null)
                return pointTableCard.PointTable_Delay(nPointTableIndex, nMillsecond);
            return false;
        }

        /// <summary>
        /// 启动或停止一个连续运动
        /// </summary>
        /// <param name="nPointTableIndex">连续运动列表的序号</param>
        /// <param name="bStart">true:开始运行, false:停止运行</param>
        /// <returns></returns>
        public bool PointTable_Start(int nPointTableIndex, bool bStart)
        {
            Motion pointTableCard = GetPointTableCard(nPointTableIndex);
            if (pointTableCard != null)
                return pointTableCard.PointTable_Start(nPointTableIndex, bStart);
            return false;
        }

        /// <summary>
        /// 确定连续运动列表的BUFF是否可以插入新的运动
        /// </summary>
        /// <param name="nPointTableIndex"></param>
        /// <returns>true: 可以插入　，　false: BUFF已满</returns>
        public bool PointTable_IsIdle(int nPointTableIndex)
        {
            Motion pointTableCard = GetPointTableCard(nPointTableIndex);
            if (pointTableCard != null)
                return pointTableCard.PointTable_IsIdle(nPointTableIndex);
            return false;
        }

        #endregion
    }
}
