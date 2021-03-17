using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace AttributeDemo
{
    public enum UserStates
    {
        [Remark("正常")]
        Normal = 0,
        [Remark("冻结")]
        Freeze,
        [Remark("删除")]
        Delete,
    }

    public enum UserLevel
    {
        [Remark("员工")]
        Employee,
        [Remark("管理者")]
        Manager,
        [Remark("董事长")]
        CEO,
    }

    [Info("这是一个员工类")]
    public class People
    {
        public People(string name, int id)
        {
            Name = name;
            Id = id;
        }

        [CheckId(10000,9999999,ErrorInfo = "ID给定的值不在范围内")]
        public int Id { get; set; }

        [CheckString(ErrorInfo = "Name的值不能为空")]
        public string Name { get; set; }

        public UserStates UserStates { get; set; }
        public UserLevel UserLevel { get; set; }
        public void ShowPeople()
        {
            string msg = string.Empty;

            if (!Manage.CheckValue(this, out msg))
            {
                Console.WriteLine(msg);
                return;
            }
            Console.WriteLine($"Name:{Name},Id:{Id},User Status:{UserStates.GetRemarkValue()},Level：{UserLevel.GetRemarkValue()}");
        }
    }
}
