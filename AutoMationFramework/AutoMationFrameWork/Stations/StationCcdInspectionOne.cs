using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HalconDotNet;
using System.IO;
using System.Reflection;
using AutoMationFrameworkDll;
using AutoMationFrameworkSystemDll;
using AutoMationFrameWork.Stations;
using Communicate;

namespace AutoMationFramework.Stations
{
    /// <summary>
    /// 视觉检测一站,负责对键盘的长边进行测量 
    /// </summary>
    class StationCcdInspectionOne : StationEx
    {
        //////定义类变量

        //当前对角点及当前拍照点的数据
        HTuple m_AngPos_X = new HTuple(), m_AngPos_Y = new HTuple();
        HTuple m_MeasurPos_X = new HTuple(), m_MeasurPos_Y = new HTuple();

        //测量后的X1到X6的点位坐标 m_ResultPos_X
        HTuple m_ResultPos_X = new HTuple(), m_ResultPos_Y = new HTuple();

        //产品类型
        private string TypeKeyboard = "";

        //图纸上的拍照点
        HTuple m_ModelKeybMeasurePosX = new HTuple();
        HTuple m_ModelKeybMeasurePosY = new HTuple();

        //图纸角度与键盘角度
        HTuple m_ModelAngle, m_KeybAngle;

        //运动轨迹选择
        private int m_VisMoveMinLocus = 0;

        //模式选择
        int m_SelectModel = 0;

        //正常运行次数
        private int RunNum = 0;

        private int n;
        public enum Point
        {
            安全点 = 1,
            检测拍照点,
            标定点,
            定位对角点1,
            定位对角点2,
            右边检测点,
            左边检测点,
            偏移检测起点,
            临时点,

            拍照点1,
            拍照点2,
            拍照点3,
            拍照点4,
            拍照点5,
            拍照点6,
            拍照点7,
            拍照点8,
            拍照点9,
            拍照点10,
            拍照点11,
            拍照点12,
            拍照点13,
            拍照点14,
            拍照点15,
            拍照点16,
        }

        private TcpLink m_tcpVision;

        /// <summary>
        /// 构造函数，需要设置站位当前的IO输入，IO输出，轴方向及轴名称，以显示在手动页面方便操作
        /// </summary>
        /// <param name="strName"></param>
        public StationCcdInspectionOne(string strName) : base(strName)
        {
            IoIn = new string[]
            {
                "检测一站顶升气缸伸",
                "检测一站顶升气缸缩",
                "检测一站三轴气缸伸",
                "检测一站三轴气缸缩",

            };

            IoOut = new string[]
            {
                "检测一站顶升气缸伸",
                "检测一站顶升气缸缩",
                "检测一站三轴气缸伸",
                "检测一站三轴气缸缩",
                "检测一站同轴光源",
                "检测一站环形光源",
            };

            // = new string[]
            //{
            //    "检测一站顶升气缸",
            //    "检测一站三轴气缸",
            //};

            //m_tcpVision = TcpMgr.GetInstance().GetTcpLink(4);
        }

        /// <summary>
        /// 站位初始化，用来添加伺服上电，打开网口，站位回原点等动作 
        /// </summary>
        public override void StationInit()
        {

            EnablePause(false);
            //连接视觉程序
            //m_tcpVision.Close();
            //Thread.Sleep(200);
            if (!m_tcpVision.Open())
            {
                Error(ErrorType.ErrVisionOpen, "视觉程序", "连接视觉程序失败");
                return;
            }

            RunNum = 0;

            n = 0;
            //return;
            //光源关闭
            SetDO("检测一站同轴光源", false);
            SetDO("检测一站环形光源", false);

            bool bRptTest = SystemManager.GetInstance().GetParamBool("RptTestEnable");

            //气缸缩回
            if (!bRptTest)
            {
                CylBack("检测一站顶升气缸");
                CylBack("检测一站三轴气缸");
            }

            //伺服上电
            AxisEnable(true);


            //回原点
            AxisHome();

            WaitTimeDelay(100);
            //m_dicPoint[(int)Point.临时点].x = 20;
            //m_dicPoint[(int)Point.临时点].y = 20;
            //AxisGoTo((int)Point.临时点);


            ////运动到安全点
            //AxisGoTo((int)Point.安全点);

            AxisGoTo((int)Point.拍照点1,10);

            //初始化完成
            SetBit(SysBitReg.视觉检测一站初始化完成, true);
            ShowLog("初始化完成");
        }
        /// <summary>
        /// 站位退出退程时调用，用来关闭伺服，关闭网口等动作
        /// </summary>
        public override void StationDeinit()
        {
            AxisStop();
            m_tcpVision.Close();

            //光源关闭
            SetDO("检测一站同轴光源", false);
            SetDO("检测一站环形光源", false);

            //气缸缩回
            //CylBack("检测一站顶升气缸", false);
            //CylBack("检测一站三轴气缸", false);

            //伺服上电
            AxisEnable(false);

        }

