/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-02                               *
*                                                                    *
*           ModifyTime:     2021-08-02                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Motion leadshine card control class      *
*********************************************************************/
using System;
using csLTDMC;
using CommonTools.Tools;
using System.Windows;
using AutoMationFrameworkSystemDll;

namespace MotionIO
{
    /// <summary>
    /// 雷赛EtherCAT控制卡资源
    /// </summary>
    public class DmcEtherCatCard
    {
        /// <summary>
        /// 控制卡最大数量，使用总线的方式，只需要一张卡
        /// </summary>
        private const int CardCountMax = 8;

        /// <summary>
        /// 控制卡ID
        /// </summary>
        public int CardId;

        /// <summary>
        /// 轴的数量
        /// </summary>
        public int AxesCount;

        /// <summary>
        /// 输入端口数量
        /// </summary>
        public int InputCount;

        /// <summary>
        /// 输出端口数量
        /// </summary>
        public int OutputCount;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DmcEtherCatCard()
        {
            short nCardCount = LTDMC.dmc_board_init();//获取卡数量
            if (nCardCount <= 0 || nCardCount > CardCountMax)
            {
                CardId = -1;
                return;
            }

            ushort nSuccessCount = 0;
            ushort[] nCardIds = new ushort[CardCountMax];
            uint[] nCardTypes = new uint[CardCountMax];
            short ret = LTDMC.dmc_get_CardInfList(ref nSuccessCount, nCardTypes, nCardIds);
            if (ret != 0)
            {
                CardId = -1;
                return;
            }

            // 控制卡
            CardId = nCardIds[0];

            // 轴数量
            uint nAxesCnt = 0;
            ret = LTDMC.nmc_get_total_axes((ushort)CardId, ref nAxesCnt);
            if (ret == 0)
            {
                AxesCount = (int)nAxesCnt;
            }
            else
            {
                CardId = -1;
                return;
            }

            // IO数量
            ushort nInCnt = 0;
            ushort nOutCnt = 0;
            ret = LTDMC.nmc_get_total_ionum((ushort)CardId, ref nInCnt, ref nOutCnt);
            if (ret == 0)
            {
                InputCount = nInCnt;
                OutputCount = nOutCnt;
            }
            else
            {
                CardId = -1;
            }
        }

        private static DmcEtherCatCard _instance;

        /// <summary>
        /// 实例
        /// </summary>
        /// <returns></returns>
        public static DmcEtherCatCard Instance()
        {
            if (_instance == null)
            {
                _instance = new DmcEtherCatCard();
            }

            return _instance;
        }
    }

    /// <summary>
    /// 雷赛EtherCAT运动控制卡封装,类名必须以"Motion"前导，否则加载不到
    /// </summary>
    public class MotionDmcEcat : Motion
    {
        /// <summary>
        /// 控制卡ID
        /// </summary>
        private ushort _nCardId;

        //todo:板卡类应该只初始化一次
        /// <summary>构造函数
        /// 
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
        public MotionDmcEcat(int cardIndex, string strName, int nMinAxisNo, int nMaxAxisNo)
            : base(cardIndex, strName, nMinAxisNo, nMaxAxisNo)
        {
            BEnable = false;
        }
        /// <summary>
        /// 轴卡初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            string str1 = LocationServices.GetLang("MotionCardGetIdError");
            string str2 = LocationServices.GetLang("MotionCardLoadConfigError");
            string str3 = LocationServices.GetLang("MotionCardInitError");

            try
            {
                int nCardId = DmcEtherCatCard.Instance().CardId;
                if (nCardId < 0)
                {
                    BEnable = false;

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionInit, CardIndex.ToString(), string.Format(str1, "DMC-E3032"));

                    return false;
                }
                _nCardId = (ushort)nCardId;

