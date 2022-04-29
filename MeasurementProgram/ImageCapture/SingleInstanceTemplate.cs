using System;
using System.Net;

namespace ImageCapture
{
    /// <summary>
    /// 单例模板类
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class SingleInstanceTemplate<T> where T : class, new()
    {
        private static T _instance = new T();
        public static T GetInstance() 
        {
            return _instance; 
        }
    }

    public class CaptureHelper
    {
        /// <summary>
        /// 是否有效IP
        /// </summary>
        /// <param name="strIP"></param>
        /// <returns></returns>
        public static bool IsValidIP(string strIP)
        {
            IPAddress ipTry;
            return IPAddress.TryParse(strIP, out ipTry);
        }
    }

}
