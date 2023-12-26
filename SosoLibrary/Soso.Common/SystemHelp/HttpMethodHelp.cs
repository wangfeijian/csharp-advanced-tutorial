#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.SystemHelp
 * 唯一标识：182b41a0-8183-429b-bf7f-03a4b690bfd5
 * 文件名：HttpMethodHelp
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：11/14/2023 11:09:29 AM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 * 
 * 版本：V1.0.0
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>
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