                short ret = LTDMC.dmc_download_configfile(_nCardId, "DMCECAT.ini");
                if (ret == 0)
                {
                    BEnable = true;
                    return true;
                }
                else
                {
                    BEnable = false;

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionInit, CardIndex.ToString(), string.Format(str2, "DMC-E3032", ret));
                    return false;
                }
            }
            catch (Exception e)
            {
                BEnable = false;
                MessageBox.Show(e.Message, str3, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// 关闭轴卡
        /// </summary>
        /// <returns></returns>
        public override bool DeInit()
        {
            short ret = LTDMC.dmc_board_close();
            if (ret == 0)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardDeInitError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionDeInit, CardIndex.ToString(), string.Format(str1, "DMC-E3032", ret));
                }
                return false;
            }
        }

        /// <summary>
        /// 给予使能
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool ServoOn(int nAxisNo)
        {
            LTDMC.nmc_clear_axis_errcode(_nCardId, (ushort)nAxisNo);
            short ret = LTDMC.nmc_set_axis_enable(_nCardId, (ushort)nAxisNo);
            if (ret == 0)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardServoOnError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionServoOn, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }
        }

        /// <summary>
        /// 断开使能
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool ServoOff(int nAxisNo)
        {
            short ret = LTDMC.nmc_set_axis_disable(_nCardId, (ushort)nAxisNo);
            if (ret == 0)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardServoOffError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionServoOff, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }
        }

        /// <summary>
        /// 读取伺服使能状态
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool GetServoState(int nAxisNo)
        {
            //EtherCAT总线轴状态
            //0:未启动状态
            //1:启动禁止状态
            //2:准备启动状态
            //3:启动状态
            //4:操作使能状态
            //5:停止状态
            //6:错误触发状态
            //7:错误状态
            ushort nAxisMachineState = 0;
            short ret = LTDMC.nmc_get_axis_state_machine(_nCardId, (ushort)nAxisNo, ref nAxisMachineState);
            if (ret != 0)
                return false;

            return (nAxisMachineState == 4);
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">回原点参数, 对于8254，此参数代表回原点的方向</param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nParam)
        {
            ushort nHomeMode = 0;
            double dVelLow = 0;
            double dVelHigh = 0;
            double dAccTime = 0;
            double dDecTime = 0;
            double dOffsetPos = 0;

            short ret = LTDMC.nmc_get_home_profile(_nCardId, (ushort)nAxisNo,
                        ref nHomeMode, ref dVelLow, ref dVelHigh,
                        ref dAccTime, ref dDecTime, ref dOffsetPos);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardGetHomeError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            //自定义回原点方式
            if (nParam > (int)HomeMode.BusBase)
            {
                nHomeMode = (ushort)(nParam - (int)HomeMode.BusBase);
            }


            // 设置HOME参数
            //nHomeMode = 11;// (ushort)nParam;
            ret = LTDMC.nmc_set_home_profile(_nCardId, (ushort)nAxisNo, nHomeMode, dVelLow, dVelHigh, dAccTime, dDecTime, dOffsetPos);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardSetHomeError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }


            // 开始回原点
            ret = LTDMC.nmc_home_move(_nCardId, (ushort)nAxisNo);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardHomeError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            //清除停止原因
            LTDMC.dmc_clear_stop_reason(_nCardId, (ushort)nAxisNo);

            return true;
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nParam"></param>
        /// <param name="vm"></param>
        /// <param name="vo"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="offset"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nParam, double vm, double vo, double acc, double dec, double offset = 0, double sFac = 0)
        {
            // 设置HOME参数
            ushort nHomeMode = (ushort)nParam;
            double dVelLow = vo;
            double dVelHigh = vm;
            double dAccTime = acc;
            double dDecTime = dec;
            double dOffsetPos = offset;

            short ret = LTDMC.nmc_get_home_profile(_nCardId, (ushort)nAxisNo,
                        ref nHomeMode, ref dVelLow, ref dVelHigh,
                        ref dAccTime, ref dDecTime, ref dOffsetPos);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardGetHomeError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            //自定义回原点方式
            if (nParam > (int)HomeMode.BusBase)
            {
                nHomeMode = (ushort)(nParam - (int)HomeMode.BusBase);
            }

            dVelLow = vo;
            dVelHigh = vm;
            dAccTime = acc;
            dDecTime = dec;
            dOffsetPos = offset;

            ret = LTDMC.nmc_set_home_profile(_nCardId, (ushort)nAxisNo, nHomeMode, dVelLow, dVelHigh, dAccTime, dDecTime, dOffsetPos);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardSetHomeError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            // 开始回原点
            ret = LTDMC.nmc_home_move(_nCardId, (ushort)nAxisNo);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardHomeError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            //清除停止原因
            LTDMC.dmc_clear_stop_reason(_nCardId, (ushort)nAxisNo);

            return true;
        }

        /// <summary>
        /// 以绝对位置移动
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nPos">位置</param>
        /// <param name="nSpeed">速度</param>
        /// <returns></returns>
        public override bool AbsMove(int nAxisNo, int nPos, int nSpeed)
        {
            // 读取速度
            double dVelMin = 0;
            double dVelMax = 0;
            double dAccTime = 0;
            double dDecTime = 0;
            double dVelStop = 0;
            short ret = LTDMC.dmc_get_profile_unit(_nCardId, (ushort)nAxisNo,
                ref dVelMin, ref dVelMax, ref dAccTime, ref dDecTime, ref dVelStop);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardGetMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            // 设置速度
            dVelMin = 0;
            dVelMax = nSpeed;
            ret = LTDMC.dmc_set_profile_unit(_nCardId, (ushort)nAxisNo,
                dVelMin, dVelMax, dAccTime, dDecTime, dVelStop);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardSetMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));

                }
                return false;
            }



            // 执行运动
            ushort nPosiMode = 1;//运动模式0：相对坐标模式，1：绝对坐标模式
            ret = LTDMC.dmc_pmove_unit(_nCardId, (ushort)nAxisNo, nPos, nPosiMode);

            //清除停止原因
            LTDMC.dmc_clear_stop_reason(_nCardId, (ushort)nAxisNo);

            if (ret == 0)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 以绝对位置移动
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
        public override bool AbsMove(int nAxisNo, double fPos, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            // 读取速度
            double dVelMin = vs;
            double dVelMax = vm;
            double dAccTime = acc;
            double dDecTime = dec;
            double dVelStop = vm;

            short ret = LTDMC.dmc_set_profile_unit(_nCardId, (ushort)nAxisNo,
                dVelMin, dVelMax, dAccTime, dDecTime, dVelStop);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardGetMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            //设置单轴速度曲线 S段参数值
            ret = LTDMC.dmc_set_s_profile(_nCardId, (ushort)nAxisNo, 0, sFac);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardSetMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            // 执行运动
            ushort nPosiMode = 1;//运动模式0：相对坐标模式，1：绝对坐标模式
            ret = LTDMC.dmc_pmove_unit(_nCardId, (ushort)nAxisNo, fPos, nPosiMode);

            //清除停止原因
            LTDMC.dmc_clear_stop_reason(_nCardId, (ushort)nAxisNo);

            if (ret == 0)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }
        }

        /// <summary>
        /// 相对位置移动
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nPos">位置</param>
        /// <param name="nSpeed">速度</param>
        /// <returns></returns>
        public override bool RelativeMove(int nAxisNo, int nPos, int nSpeed)
        {
            // 读取速度
            double dVelMin = 0;
            double dVelMax = 0;
            double dAccTime = 0;
            double dDecTime = 0;
            double dVelStop = 0;
            short ret = LTDMC.dmc_get_profile_unit(_nCardId, (ushort)nAxisNo,
                ref dVelMin, ref dVelMax, ref dAccTime, ref dDecTime, ref dVelStop);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardGetMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            // 设置速度
            dVelMin = 0;
            dVelMax = nSpeed;
            ret = LTDMC.dmc_set_profile_unit(_nCardId, (ushort)nAxisNo,
                dVelMin, dVelMax, dAccTime, dDecTime, dVelStop);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardSetMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            // 执行运动
            ushort nPosiMode = 0;//运动模式0：相对坐标模式，1：绝对坐标模式
            ret = LTDMC.dmc_pmove_unit(_nCardId, (ushort)nAxisNo, nPos, nPosiMode);
            //清除停止原因
            LTDMC.dmc_clear_stop_reason(_nCardId, (ushort)nAxisNo);
            if (ret == 0)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }
        }

        /// <summary>
        /// 相对位置移动
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
        public override bool RelativeMove(int nAxisNo, double fOffset, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            // 读取速度
            double dVelMin = vs;
            double dVelMax = vm;
            double dAccTime = acc;
            double dDecTime = dec;
            double dVelStop = vm;

            short ret = LTDMC.dmc_set_profile_unit(_nCardId, (ushort)nAxisNo,
                dVelMin, dVelMax, dAccTime, dDecTime, dVelStop);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardGetMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            //设置单轴速度曲线 S段参数值
            ret = LTDMC.dmc_set_s_profile(_nCardId, (ushort)nAxisNo, 0, sFac);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardSetMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            // 执行运动
            ushort nPosiMode = 0;//运动模式0：相对坐标模式，1：绝对坐标模式
            ret = LTDMC.dmc_pmove_unit(_nCardId, (ushort)nAxisNo, fOffset, nPosiMode);

            //清除停止原因
            LTDMC.dmc_clear_stop_reason(_nCardId, (ushort)nAxisNo);

            if (ret == 0)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }
        }
        /// <summary>
        /// JOG运动
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="bPositive">方向</param>
        /// <param name="bStart">开始标志</param>
        /// <param name="nSpeed">速度</param>
        /// <returns></returns>
        public override bool JogMove(int nAxisNo, bool bPositive, int bStart, int nSpeed)
        {
            // 读取速度
            double dVelMin = 0;
            double dVelMax = 0;
            double dAccTime = 0;
            double dDecTime = 0;
            double dVelStop = 0;
            short ret = LTDMC.dmc_get_profile_unit(_nCardId, (ushort)nAxisNo,
                ref dVelMin, ref dVelMax, ref dAccTime, ref dDecTime, ref dVelStop);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardGetMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionJog, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            // 设置速度
            dVelMin = 0;
            dVelMax = nSpeed;
            ret = LTDMC.dmc_set_profile_unit(_nCardId, (ushort)nAxisNo,
                dVelMin, dVelMax, dAccTime, dDecTime, dVelStop);
            if (ret != 0)
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardSetMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionJog, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }

            // 执行运动
            ushort nDir = (ushort)(bPositive ? 1 : 0);//0：负方向，1：正方向
            ret = LTDMC.dmc_vmove(_nCardId, (ushort)nAxisNo, nDir);
            //清除停止原因
            LTDMC.dmc_clear_stop_reason(_nCardId, (ushort)nAxisNo);
            if (ret == 0)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardMoveError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionJog, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }
        }

        /// <summary>
        /// 轴正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool StopAxis(int nAxisNo)
        {
            ushort nStopMode = 0; //制动方式，0：减速停止，1：紧急停止
            short ret = LTDMC.dmc_stop(_nCardId, (ushort)nAxisNo, nStopMode);
            if (ret == 0)
            {
                return true;

            }
            else
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardAxisStopError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionStop, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));
                }
                return false;
            }
        }

        /// <summary>
        /// 急停
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool StopEmg(int nAxisNo)
        {
            ushort nStopMode = 1; //制动方式，0：减速停止，1：紧急停止
            short ret = LTDMC.dmc_stop(_nCardId, (ushort)nAxisNo, nStopMode);
            if (ret == 0)
            {
                return true;

            }
            else
            {
                if (BEnable)
                {
                    string str1 = LocationServices.GetLang("MotionCardAxisStopError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionEmgStop, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 获取轴卡运动状态
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetMotionState(int nAxisNo)
        {
            // 0:轴正在运行，1：轴已停止
            short ret = LTDMC.dmc_check_done(_nCardId, (ushort)nAxisNo);
            return ret == 0 ? 1 : 0;
        }

        /// <summary>
        /// 获取轴卡运动IO信号
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetMotionIoState(int nAxisNo)
        {
            // 雷赛轴IO状态
            // 0:保留
            // 1:EL+
            // 2:EL-
            // 3:EMG
            // 4:ORG
            // 6:SL+
            // 7:SL-
            uint nIoStatus = LTDMC.dmc_axis_io_status(_nCardId, (ushort)nAxisNo);

            // 8254 motion io table
            // |-bit0-|--1--|--2--|--3--|--4--|--5--|--6--|--7--|--8--|...|--11--|--12--|
            // |-ALM--|-PEL-|-MEL-|-ORG-|-EMG-|-EZ--|-INP-|-SVO-|-RDY-|...|-SPEL-|-SMEL-|
            long nStdIo = 0;//以8254的IO状态为标准
            if ((nIoStatus & (0x01 << 3)) != 0)
                nStdIo |= (0x01 << 0);
            if ((nIoStatus & (0x01 << 1)) != 0)
                nStdIo |= (0x01 << 1);
            if ((nIoStatus & (0x01 << 2)) != 0)
                nStdIo |= (0x01 << 2);
            if ((nIoStatus & (0x01 << 4)) != 0)
                nStdIo |= (0x01 << 3);
            if ((nIoStatus & (0x01 << 3)) != 0)
                nStdIo |= (0x01 << 4);
            if ((nIoStatus & (0x01 << 6)) != 0)
                nStdIo |= (0x01 << 11);
            if ((nIoStatus & (0x01 << 7)) != 0)
                nStdIo |= (0x01 << 12);

            if (GetServoState(nAxisNo))
                nStdIo |= (0x01 << 7);

            // 0:轴正在运行，1：轴已停止
            short nMoveDone = LTDMC.dmc_check_done(_nCardId, (ushort)nAxisNo);
            if (nMoveDone == 1)
            {
                nStdIo |= (0x01 << 6);
            }

            return nStdIo;
        }

        /// <summary>
        /// 获取轴的当前位置
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override double GetAixsPos(int nAxisNo)
        {
            double dPos = 0;
            //short ret = LTDMC.dmc_get_position_unit(_nCardId, (ushort)nAxisNo, ref dPos);
            short ret = LTDMC.dmc_get_encoder_unit(_nCardId, (ushort)nAxisNo, ref dPos);
            if (ret != 0)
            {
                return -1;
            }

            return (long)dPos;
        }

        /// <summary>
        /// 轴是否正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns>0:正常停止, -1:未到位 其它:急停,报警等</returns>
        public override int IsAxisNormalStop(int nAxisNo)
        {
            // 0:轴正在运行，1：轴已停止
            short nMoveDone = LTDMC.dmc_check_done(_nCardId, (ushort)nAxisNo);
            if (nMoveDone == 0)
                return -1;


            // 0:正常停止
            // 1:保留
            // 2:保留
            // 3:LTC外部触发立即停止
            // 4:EMG立即停止
            // 5:硬件正限位，立即停止
            // 6:硬件负限位，立即停止
            // 7:硬件正限位，减速停止
            // 8:硬件负限位，减速停止
            // 9:软件正限位，立即停止
            // 10:软件负限位，立即停止
            // 11:软件正限位，减速停止
            // 12:软件负限位，减速停止
            // 13:命令立即停止
            // 14:命令减速停止
            // 15:其他原因，立即停止
            // 16:其他原因，减速停止
            // 17:未知原因，立即停止
            // 18:未知原因，减速停止
            // 19:保留
            int nStopReason = 0;
            short ret = LTDMC.dmc_get_stop_reason(_nCardId, (ushort)nAxisNo, ref nStopReason);
            if (ret != 0)
                return -1;

            if (nStopReason == 0
                || nStopReason == 13
                || nStopReason == 14)//正常停止
                return 0;

            int nStopCode;
            if (!GetServoState(nAxisNo))// servo off
            {
                nStopCode = 3;
            }
            else
            {
                if (nStopReason == 4)//急停
                    nStopCode = 1;
                else if (nStopReason == 5
                    || nStopReason == 7
                    || nStopReason == 9
                    || nStopReason == 11)//正限位
                    nStopCode = 4;
                else if (nStopReason == 6
                    || nStopReason == 8
                    || nStopReason == 10
                    || nStopReason == 12)//负限位
                    nStopCode = 5;
                else
                {
                    nStopCode = 2;// 其他原因
                }

            }

            return (nStopCode + 10);
        }

        /// <summary>
        /// 判断轴是否到位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nInPosError">到位误差</param>
        /// <returns></returns>
        public override int IsAxisInPos(int nAxisNo, int nInPosError = 1000)
        {
            int nRet = IsAxisNormalStop(nAxisNo);
            //if (nRet == 0)
            //{
            //    double dEncoderPosition = 0;
            //    double dPosition = 0;
            //    short ret = LTDMC.dmc_get_encoder_unit(_nCardId, (ushort)nAxisNo, ref dEncoderPosition);
            //    if (ret != 0)
            //        return -1;
            //    ret = LTDMC.dmc_get_position_unit(_nCardId, (ushort)nAxisNo, ref dPosition);
            //    if (ret != 0)
            //        return -1;

            //    if (Math.Abs(dPosition - dEncoderPosition) > nInPosError)
            //        return 6;  //轴停止后位置超限
            //}
            return nRet;
        }

        /// <summary>
        /// 位置清零
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool SetPosZero(int nAxisNo)
        {
            short ret = LTDMC.dmc_set_position_unit(_nCardId, (ushort)nAxisNo, 0);
            if (ret != 0)
                return false;

            ret = LTDMC.dmc_set_encoder_unit(_nCardId, (ushort)nAxisNo, 0);
            if (ret != 0)
                return false;

            return true;
        }


        /// <summary>
        /// 设置单轴的某一运动参数
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">参数:1:加速度 2:减速度 3:起跳速度 4:结束速度(凌华卡) 5:平滑时间(固高卡S曲线) 其它：自定义扩展</param>
        /// <param name="nData">参数值</param>
        /// <returns></returns>
        public override bool SetAxisParam(int nAxisNo, int nParam, int nData)
        {
            // 读取速度
            double dVelMin = 0;
            double dVelMax = 0;
            double dAccTime = 0;
            double dDecTime = 0;
            double dVelStop = 0;
            short ret = LTDMC.dmc_get_profile_unit(_nCardId, (ushort)nAxisNo,
                ref dVelMin, ref dVelMax, ref dAccTime, ref dDecTime, ref dVelStop);
            if (ret != 0)
            {
                string str1 = LocationServices.GetLang("MotionCardGetMoveError");

                RunInforManager.GetInstance().Error(ErrorType.ErrMotionSetParam, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));

                return false;
            }

            switch (nParam)
            {
                case 1:
                    dAccTime = nData;
                    break;
                case 2:
                    dDecTime = nData;
                    break;

                case 3:
                    dVelMin = nData;
                    break;

                case 4:
                    dVelStop = nData;
                    break;

                default:
                    return false;
            }

            ret = LTDMC.dmc_set_profile_unit(_nCardId, (ushort)nAxisNo, dVelMin, dVelMax, dAccTime, dDecTime, dVelStop);
            if (ret != 0)
            {
                string str1 = LocationServices.GetLang("MotionCardSetMoveError");

                RunInforManager.GetInstance().Error(ErrorType.ErrMotionSetParam, CardIndex.ToString(), string.Format(str1, "DMC-E3032", nAxisNo, ret));

                return false;
            }

            return true;
        }

        /// <summary>
        /// 速度模式旋转轴
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool VelocityMove(int nAxisNo, int nSpeed)
        {
            bool bDir = nSpeed >= 0;
            return JogMove(_nCardId, bDir, 0, Math.Abs(nSpeed));
        }

        /// <summary>
        ///此函数8254板卡不提供不使用,回原点内部已经封装好过程处理 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsHomeNormalStop(int nAxisNo)
        {
            return IsAxisNormalStop(nAxisNo);
        }

        /// <summary>
        /// 清除报警
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool ClearError(int nAxisNo)
        {
            ushort errorcode = 0;

            LTDMC.nmc_get_axis_errcode(_nCardId, (ushort)nAxisNo, ref errorcode);

            if (errorcode != 0)
            {
                LTDMC.nmc_clear_axis_errcode(_nCardId, (ushort)nAxisNo);
            }

            errorcode = 0;
            LTDMC.nmc_get_errcode(_nCardId, 2, ref errorcode);

            if (errorcode != 0)
            {
                LTDMC.nmc_clear_errcode(_nCardId, 2);
            }

            return true;
        }
    }
}
