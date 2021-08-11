using System;
using System.Diagnostics;
using Communicate;
using System.IO;
using System.Linq;
using System.Threading;
using AutoMationFrameWork.Stations;

namespace AutoMationFramework.Stations
{
    /// <summary>
    /// 镭射三站,负责对键盘长边的厚度进行测量
    /// </summary>
    class StationLaserThree : StationEx
    {

        //private static int m_ArrayLength = 13;
        double[] m_FrontHeightOne=new double[10];
        double[] m_FrontHeightTwo = new double[10];
        double[] m_RearHeightOne=new double[10];
        double[] m_RearHeightTwo = new double[10];
        private int m_nLoopCount = 0;
        private double m_fAxisXSearchPos = 0;


        public enum Point
        {
            安全点 = 1,
            长边测量始点,
            临时点,
            测量基准点,
        }

        /// <summary>
        /// 产品上镭射头的高度
        /// </summary>
        private double[] m_fUpHeightDoubles;

        /// <summary>
        /// 产品下镭射头的高度
        /// </summary>
        private double[] m_fDownHeightDoubles;

        /// <summary>
        /// 产品算平面度的坐标
        /// </summary>
        private double[,] m_fMulDoubles;

        /// <summary>
        /// 长短产品初始测量位置
        /// </summary>
        private double[,] m_xPosInitData;
        private double[,] m_yPosInitData;
        private double[,] m_xPosAfineData;
        private double[,] m_yPosAfineData;
        private double[,] m_fThicknessOffset;
        private double[,] m_fHeightOffset;


        /// <summary>
        /// 构造函数，需要设置站位当前的IO输入，IO输出，轴方向及轴名称，以显示在手动页面方便操作
        /// </summary>
        /// <param name="strName"></param>
        public StationLaserThree(string strName) : base(strName)
        {
            //获得镭射三站的ip
        }

    }
}

