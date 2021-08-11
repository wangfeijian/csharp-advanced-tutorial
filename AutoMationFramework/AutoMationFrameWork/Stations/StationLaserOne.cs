using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using Communicate;
using HalconDotNet;
using System.Diagnostics;
using System.Windows.Forms;
using AutoMationFrameWork.Stations;
using MotionIO;

namespace AutoMationFramework.Stations
{
    /// <summary>
    /// 镭射一站,负责对键盘框架的厚度进行测量
    /// </summary>
    class StationLaserOne : StationEx
    {
        public enum Point
        {
            安全位 = 1,
            X轴1工作位,
            X轴2工作位,
            X轴3工作位,
            X轴4工作位,
            标定初始位,
            标定位,
            拍照位,

            //空跑点位
            X轴起始位,
            X轴终点位,
        }

        // ReSharper disable once InconsistentNaming

        private double m_fMarkAxisPosX = 0;
        private double m_fMarkAxisPosY = 0;

        HTuple m_hommatId = new HTuple();

        private int m_nLoopNum = 0;

        /// <summary>
        /// 长短产品的上laser高度
        /// </summary>
        private double[] m_lUpHeightDoubles = new double[98];

        /// <summary>
        /// 长短产品的上laser高度
        /// </summary>
        private double[] m_lDownHeightDoubles = new double[98];
        public double[] m_sDownHeightDoubles = new double[70];

        /// <summary>
        /// 长短产品算平面度的坐标
        /// </summary>
        private double[,] m_lMulDoubles = new double[98, 3];
        private double[,] m_sMulDoubles = new double[70, 3];

        /// <summary>
        /// 长短产品初始测量位置
        /// </summary>
        private double[,] m_lxPosInitData;
        private double[,] m_lyPosInitData;
        private double[,] m_sxPosInitData;
        private double[,] m_syPosInitData;

        private double[,] m_lxPosAfineData;
        private double[,] m_lyPosAfineData;
        private double[,] m_sxPosAfineData;
        private double[,] m_syPosAfineData;

        private double[,] m_lOffset;
        private double[,] m_sOffset;

        private double[,] m_lHighOffset;
        private double[,] m_sHighOffset;
        /// <summary>
        /// 起始位置
        /// </summary>
        private int[] m_startPosition;
        private int[] m_endPosition;
        private int[] m_position;

        /// <summary>
        /// 构造函数，需要设置站位当前的IO输入，IO输出，轴方向及轴名称，以显示在手动页面方便操作
        /// </summary>
        /// <param name="strName"></param>
        public StationLaserOne(string strName) : base(strName)
        {
            IoIn = new[]
            {
                "波形突变信号",
            };

            IoOut = new[]
            {
                "镭射一站环形光源",
                "镭射一站控制器一触发",
                "镭射一站控制器二触发",
            };

            RenameAxisName(4, "X1");
            RenameAxisName(5, "X2");
            RenameAxisName(6, "X3");
            RenameAxisName(7, "X4");
        }


    }
}
