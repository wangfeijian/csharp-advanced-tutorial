using System;
using System.Net.Http;
using System.Net.Http.Headers;
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

        private static AuthenticationHeaderValue GetAuthenticationHeader(string user, string password)
        {
            return new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}")
                ));
        }

        /// <summary>
        /// Http Get方法
        /// </summary>
        /// <param name="url">http get地址</param>
        /// <param name="useAuthenticationHeader">是否需要登陆密码</param>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>json数据</returns>
        /// <exception cref="Exception"></exception>
        public static string Get(string url, bool useAuthenticationHeader = false, string user = "", string password = "")
        {
            string result = "error";
            try
            {
                HttpClient httpClient = new HttpClient();

                if (useAuthenticationHeader)
                {
                    AuthenticationHeaderValue authentication = GetAuthenticationHeader(user, password);
                    httpClient.DefaultRequestHeaders.Authorization = authentication;
                }

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
        /// <param name="useAuthenticationHeader">是否需要登陆密码</param>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>json字符串</returns>
        /// <exception cref="Exception"></exception>
        public static string Post(string url, string postJsonData, bool useAuthenticationHeader = false, string user = "", string password = "")
        {
            string result = "error";
            try
            {
                HttpClient httpClient = new HttpClient();

                if (useAuthenticationHeader)
                {
                    AuthenticationHeaderValue authentication = GetAuthenticationHeader(user, password);
                    httpClient.DefaultRequestHeaders.Authorization = authentication;
                }

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