        /// <summary>
        /// 初始化时设置站位安全状态，用来设备初始化时各站位之间相互卡站，
        /// 控制各站位先后动作顺序用，比如流水线组装，肯定需要组装的Z轴升
        /// 起后，流水线才能动作这种情况
        /// </summary>
        public override void InitSecurityState()
        {
            SetBit(SysBitReg.视觉检测一站初始化完成, false);
            SetBit(SysBitReg.视觉检测一站工作完成, true);
            SetBit(SysBitReg.视觉检测一站工作中, false);
        }

        protected override void NormalRun()
        {
            bool bRptTest = SystemManager.GetInstance().GetParamBool("RptTestEnable");
            bool bGrrTest = SystemManager.GetInstance().GetParamBool("GrrTestEnable");
            
            //到安全点 
            AxisGoTo((int)Point.拍照点1);

            //气缸将产品推到位(水平) 
            if (StationManager.GetInstance().GetStation("转盘站").StationEnable)
            {
                WaitRegBit((int)SysBitReg.转盘站通知视觉检测一站转动到位, true);
                SetBit(SysBitReg.转盘站通知视觉检测一站转动到位, false);
            }
            else if(bGrrTest)
            {
                ShowMessage("请上料");
            }

            SetBit(SysBitReg.视觉检测一站工作中, true);
            SetBit(SysBitReg.视觉检测一站工作完成, false);

           // ProductData data = ProductMgr.GetInstance().Job.Get(2);
            //data.m_dtCCD1StartTime = DateTime.Now;


            WaitIo("产品切换气缸1伸", true);
            WaitIo("产品切换气缸2伸", true);
            WaitIo("产品切换气缸3伸", true);
            WaitIo("产品切换气缸4伸", true);
            WaitIo("产品切换气缸5伸", true);
            WaitIo("产品切换气缸6伸", true);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            CylOut("检测一站顶升气缸");

            if (GetParamBool("CCDThirdCylinderEnable"))
            {
                CylOut("检测一站三轴气缸");
            }

            //弹簧抖动
            Thread.Sleep(GetParamInt("CylDelayTime"));

            sw.Stop();

            ShowLog("CylOut Time:" + sw.ElapsedMilliseconds.ToString());

            if (bRptTest || bGrrTest)
            {
               // data.m_strBarCode = RunNum.ToString();
            }


            RunNum++;

            sw.Restart();

            //气缸动作准备放料
            if (!bRptTest)
            {
                CylBack("检测一站三轴气缸");
                CylBack("检测一站顶升气缸");
            }

            UpdateProgress(18);

            //data.m_dtCCD1EndTime = DateTime.Now;

            sw.Stop();

             ShowLog("CylBack Time:" + sw.ElapsedMilliseconds.ToString());

            //通知转盘站可以动作
            SetBit(SysBitReg.视觉检测一站工作完成, true);
            SetBit(SysBitReg.视觉检测一站工作中, false);

            
            
        }

        protected override void DryRun()
        {
            NormalRun();

        }

        /// <summary>
        /// 自动标定
        /// </summary>
        protected override void AutoCalib()
        {

            ShowLog("T1相机标定开始");

            ShowMessage("准备就绪，请手动放置标定板，按启动键开始标定！");

            //string strCalib = SystemManagerEx.GetInstance().CurrentCalib;

            //RunModeInfo info;
            //if (SystemManagerEx.GetInstance().m_dictCalibs.TryGetValue(strCalib, out info))
            //{
            //    MethodInfo method = GetType().GetMethod(info.m_strMethod);

            //    if (method != null)
            //    {
            //        method.Invoke(this, null);
            //    }
            //    else
            //    {
            //        ShowMessage("标定方法错误，请确认", true);
            //    }
            //}

            //WaitIo("启动", true);
            /*
            if (CalibMoveFlow())
            {
                //AutoCountPhotographModelPoint();
                this.StopRun();
                return;
            }
            else
            {
                if (MessageBox.Show("标定失败是否重新标定？", "提示", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    StationManager.GetInstance().StopRun();
                }
                return;
            }
            */

            //Calib();

            ShowMessage("标定完成");

        }

        protected override void GrrRun()
        {
            //SetBit(SysBitReg.转盘站通知视觉检测一站转动到位, false);

            //SetBit(SysBitReg.视觉检测一站工作中, true);
            //SetBit(SysBitReg.视觉检测一站工作完成, false);

            //Snap();

            ////通知转盘站可以动作
            //SetBit(SysBitReg.视觉检测一站工作完成, true);
            //SetBit(SysBitReg.视觉检测一站工作中, false);
            //string strGRR = SystemManagerEx.GetInstance().CurrentGrr;

            //RunModeInfo info;
            //if (SystemManagerEx.GetInstance().m_dictGrrs.TryGetValue(strGRR, out info))
            //{
            //    MethodInfo method = GetType().GetMethod(info.m_strMethod);

            //    if (method != null)
            //    {
            //        method.Invoke(this, null);
            //    }
            //    else
            //    {
            //        ShowMessage("GRR方法错误，请确认", true);
            //    }
            //}

        }

    }
}
