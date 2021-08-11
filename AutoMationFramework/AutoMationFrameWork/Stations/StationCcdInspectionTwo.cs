using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HalconDotNet;
using Communicate;
using System.Reflection;
using System.IO;
using AutoMationFrameWork.Stations;

namespace AutoMationFramework.Stations
{
    /// <summary>
    /// 视觉检测二站,负责对键盘的短边进行测量 
    /// </summary>
    class StationCcdInspectionTwo : StationEx
    {
        //////定义类变量

        //当前对角点及拍照点数据
        HTuple m_AngPos_X = new HTuple(), m_AngPos_Y = new HTuple();
        HTuple m_MeasurPos_X = new HTuple(), m_MeasurPos_Y = new HTuple();

        //测量后的Y1到Y10的点位坐标 m_ResultPos_X
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

        enum Point
        {
            安全点 = 1,
            检测拍照点,
            标定点,
            定位对角点1,
            定位对角点2,
            上边检测点,
            下边检测点,
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

            test1,
            test2,
        }

        private TcpLink m_tcpVision;

        /// <summary>
        /// 构造函数，需要设置站位当前的IO输入，IO输出，轴方向及轴名称，以显示在手动页面方便操作 
        /// </summary>
        /// <param name="strName"></param>
        public StationCcdInspectionTwo(string strName) : base(strName)
        {
            IoIn = new string[]
            {
                "检测二站顶升气缸伸",
                "检测二站顶升气缸缩",
                "检测二站三轴气缸伸",
                "检测二站三轴气缸缩",
            };

            IoOut = new string[]
            {
                "检测二站顶升气缸伸",
                "检测二站顶升气缸缩",
                "检测二站三轴气缸伸",
                "检测二站三轴气缸缩",
                "检测二站同轴光源",
                "检测二站环形光源",
            };

            m_cylinders = new string[]
            {
                "检测二站顶升气缸",
                "检测二站三轴气缸",
            };

            //配置手动调试时的伺服的运动方向，如果发现界面上的方向和实际的方向相反，则把对应的轴的方向改为false
            //InverseAxisPositiveByAxisNo(AxisZ);

            //给轴重命名
            //RenameAxisName(4, "X1");
            //RenameAxisName(5, "Y1");
            //RenameAxisName(6, "Z1");
            //RenameAxisName(7, "U1");

            m_tcpVision = TcpManager.GetInstance().GetTcpLink(5);
        }
    }
}

