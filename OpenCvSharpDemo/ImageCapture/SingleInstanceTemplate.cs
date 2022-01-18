using System;

namespace ImageCapture
{
    /// <summary>
    /// 单例模板类
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class SingleInstanceTemplate<T> where T : class, new()
    {
        private static readonly object Lock = new object();

        private static T _instance;

        /// <summary>
        /// 获取单例类实例
        /// </summary>
        /// <returns></returns>
        public static T GetInstance()
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    if (_instance == null)
                        _instance = Activator.CreateInstance<T>();
                }
            }
            return _instance;
        }
    }
}
