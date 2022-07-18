using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankInterface;

namespace BandOfTraffic
{
    [Export(typeof(ICard))]
    public class BandOfTraffic : ICard
    {
        public double Money { get; set; }
        public string GetCountInfo()
        {
            return "Bank of Traffic";
        }

        public void SaveMoney(double money)
        {
            Money += money;
        }

        public void CheckOutMoney(double money)
        {
            Money -= money;
        }
    }
}
