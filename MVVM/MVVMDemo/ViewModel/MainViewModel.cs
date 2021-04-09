using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using System.Windows;
using System.Collections;

namespace MVVMDemo.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        //配置文件属性
        public string User{ get; set; }
        public string LoginId{ get; set; }
        public string Client{ get; set; }
        public string ClientId{ get; set; }
        public string Section{ get; set; }
        public string SectionId{ get; set; }
        public string LineName{ get; set; }
        public string ShopOrder{ get; set; }
        public string ProjectName{ get; set; }
        public string ProductId{ get; set; }
        public string ShopOrderId{ get; set; }
        public string ProductName{ get; set; }
        public string Id{ get; set; }
        public string ProjectId{ get; set; }
        public string StationName{ get; set; }
        public string Station2Name{ get; set; }
        public string SelectStationName{ get; set; }
        public string StationId{ get; set; }
        public string FixtureNo{ get; set; }

        //连接字符串
        public string MainUrl{ get; set; }
        public string LoginUrl{ get; set; }
        public string GetLine{ get; set; }
        public string GetOrder{ get; set; }
        public string GetStation{ get; set; }
        public string Start{ get; set; }
        public string TestData{ get; set; }
        public string PassComplete{ get; set; }
        public string FailComplete{ get; set; }
        public bool CheckOnline { get; set; } = false;

        IniHelper ini = new IniHelper();
        public List<MesDataStruct.TestData> TestDatas = new List<MesDataStruct.TestData>();

        public List<string> ClientList { get; set; }

        public List<MesDataStruct.ClientInfo> ClientInfo { get; set; }
        public List<string> LineList { get; set; }

        public List<MesDataStruct.OrderInfo> OrderList {get; set;}

        public MesDataStruct.OrderInfo OrderObj { get; set; }

        public List<MesDataStruct.StationInfo> StationList { get; set; }

        public ICommand GetLineCommand { get; set; }
        public ICommand GetOrderCommand { get; set; }
        public ICommand GetStationCommand { get; set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            InitVar();
            InitCommand();
        }

        private void InitVar()
        {
            ini.IniFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MesConfig.ini");
            MainUrl = ini.GetString("Url", "mainUrl", "test");
            LoginUrl = ini.GetString("Url", "loginUrl", "test");
            GetLine = ini.GetString("Url", "getLine", "test");
            GetOrder = ini.GetString("Url", "getOrder", "test");
            GetStation = ini.GetString("Url", "getStation", "test");
            Start = ini.GetString("Url", "start", "test");
            TestData = ini.GetString("Url", "testData", "test");
            PassComplete = ini.GetString("Url", "passComplete", "test");
            FailComplete = ini.GetString("Url", "failComplete", "test");
            StationName = ini.GetString("StationName", "station", "test");
            Station2Name = ini.GetString("StationName", "station2", "test");
            FixtureNo = ini.GetString("StationName", "machineNo", "test");
            LoginId = ini.GetString("UserInfo", "LOGIN_ID");
            Client = ini.GetString("UserInfo", "CLIENT");
            ClientId = ini.GetString("UserInfo", "CLIENT_ID");
            User = ini.GetString("UserInfo", "USER");
            CheckOnline = ini.GetBool("Url", "checked", false);

            ClientList= new List<string>();
            ClientList.Add(Client);
            LineList = new List<string>();
            OrderList = new List<MesDataStruct.OrderInfo>();
            StationList = new List<MesDataStruct.StationInfo>();

            ClientInfo = new List<MesDataStruct.ClientInfo>
            {
                new MesDataStruct.ClientInfo {Section = "1",SectionName = "测试"},
                new MesDataStruct.ClientInfo {Section = "2",SectionName = "组装"},
                new MesDataStruct.ClientInfo {Section = "3",SectionName = "包装"},
            };
        }

        private void InitCommand()
        {
            GetLineCommand = new RelayCommand(GetLineMethod);
            GetOrderCommand = new RelayCommand(GetOrderMethod);
            GetStationCommand = new RelayCommand(GetStationMethod);
        }

        private void GetLineMethod()
        {
            var lineObj = new MesDataStruct.GetLineParam
            {
                LOGIN_ID = LoginId,
                CLIENT_ID = ClientId,
                SECTION = SectionId
            };

            string lineUrl = JsonConvert.SerializeObject(lineObj);

            string url = MainUrl + GetLine + lineUrl;

            //连接mes方法，正常情况请打开

            string result = string.Empty;

            if (!CheckOnline)
                result = MesTool.doHttpPost(url, null);
            else
                //测试用字符串
                result = @"{""LINES"":[""C7-C01"",""C7-C02""],""RESULT"":""PASS"",""SERVICE_RUNTIME"":1}";

            if (result == "error")
            {
                MessageBox.Show("Mes连接不正常", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //获取mes返回json字符串，并反序列化为字典
            var returnStr = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

            string message = returnStr.ContainsKey("MESSAGE") ? returnStr["MESSAGE"].ToString() : "error";

            //如果mes返回FAIL，获取失败信息并显示到界面
            if (returnStr["RESULT"].ToString() == "FAIL")
            {
                MessageBox.Show(message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string lineFull = returnStr["LINES"].ToString();
            lineFull = lineFull.Substring(1, lineFull.Length - 2).Trim();
            string[] allLine = lineFull.Split(',');

            foreach (var str in allLine)
            {
                LineList.Add(str.Replace('\"', ' ').Trim());
            }

            ini.WriteString("UserInfo", "SECTION", Section);
            ini.WriteString("UserInfo", "SECTION_ID", SectionId);
        }

        private void GetOrderMethod()
        {
            var orderObj = new MesDataStruct.GetOrderParam
            {
                LOGIN_ID = LoginId,
                CLIENT_ID = ClientId,
                LINE = LineName
            };

            string orderUrl = JsonConvert.SerializeObject(orderObj);

            string url = MainUrl + GetOrder + orderUrl;

            string result = string.Empty;

            if (!CheckOnline)
                result = MesTool.doHttpPost(url, null);
            else
                //测试用字符串
                result = @"{""SERVICE_RUNTIME"":0,""SCHEDULING"":[{""PROJECT"":""GA20808"",""LOAD_ID"":-1,""END_DATE"":""2020-10-31T22:54:00"",""STATE"":7,""TEXT"":""XK_20200928 27日 晚班"",""SHOPORDER"":""XK_20200928"",""PRODUCT_ID"":102,""UPH"":300,""ACTUAL_QTY"":0,""SHOPORDER_ID"":83,""BEGIN_DATE"":""2020-09-27T22:54:00"",""PRODUCT"":""10000008-00"",""QTY"":60000,""SAP_SHOPORDER"":""XK_20200928"",""ID"":165,""PROJECT_ID"":63},{""PROJECT"":""GA20809"",""LOAD_ID"":-1,""END_DATE"":""2020-10-31T22:50:00"",""STATE"":7,""TEXT"":""DK_20200929 27日 晚班"",""SHOPORDER"":""DK_20200929"",""PRODUCT_ID"":103,""UPH"":300,""ACTUAL_QTY"":0,""SHOPORDER_ID"":84,""BEGIN_DATE"":""2020-09-27T22:50:00"",""PRODUCT"":""10000009-00"",""QTY"":50000,""SAP_SHOPORDER"":""DK_20200929"",""ID"":164,""PROJECT_ID"":62}],""RESULT"":""PASS""}";

            //获取mes返回json字符串，并反序列化为字典
            if (result == "error")
            {
                MessageBox.Show("Mes连接不正常", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //获取mes返回json字符串，并反序列化为字典
            var returnStr = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

            string message = returnStr.ContainsKey("MESSAGE") ? returnStr["MESSAGE"].ToString() : "error";

            //如果mes返回FAIL，获取失败信息并显示到界面
            if (returnStr["RESULT"].ToString() == "FAIL")
            {
                MessageBox.Show(message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //获取所有的工单json信息
            string orderFull = returnStr["SCHEDULING"].ToString();
            var ordersObj = JsonConvert.DeserializeObject(orderFull);

            

            foreach (var obj in (IEnumerable)ordersObj)
            {
                var orderDic = JsonConvert.DeserializeObject<MesDataStruct.OrderInfo>(JsonConvert.SerializeObject(obj));
                if (orderDic.STATE == "3" || orderDic.STATE == "7" || orderDic.STATE == "8")
                {
                    OrderList.Add(orderDic);
                }

            }

            ini.WriteString("UserInfo", "LINE_NAME", LineName);
        }

        private void GetStationMethod()
        {
            ShopOrder = OrderObj.SHOPORDER;
            ProjectName = OrderObj.PROJECT;
            ProductId = OrderObj.PRODUCT_ID;
            ShopOrderId = OrderObj.SHOPORDER_ID;
            ProductName = OrderObj.PRODUCT;
            Id = OrderObj.ID;
            ProjectId = OrderObj.PROJECT_ID;

            ini.WriteString("UserInfo", "SHOPORDER", ShopOrder);
            ini.WriteString("UserInfo", "PROJECT_NAME", ProjectName);
            ini.WriteString("UserInfo", "PRODUCT_ID", ProductId);
            ini.WriteString("UserInfo", "SHOPORDER_ID", ShopOrderId);
            ini.WriteString("UserInfo", "PRODUCT_NAME", ProductName);
            ini.WriteString("UserInfo", "ID", Id);
            ini.WriteString("UserInfo", "PROJECT_ID", ProjectId);


            var stationObj = new MesDataStruct.GetStationParam
            {
                LOGIN_ID = LoginId,
                CLIENT_ID = ClientId,
                SHOPORDER = ShopOrder
            };

            string stationUrl = JsonConvert.SerializeObject(stationObj);

            string url = MainUrl + GetStation + stationUrl;

            string result = string.Empty;

            if (!CheckOnline)
                result = MesTool.doHttpPost(url, null);
            else
                result = @"{""RESULT"":""PASS"",""STATIONS"":[{""completeMaxTime"":0.0,""completeMinTime"":0.0,""createName"":""3318418"",""createTime"":""2021-01-06T19:04:56"",""ctType"":null,""failCount"":3,""horizontalAxis"":90,""id"":1772,""isFind"":1,""isGatherTestdata"":1,""isInput"":1,""isRedoingCheck"":1,""isRepairCheck"":1,""isReworkSamping"":1,""lineAllCheckNum"":0,""maxQueuingNum"":0,""module"":{""authorityUsers"":null,""createName"":null,""createTime"":null,""id"":61,""inMultiple"":1,""name"":""过站新"",""orderNum"":0,""outMultiple"":1,""path"":""%IPAddressDir%AMF\\public\\过站新.rar"",""remark"":null,""type"":1,""updateName"":null,""updateTime"":null,""version"":""1""},""name"":""上挂(Ano_unloading)"",""nextIdFail"":-1,""nextIdPass"":1773,""nextIdPassSamping"":0,""nextIdTimeout"":0,""nextStaionPassSamping"":null,""nextStationFail"":null,""nextStationPass"":null,""orderNum"":1,""preStation"":null,""previousId"":0,""remark"":null,""repairInfo"":null,""route"":null,""sampingModel"":1,""sampingRate"":null,""sectionType"":-1,""startMaxTime"":0.0,""startMinTime"":0.0,""subAllCheckNum"":0,""type"":0,""updateName"":null,""updateTime"":null,""verticalAxis"":85},{""completeMaxTime"":0.0,""completeMinTime"":0.0,""createName"":""3318418"",""createTime"":""2021-01-06T19:04:56"",""ctType"":null,""failCount"":3,""horizontalAxis"":215,""id"":1773,""isFind"":1,""isGatherTestdata"":1,""isInput"":0,""isRedoingCheck"":1,""isRepairCheck"":1,""isReworkSamping"":1,""lineAllCheckNum"":0,""maxQueuingNum"":0,""module"":{""authorityUsers"":null,""createName"":null,""createTime"":null,""id"":61,""inMultiple"":1,""name"":""过站新"",""orderNum"":0,""outMultiple"":1,""path"":""%IPAddressDir%AMF\\public\\过站新.rar"",""remark"":null,""type"":1,""updateName"":null,""updateTime"":null,""version"":""1""},""name"":""COLORAIM-TEST"",""nextIdFail"":-1,""nextIdPass"":1774,""nextIdPassSamping"":0,""nextIdTimeout"":0,""nextStaionPassSamping"":null,""nextStationFail"":null,""nextStationPass"":null,""orderNum"":2,""preStation"":null,""previousId"":1772,""remark"":null,""repairInfo"":null,""route"":null,""sampingModel"":1,""sampingRate"":null,""sectionType"":-1,""startMaxTime"":0.0,""startMinTime"":0.0,""subAllCheckNum"":0,""type"":1,""updateName"":null,""updateTime"":null,""verticalAxis"":65},{""completeMaxTime"":0.0,""completeMinTime"":0.0,""createName"":""3318418"",""createTime"":""2021-01-06T19:04:56"",""ctType"":null,""failCount"":3,""horizontalAxis"":365,""id"":1774,""isFind"":1,""isGatherTestdata"":1,""isInput"":0,""isRedoingCheck"":1,""isRepairCheck"":1,""isReworkSamping"":1,""lineAllCheckNum"":0,""maxQueuingNum"":0,""module"":{""authorityUsers"":null,""createName"":null,""createTime"":null,""id"":61,""inMultiple"":1,""name"":""过站新"",""orderNum"":0,""outMultiple"":1,""path"":""%IPAddressDir%AMF\\public\\过站新.rar"",""remark"":null,""type"":1,""updateName"":null,""updateTime"":null,""version"":""1""},""name"":""阳极全检(Ano_QC)"",""nextIdFail"":-1,""nextIdPass"":1775,""nextIdPassSamping"":0,""nextIdTimeout"":0,""nextStaionPassSamping"":null,""nextStationFail"":null,""nextStationPass"":null,""orderNum"":3,""preStation"":null,""previousId"":1773,""remark"":null,""repairInfo"":null,""route"":null,""sampingModel"":1,""sampingRate"":null,""sectionType"":-1,""startMaxTime"":0.0,""startMinTime"":0.0,""subAllCheckNum"":0,""type"":1,""updateName"":null,""updateTime"":null,""verticalAxis"":95},{""completeMaxTime"":0.0,""completeMinTime"":0.0,""createName"":""3318418"",""createTime"":""2021-01-06T19:04:56"",""ctType"":null,""failCount"":3,""horizontalAxis"":515,""id"":1775,""isFind"":1,""isGatherTestdata"":1,""isInput"":0,""isRedoingCheck"":1,""isRepairCheck"":1,""isReworkSamping"":1,""lineAllCheckNum"":0,""maxQueuingNum"":0,""module"":{""authorityUsers"":null,""createName"":null,""createTime"":null,""id"":61,""inMultiple"":1,""name"":""过站新"",""orderNum"":0,""outMultiple"":1,""path"":""%IPAddressDir%AMF\\public\\过站新.rar"",""remark"":null,""type"":1,""updateName"":null,""updateTime"":null,""version"":""1""},""name"":""来料全检(Assy_IQC)"",""nextIdFail"":-1,""nextIdPass"":1776,""nextIdPassSamping"":0,""nextIdTimeout"":0,""nextStaionPassSamping"":null,""nextStationFail"":null,""nextStationPass"":null,""orderNum"":4,""preStation"":null,""previousId"":1774,""remark"":null,""repairInfo"":null,""route"":null,""sampingModel"":1,""sampingRate"":null,""sectionType"":-1,""startMaxTime"":0.0,""startMinTime"":0.0,""subAllCheckNum"":0,""type"":1,""updateName"":null,""updateTime"":null,""verticalAxis"":80},{""completeMaxTime"":0.0,""completeMinTime"":0.0,""createName"":""3318418"",""createTime"":""2021-01-06T19:04:56"",""ctType"":null,""failCount"":3,""horizontalAxis"":700,""id"":1776,""isFind"":1,""isGatherTestdata"":1,""isInput"":0,""isRedoingCheck"":1,""isRepairCheck"":1,""isReworkSamping"":1,""lineAllCheckNum"":0,""maxQueuingNum"":0,""module"":{""authorityUsers"":null,""createName"":null,""createTime"":null,""id"":61,""inMultiple"":1,""name"":""过站新"",""orderNum"":0,""outMultiple"":1,""path"":""%IPAddressDir%AMF\\public\\过站新.rar"",""remark"":null,""type"":1,""updateName"":null,""updateTime"":null,""version"":""1""},""name"":""UMP1-TEST"",""nextIdFail"":-1,""nextIdPass"":1777,""nextIdPassSamping"":0,""nextIdTimeout"":0,""nextStaionPassSamping"":null,""nextStationFail"":null,""nextStationPass"":null,""orderNum"":5,""preStation"":null,""previousId"":1775,""remark"":null,""repairInfo"":null,""route"":null,""sampingModel"":1,""sampingRate"":null,""sectionType"":-1,""startMaxTime"":0.0,""startMinTime"":0.0,""subAllCheckNum"":0,""type"":1,""updateName"":null,""updateTime"":null,""verticalAxis"":90},{""completeMaxTime"":0.0,""completeMinTime"":0.0,""createName"":""3318418"",""createTime"":""2021-01-06T19:04:56"",""ctType"":null,""failCount"":3,""horizontalAxis"":850,""id"":1777,""isFind"":1,""isGatherTestdata"":1,""isInput"":0,""isRedoingCheck"":1,""isRepairCheck"":1,""isReworkSamping"":1,""lineAllCheckNum"":0,""maxQueuingNum"":0,""module"":{""authorityUsers"":null,""createName"":null,""createTime"":null,""id"":61,""inMultiple"":1,""name"":""过站新"",""orderNum"":0,""outMultiple"":1,""path"":""%IPAddressDir%AMF\\public\\过站新.rar"",""remark"":null,""type"":1,""updateName"":null,""updateTime"":null,""version"":""1""},""name"":""LMI1-TEST"",""nextIdFail"":-1,""nextIdPass"":1778,""nextIdPassSamping"":0,""nextIdTimeout"":0,""nextStaionPassSamping"":null,""nextStationFail"":null,""nextStationPass"":null,""orderNum"":6,""preStation"":null,""previousId"":1776,""remark"":null,""repairInfo"":null,""route"":null,""sampingModel"":1,""sampingRate"":null,""sectionType"":-1,""startMaxTime"":0.0,""startMinTime"":0.0,""subAllCheckNum"":0,""type"":1,""updateName"":null,""updateTime"":null,""verticalAxis"":80},{""completeMaxTime"":0.0,""completeMinTime"":0.0,""createName"":""3318418"",""createTime"":""2021-01-06T19:04:56"",""ctType"":null,""failCount"":3,""horizontalAxis"":1020,""id"":1778,""isFind"":1,""isGatherTestdata"":1,""isInput"":0,""isRedoingCheck"":1,""isRepairCheck"":1,""isReworkSamping"":1,""lineAllCheckNum"":0,""maxQueuingNum"":0,""module"":{""authorityUsers"":null,""createName"":null,""createTime"":null,""id"":121,""inMultiple"":1,""name"":""序列化"",""orderNum"":0,""outMultiple"":1,""path"":""%IPAddressDir%AMF\\public\\序列化.rar"",""remark"":null,""type"":1,""updateName"":null,""updateTime"":null,""version"":""1""},""name"":""出货全检(Assy_FQC)"",""nextIdFail"":-1,""nextIdPass"":1779,""nextIdPassSamping"":0,""nextIdTimeout"":0,""nextStaionPassSamping"":null,""nextStationFail"":null,""nextStationPass"":null,""orderNum"":7,""preStation"":null,""previousId"":1777,""remark"":null,""repairInfo"":null,""route"":null,""sampingModel"":1,""sampingRate"":null,""sectionType"":-1,""startMaxTime"":0.0,""startMinTime"":0.0,""subAllCheckNum"":0,""type"":1,""updateName"":null,""updateTime"":null,""verticalAxis"":95},{""completeMaxTime"":0.0,""completeMinTime"":0.0,""createName"":""3318418"",""createTime"":""2021-01-06T19:04:56"",""ctType"":null,""failCount"":3,""horizontalAxis"":1160,""id"":1779,""isFind"":1,""isGatherTestdata"":1,""isInput"":1,""isRedoingCheck"":1,""isRepairCheck"":1,""isReworkSamping"":1,""lineAllCheckNum"":0,""maxQueuingNum"":0,""module"":{""authorityUsers"":null,""createName"":null,""createTime"":null,""id"":21,""inMultiple"":1,""name"":""装箱"",""orderNum"":0,""outMultiple"":1,""path"":""%IPAddressDir%AMF\\public\\众思装箱.rar"",""remark"":null,""type"":1,""updateName"":null,""updateTime"":null,""version"":""1""},""name"":""打包装箱(package)"",""nextIdFail"":-1,""nextIdPass"":1780,""nextIdPassSamping"":0,""nextIdTimeout"":0,""nextStaionPassSamping"":null,""nextStationFail"":null,""nextStationPass"":null,""orderNum"":8,""preStation"":null,""previousId"":1778,""remark"":null,""repairInfo"":null,""route"":null,""sampingModel"":1,""sampingRate"":null,""sectionType"":-1,""startMaxTime"":0.0,""startMinTime"":0.0,""subAllCheckNum"":0,""type"":1,""updateName"":null,""updateTime"":null,""verticalAxis"":95},{""completeMaxTime"":0.0,""completeMinTime"":0.0,""createName"":""3318418"",""createTime"":""2021-01-06T19:04:56"",""ctType"":null,""failCount"":3,""horizontalAxis"":1300,""id"":1780,""isFind"":1,""isGatherTestdata"":1,""isInput"":1,""isRedoingCheck"":1,""isRepairCheck"":1,""isReworkSamping"":1,""lineAllCheckNum"":0,""maxQueuingNum"":0,""module"":{""authorityUsers"":null,""createName"":null,""createTime"":null,""id"":61,""inMultiple"":1,""name"":""过站新"",""orderNum"":0,""outMultiple"":1,""path"":""%IPAddressDir%AMF\\public\\过站新.rar"",""remark"":null,""type"":1,""updateName"":null,""updateTime"":null,""version"":""1""},""name"":""OQC抽检(OQC)"",""nextIdFail"":-1,""nextIdPass"":0,""nextIdPassSamping"":0,""nextIdTimeout"":0,""nextStaionPassSamping"":null,""nextStationFail"":null,""nextStationPass"":null,""orderNum"":9,""preStation"":null,""previousId"":1779,""remark"":null,""repairInfo"":null,""route"":null,""sampingModel"":1,""sampingRate"":null,""sectionType"":-1,""startMaxTime"":0.0,""startMinTime"":0.0,""subAllCheckNum"":0,""type"":10,""updateName"":null,""updateTime"":null,""verticalAxis"":90}],""SERVICE_RUNTIME"":16}";

            //获取mes返回json字符串，并反序列化为字典
            if (result == "error")
            {
                MessageBox.Show("Mes连接不正常", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //获取mes返回json字符串，并反序列化为字典
            var returnStr = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

            string message = returnStr.ContainsKey("MESSAGE") ? returnStr["MESSAGE"].ToString() : "error";

            //如果mes返回FAIL，获取失败信息并显示到界面
            if (returnStr["RESULT"].ToString() == "FAIL")
            {
                MessageBox.Show(message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //获取所有的工站json信息
            string stationFull = returnStr["STATIONS"].ToString();
            var ordersObj = JsonConvert.DeserializeObject(stationFull);

            foreach (var obj in (IEnumerable)ordersObj)
            {
                var orderDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(obj));
                StationList.Add(new MesDataStruct.StationInfo { Id = orderDic["id"].ToString(), Name = orderDic["name"].ToString() });
            }
        }
    }
}