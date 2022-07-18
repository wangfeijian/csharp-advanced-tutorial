using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankInterface
{
    public interface ICard
    {
        //账户金额
        double Money { get; set; }
        //获取账户信息
        string GetCountInfo();
        //存钱
        void SaveMoney(double money);
        //取钱
        void CheckOutMoney(double money);
    }
}