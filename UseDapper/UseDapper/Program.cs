using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace UseDapper
{
    class Program
    {
        // 安装System.Data.SQLite
        // 安装Dapper

        static void Main(string[] args)
        {
            var datas = GetListAsnyc<Person>("SELECT * FROM Test");

            foreach (var s in datas.Result)
            {
                Console.WriteLine($"{s.Id}_{s.Name}_{s.Mobile}_{s.Mail}_{s.Address}");
            }

            Person p = new Person { Name = "Wang", Mobile = "123456789", Mail = "wang@163.com", Address = "广东省深圳市" };
            // AsnycInsert(p);

            AsyncUpdate(p, "wangfeijian");

            Console.WriteLine("修改后");
            var data = AsyncQuery("select * from test where Name='Wang'");
            if (data.Result.Count > 0)
            {
                var persons = data.Result.First();
                Console.WriteLine($"{persons.Id}_{persons.Name}_{persons.Mobile}_{persons.Mail}_{persons.Address}");
            }
            else
            {
                Console.WriteLine("未查询到相关数据");
            }

            Console.WriteLine("删除后");

            Console.ReadLine();

            var count = AsyncDelete("Wang");
            Console.WriteLine($"删除了{count.Result}行");

            //datas = AsyncQuery("SELECT * FROM Test");

            foreach (var s in datas.Result)
            {
                Console.WriteLine($"{s.Id}_{s.Name}_{s.Mobile}_{s.Mail}_{s.Address}");
            }

            Console.ReadLine();
        }

        private static T Execute<T>(Func<SQLiteConnection, T> func)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=Test.db"))
            {
                return func(connection);
            }
        }

        public static Task<IEnumerable<T>> GetListAsnyc<T>(string sql)
        {
            return Execute(async (conn) =>
            {
                var datas = await conn.QueryAsync<T>(sql);
                return datas;
            });
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        static async Task<List<Person>> AsyncQuery(string sql)
        {
            using (IDbConnection connection = new SQLiteConnection("Data Source=Test.db"))
            {
                //connection.Open();
                var datas = await connection.QueryAsync<Person>(sql);
                return datas.ToList();
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        static async void AsnycInsert(Person person)
        {
            using (IDbConnection connection = new SQLiteConnection("Data Source=Test.db"))
            {
                await connection.ExecuteAsync("insert into Test (Name,Mobile,Mail,Address) VALUES (@Name,@Mobile,@Mail,@Address)", person);
            }
        }

        /// <summary>
        /// 修改
        /// 需要注意如果是字符串需要额外添加双引号或者单引号
        /// </summary>
        /// <param name="person"></param>
        /// <param name="name"></param>
        static async void AsyncUpdate(Person person, string name)
        {
            using (IDbConnection connection = new SQLiteConnection("Data Source=Test.db"))
            {
                await connection.ExecuteAsync(
                    $"update Test set Name = @Name, Mobile = @Mobile, Mail = @Mail, Address = @Address where Name='{name}'",
                    person);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static async Task<int> AsyncDelete(string name)
        {
            using (IDbConnection connection = new SQLiteConnection("Data Source=Test.db"))
            {
                return await connection.ExecuteAsync($"delete from Test where Name='{name}'");
            }

        }
    }
}
