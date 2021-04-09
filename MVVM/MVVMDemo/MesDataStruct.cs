using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMDemo
{
    public class MesDataStruct
    {
        public struct UserLogin
        {
            public string user;
            public string pwd;
        }

        public struct ClientInfo
        {
            public string Section { get; set; }
            public string SectionName { get; set; }

            public override string ToString()
            {
                return SectionName;
            }
        }

        public struct GetLineParam
        {
            public string LOGIN_ID { get; set; }
            public string CLIENT_ID { get; set; }
            public string SECTION { get; set; }
        }

        public struct GetOrderParam
        {
            public string LOGIN_ID { get; set; }
            public string CLIENT_ID { get; set; }
            public string LINE { get; set; }
        }

        public struct OrderInfo
        {
            public string PROJECT { get; set; }
            public string LOAD_ID { get; set; }
            public string END_DATE { get; set; }
            public string STATE { get; set; }
            public string TEXT { get; set; }
            public string SHOPORDER { get; set; }
            public string PRODUCT_ID { get; set; }
            public string UPH { get; set; }
            public string ACTUAL_QTY { get; set; }
            public string SHOPORDER_ID { get; set; }
            public string BEGIN_DATE { get; set; }
            public string PRODUCT { get; set; }
            public string QTY { get; set; }
            public string SAP_SHOPORDER { get; set; }
            public string ID { get; set; }
            public string PROJECT_ID { get; set; }
        }

        public struct GetStationParam
        {
            public string LOGIN_ID { get; set; }
            public string SHOPORDER { get; set; }
            public string CLIENT_ID { get; set; }
        }

        public struct StationInfo
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }

        public struct CheckSfcParam
        {
            public string LOGIN_ID { get; set; }
            public string SFC { get; set; }
            public string STATION_NAME { get; set; }
            public string LINE { get; set; }
            public string SHOPORDER { get; set; }
            public string SCHEDULING_ID { get; set; }
            public string CLIENT_ID { get; set; }
        }

        public struct TestData
        {
            public string NAME { get; set; }
            public double VALUE { get; set; }
            public double MAX_VALUE { get; set; }
            public double MIN_VALUE { get; set; }
            public double STANDARD_VALUE { get; set; }
            public string TEST_RESULT { get; set; }

            public TestData(double value, double maxValue, double minValue, string name, double standardValue)
            {
                NAME = name;
                VALUE = value;
                MAX_VALUE = maxValue;
                MIN_VALUE = minValue;
                STANDARD_VALUE = standardValue;
                TEST_RESULT = value <= maxValue && value >= minValue ? "PASS" : "FAIL";
            }
        }

        public struct UpLoadData
        {
            public string CLIENT_ID { get; set; }
            public string LINE_NO { get; set; }
            public string LOGIN_ID { get; set; }
            public string PRODUCT_NAME { get; set; }
            public string PROJECT_NAME { get; set; }
            public string SHOPORDER_NO { get; set; }
            public string SN { get; set; }
            public string STARTIME { get; set; }
            public string TEST_STATION { get; set; }
            public string STATION_ID { get; set; }
            public string TDS_NAME => "LOKI";
            public string FIXTURE_NO { get; set; }
            public string TEST_RESULT { get; set; }
            public string TEXT { get; set; }
            public string VERSION { get; set; }
            public string HW_VERSION => "";
            public List<TestData> TEST_DATA_LIST { get; set; }

        }

        public struct CompleteData
        {
            public string LOGIN_ID { get; set; }
            public string CLIENT_ID { get; set; }
            public string SFC { get; set; }
            public string SCHEDULING_ID { get; set; }
            public string STATION_ID { get; set; }
            public string TEST_TIME { get; set; }
            public string time { get; set; }
        }

        public struct NcCompleteData
        {
            public string LOGIN_ID { get; set; }
            public string SFC { get; set; }
            public string STATION_ID { get; set; }
            public string NC_CODE { get; set; }
            public string NC_CONTEXT { get; set; }
            public string NC_TYPE { get; set; }
            public string SCHEDULING_ID { get; set; }
            public string CLIENT_ID { get; set; }
        }
    }
}
