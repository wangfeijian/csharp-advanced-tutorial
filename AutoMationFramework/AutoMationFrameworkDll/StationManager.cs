/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-06                               *
*                                                                    *
*           ModifyTime:     2021-08-06                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Station Manager class                    *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMationFrameworkSystemDll;

namespace AutoMationFrameworkDll
{
    public class StationManager:SingletonPattern<StationManager>
    {
        /// <summary>当前是否在自动运行状态</summary>
        /// <returns></returns>
        public bool IsAutoRunning()
        {
            return this.m_nCurState != StationState.STATE_MANUAL;
        }

        /// <summary>计数减一</summary>
        public void DecreaseOne()
        {
            lock (StationMgr.m_lockCount)
            {
                --this.m_nShowMessageCount;
                // ISSUE: reference to a compiler-generated field
                if (this.OnShowMessageCountChanged == null)
                    return;
                // ISSUE: reference to a compiler-generated field
                this.OnShowMessageCountChanged(this.m_nShowMessageCount);
            }
        }
        /// <summary>弹窗计数加一</summary>
        public void IncreaseOne()
        {
            lock (StationMgr.m_lockCount)
            {
                ++this.m_nShowMessageCount;
                // ISSUE: reference to a compiler-generated field
                if (this.OnShowMessageCountChanged == null)
                    return;
                // ISSUE: reference to a compiler-generated field
                this.OnShowMessageCountChanged(this.m_nShowMessageCount);
            }
        }

        /// <summary>当前是否处于急停模式</summary>
        /// <returns></returns>
        public bool IsEmg()
        {
            return this.m_nCurState == StationState.STATE_EMG;
        }

        /// <summary>判断一个线程是在自动状态还是手动状态</summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public bool IsAutoThread(Thread thread)
        {
            foreach (StationBase stationBase in this.m_lsStation)
            {
                if (thread == stationBase.m_AutoThread)
                    return true;
            }
            return false;
        }

        /// <summary>判断一个线程是在自动状态还是手动状态</summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public bool IsManaualThread(Thread thread)
        {
            foreach (StationBase stationBase in this.m_lsStation)
            {
                if (thread == stationBase.m_ManualThread)
                    return true;
            }
            return false;
        }

        /// <summary>急停所有站位</summary>
        /// <returns></returns>
        public bool EmgStopAllStation()
        {
            if (this.m_nCurState == StationState.STATE_EMG || this.m_nCurState == StationState.STATE_MANUAL)
                return false;
            if ((uint)SingletonTemplate<LanguageMgr>.GetInstance().LanguageID > 0U)
                this.ShowLog("Alarm and emergency stop!!!", CommonTool.LogLevel.Error);
            else
                this.ShowLog("报警急停!!!", CommonTool.LogLevel.Error);
            this.ChangeState(StationState.STATE_EMG);
            foreach (StationBase stationBase in this.m_lsStation)
            {
                stationBase.SwitchState(StationState.STATE_EMG);
                stationBase.EmgStop();
            }
            foreach (StationBase stationBase in this.m_lsStation)
                stationBase.StopManualRun();
            return true;
        }
    }
}
