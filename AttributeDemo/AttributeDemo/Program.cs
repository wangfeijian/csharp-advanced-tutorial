using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            People p = new People(" ", 1000) {UserStates = UserStates.Normal,UserLevel = UserLevel.CEO};
            People p2 = new People("fei", 200000) {UserStates = UserStates.Freeze,UserLevel = UserLevel.Employee};
            People p3 = new People("jian", 3000) {UserStates = UserStates.Delete,UserLevel = UserLevel.Manager};
            p.ShowPeople();
            p2.ShowPeople();
            p3.ShowPeople();
            
            Console.WriteLine(Manage.GetInfo(p));
        }
    }
}
