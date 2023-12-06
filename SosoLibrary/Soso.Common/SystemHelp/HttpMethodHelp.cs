using System;
using System.Net.Http;
using System.Text;

namespace Soso.Common.SystemHelp
{
    /// <summary>
    /// Http常用方法帮助类
    /// </summary>
    /// <remark>
    /// 主要包括Post、Get等方法
    /// </remark>
    public static class HttpMethodHelp
    {
        /// <summary>
        /// Http Get方法
        /// </summary>
        /// <param name="url">http get地址</param>
        /// <returns>json数据</returns>
        /// <exception cref="Exception"></exception>
        public static string Get(string url)
        {
            string result = "error";
            try
            {
                HttpClient httpClient = new HttpClient();
                using (var response = httpClient.GetAsync(url).Result)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                throw (new Exception("Get方法异常！", ex));
            }

            return result;
        }

        /// <summary>
        /// Http Post方法
        /// </summary>
        /// <param name="url">http post地址</param>
        /// <param name="postJsonData">需要传入json格式的字符串</param>
        /// <returns>json字符串</returns>
        /// <exception cref="Exception"></exception>
        public static string Post(string url, string postJsonData)
        {
            string result = "error";
            try
            {
                HttpClient httpClient = new HttpClient();
                using (StringContent jsonContent = new StringContent(postJsonData, Encoding.UTF8, "application/json"))
                {
                    using (var response = httpClient.PostAsync(url, jsonContent).Result)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw (new Exception("Post方法异常！", ex));
            }
            return result;
        }
    }
}
