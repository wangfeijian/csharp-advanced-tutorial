using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using MSXML2;

namespace MVVMDemo
{
    public class MesTool
    {
        static CookieContainer cookie = new CookieContainer();
        public static string doHttpPost(string Url, string postDataStr)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/json";
                //request.Accept = "application/json";
                //request.ContentType = "application/x-www-form-urlencoded";

                //request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                request.CookieContainer = cookie;
                Stream myRequestStream = request.GetRequestStream();
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                myStreamWriter.Write(postDataStr);
                myStreamWriter.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                response.Cookies = cookie.GetCookies(response.ResponseUri);
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception e)
            {
                MessageBox.Show("Mes网络连接失败");
                return "error";
            }
        }

        public static bool InterfaceEnble(string url)
        {
            XMLHTTP http = new XMLHTTP();
            try
            {
                http.open("GET ", url, false, null, null);
                http.send(null);
                int iStatus = http.status;
                //如果状态不为200，就是不能调用此接口 
                if (iStatus == 200)
                    return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mes网络连接失败");
                //MessageBox.Show("接口调用失败," + ex);
            }

            return false;
        }

        public static bool CheckActiveWebService(string url)
        {
            try
            {
                string uri = url + "?wsdl";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                request.UseDefaultCredentials = true;
                request.Method = "GET";
                request.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK && response.ContentType.Substring(0, 8) == "text/xml")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (WebException e)
            {
                MessageBox.Show("Mes网络连接失败");
                return false;
            }
        }

        /// <summary>
        /// 此方法为获取mes返回的结果，成功为PASS，失败为FAIL
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string MesReturnResult(string result)
        {
            string pattern = @"""RESULT"":""\w{4}""";
            Match mc = Regex.Match(result, pattern);
            if (mc.Length<=0)
            {
                return "error";
            }
            return mc.Groups[0].Value;
        }

        /// <summary>
        /// 返回最终属性的字符串值
        /// </summary>
        /// <param name="result">匹配的字符串</param>
        /// <param name="pattern">正则表达式匹配规则</param>
        /// <param name="b">是否为最终的属性，由于匹配的字符串有可能是一个json对象，不是单个属性</param>
        /// <returns></returns>
        public static string SubStringResult(string result, string pattern, bool b = false)
        {
            Match mc = Regex.Match(result, pattern);
            string returnStr = mc.Groups[0].Value;
            if (!b)
            {
                return returnStr;
            }
            string[] str = returnStr.Split(':');
            return str[1].Replace('"', ' ').Trim().Replace(',', ' ').Trim();
        }
    }
}
