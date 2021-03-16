using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDbMgr;

namespace SqlServiceMgr
{
    public class SqlDbHelper :IDbHelper
    {
        public SqlDbHelper()
        {
            Console.WriteLine($"{GetType().Name}被构造。");
        }



        public void Query()
        {
            Console.WriteLine($"{GetType().FullName}被调用。");
        }
    }
}
