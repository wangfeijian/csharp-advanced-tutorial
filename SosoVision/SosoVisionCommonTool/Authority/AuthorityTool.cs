using SosoVisionCommonTool.Log;
using System.Runtime.InteropServices;
using Prism.Ioc;

namespace SosoVisionCommonTool.Authority
{
    /// <summary>
    /// 程序登陆用户模式
    /// </summary>
    public enum UserMode
    {
        /// <summary>
        /// 生产模式, 对应OP权限
        /// </summary>
        Operator,

        /// <summary>
        /// 调试员模式
        /// </summary>
        Adjustor,

        /// <summary>
        /// 工程师模式，对应软件工程师
        /// </summary>
        Engineer,
    }

    /// <summary>
    /// 权限管理类
    /// </summary>
    public class AuthorityTool
    {
        /// <summary>
        /// 定义一个模式变化委托函数
        /// </summary>
        public delegate void ModeChangedHandler();

        private static UserMode _userMode = (UserMode)GetCurUser();

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetCurUser();

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ChangeUser(int nNewUser, string szPassword);

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ChangePassword(int nUser, string szPassword, string szNewPassword);

        /// <summary>
        /// 定义模式变化事件
        /// </summary>
        public static event ModeChangedHandler ModeChangedEvent;

        /// <summary>
        /// 获取当前模式
        /// </summary>
        /// <returns></returns>
        public static UserMode GetUserMode()
        {
            return _userMode;
        }

        /// <summary>
        /// 是否为OP模式
        /// </summary>
        /// <returns></returns>
        public static bool IsOpMode()
        {
            return _userMode == UserMode.Operator;
        }

        /// <summary>
        /// 调试员模式
        /// </summary>
        /// <returns></returns>
        public static bool IsAdjustorMode()
        {
            return _userMode == UserMode.Adjustor;
        }

        /// <summary>
        /// 是否为工程师模式
        /// </summary>
        /// <returns></returns>
        public static bool IsEngMode()
        {
            return _userMode == UserMode.Engineer;
        }

        /// <summary>
        /// 系统权限切换到OP等级
        /// </summary>
        public static bool ChangeOpMode()
        {
            return ChangeUserMode(UserMode.Operator, "");
        }

        /// <summary>
        /// 系统权限切换到调试员
        /// </summary>
        /// <param name="strPassword"></param>
        /// <returns>切换成功或切换失败，成功则对整个系统触发权限变更事件</returns>
        public static bool ChangeAdjustorMode(string strPassword)
        {
            return ChangeUserMode(UserMode.Adjustor, strPassword);
        }

        /// <summary>
        /// 系统权限切换到工程师等级
        /// </summary>
        /// <param name="strPassword"></param>
        /// <returns>切换成功或切换失败，成功则对整个系统触发权限变更事件</returns>
        public static bool ChangeEngMode(string strPassword)
        {
            return ChangeUserMode(UserMode.Engineer, strPassword);
        }

        /// <summary>
        /// 修改用户密码，只有工程师才有权限修改密码
        /// </summary>
        /// <param name="newMode">用户</param>
        /// <param name="strOldPassword">旧密码</param>
        /// <param name="strNewPassword">新密码</param>
        /// <returns></returns>
        public static bool ChangePassword(UserMode newMode, string strOldPassword, string strNewPassword)
        {
            if (ChangePassword((int)newMode, strOldPassword, strNewPassword) == 1)
            {
                return true;
            }
            return true;
        }

        /// <summary>
        /// 切换用户模式
        /// </summary>
        /// <param name="newMode"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        public static bool ChangeUserMode(UserMode newMode, string strPassword)
        {
            if (ChangeUser((int)newMode, strPassword) != 1)
                return false;

            _userMode = newMode;

            ModeChangedEvent?.Invoke();
            return true;
        }
    }
}
