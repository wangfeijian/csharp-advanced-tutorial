using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using AutoMationFrameWork.Stations;
using Communicate;
using Newtonsoft.Json;

namespace AutoMationFramework.Stations
{
    class StationRotatingDisk : StationEx
    {
        private TcpLink m_tcpScanHsgCode; //物料码
        private TcpLink m_Meslink;
        private static int[] m_nPlateHasIndex = new[] { 1, 6, 5, 4, 3, 2 };
        private static int[] m_nPlateCylIndex = new[] { 1, 2, 3, 4, 5, 0 };
        private int m_okCount = 0;
        private int m_ngCount = 0;

        //配置文件属性
        private string User;
        private string LoginId;
        private string Client;
        private string ClientId;
        private string Section;
        private string SectionId;
        private string LineName;
        private string ShopOrder;
        private string ProjectName;
        private string ProductId;
        private string ShopOrderId;
        private string ProductName;
        private string Id;
        private string ProjectId;
        private string StationName;
        private string StationName2;
        private string SelectStationName;
        private string StationId;
        private string StationId2;
        private string FixtureNo;

        //连接字符串
        private string MainUrl;
        private string LoginUrl;
        private string GetLine;
        private string GetOrder;
        private string GetStation;
        private string Start;
        private string TestData;
        private string PassComplete;
        private string FailComplete;


        // 寄存器数组,通知转动到位
        private SysBitReg[] m_sysBitRegs =
        {
            SysBitReg.xxx,
            SysBitReg.转盘站通知视觉检测一站转动到位,
            SysBitReg.转盘站通知视觉检测二站转动到位,
            SysBitReg.转盘站通知镭射一站站转动到位,
            SysBitReg.转盘站通知镭射二站站转动到位,
            SysBitReg.转盘站通知镭射三站站转动到位,
        };

        // 寄存器数组 接收工作完成
        private SysBitReg[] m_sysBitRegs1 =
        {
            SysBitReg.xxx,
            SysBitReg.视觉检测一站工作完成,
            SysBitReg.视觉检测二站工作完成,
            SysBitReg.镭射一站工作完成,
            SysBitReg.镭射二站工作完成,
            SysBitReg.镭射三站工作完成,
        };

        // 载具物料有无检测数组
        string[] m_strPlateHasSns =
        {
            "载具有料检测1",
            "载具有料检测2",
            "载具有料检测3",
            "载具有料检测4",
            "载具有料检测5",
            "载具有料检测6",
       };

        // 固定位置气缸数组
        string[] m_strPlateCylName =
        {
            "定位气缸1",
            "定位气缸2",
            "定位气缸3",
            "定位气缸4",
            "定位气缸5",
            "定位气缸6",
       };

        string[] m_strPlateBackCylName =
        {
            "定位气缸1",
            "定位气缸6",
            "定位气缸5",
            "定位气缸4",
            "定位气缸3",
            "定位气缸2",
        };

        bool m_bWaitLoading = true;

        /// <summary>
        /// 构造函数，需要设置站位当前的IO输入，IO输出，轴方向及轴名称，以显示在手动页面方便操作
        /// </summary>
        /// <param name="strName"></param>
        public StationRotatingDisk(string strName) : base(strName)
        {
            IoIn = new string[]
            {
                "载具有料检测1",
                "产品切换气缸1伸",
                "产品切换气缸1缩",
                "定位气缸1伸",
                "定位气缸1缩",
                "载具有料检测2",
                "产品切换气缸2伸",
                "产品切换气缸2缩",
                "定位气缸2伸",
                "定位气缸2缩",
                "载具有料检测3",
                "产品切换气缸3伸",
                "产品切换气缸3缩",
                "定位气缸3伸",
                "定位气缸3缩",
                "载具有料检测4",
                "产品切换气缸4伸",
                "产品切换气缸4缩",
                "定位气缸4伸",
                "定位气缸4缩",
                "载具有料检测5",
                "产品切换气缸5伸",
                "产品切换气缸5缩",
                "定位气缸5伸",
                "定位气缸5缩",
                "载具有料检测6",
                "产品切换气缸6伸",
                "产品切换气缸6缩",
                "定位气缸6伸",
                "定位气缸6缩",
                "圆孔定位气缸伸",
                "圆孔定位气缸缩",
                "DD马达转动到位",
                "DD马达原点位置",
                "DD马达报警1" ,
                "双手启动1" ,
                "双手启动2" ,
            };

            IoOut = new string[]
            {
                "产品切换气缸伸",
                "产品切换气缸缩",
                "定位气缸1伸",
                "定位气缸2伸",
                "定位气缸3伸",
                "定位气缸4伸",
                "定位气缸5伸",
                "定位气缸6伸",
                "圆孔定位气缸伸",
                "圆孔定位气缸缩",
                "DD马达报警清除",
                "DD马达原点信号",
                "DD马达启动" ,
                "DD马达急停" ,
                "测量完成信号",
                "产品测试NG"
            };

            m_cylinders = new string[]
            {
                "产品切换气缸",
                "定位气缸1",
                "定位气缸2",
                "定位气缸3",
                "定位气缸4",
                "定位气缸5",
                "定位气缸6",
                "圆孔定位气缸"
            };


            // 获得扫码网口对象

           
        }

    }
}
