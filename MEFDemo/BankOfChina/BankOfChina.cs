using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankInterface;

namespace BankOfChina
{
    [Export(typeof(ICard))]
    public class BankOfChina : ICard
    {
        public double Money { get; set; }

        public void CheckOutMoney(double money)
        {
            Money -= money;
        }

        public string GetCountInfo()
        {
            return "Bank of China";
        }

        public void SaveMoney(double money)
        {
            Money += money;
        }
    }
}
