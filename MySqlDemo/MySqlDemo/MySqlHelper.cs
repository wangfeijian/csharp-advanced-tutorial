using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Reflection;
using DataModel;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MySqlDemo
{
    public class MySqlHelper
    {
        public static readonly string ConnectionMySqlStr = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;

        /// <summary>
        /// 只能用于主键查询，当有多条数据时会出错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T FindSingleData<T>(int id)
        {
            Type type = typeof(T);

            string commandStr = GetSingleSqlDataCommandText<T>(id);

            var tReturn = ExceteSql(commandStr, command =>
            {
                T tempT = (T)Activator.CreateInstance(type);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foreach (PropertyInfo property in type.GetProperties())
                        {
                            property.SetValue(tempT, reader[property.Validate()] is DBNull ? DBNull.Value.ToString() : reader[property.Validate()]);
                        }
                    }
                }

                return tempT;
            });

            return tReturn;
        }

        public static T FindSingleData<T>(string id)
        {
            Type type = typeof(T);

            string commandStr = GetSingleSqlDataCommandText<T>(id);

            var tReturn = ExceteSql(commandStr, command =>
            {
                T tempT = (T)Activator.CreateInstance(type);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foreach (PropertyInfo property in type.GetProperties())
                        {
                            property.SetValue(tempT, reader[property.Validate()] is DBNull ? DBNull.Value.ToString() : reader[property.Validate()]);
                        }
                    }
                }

                return tempT;
            });
            return tReturn;
        }

        public static List<T> FindAllData<T>(int id)
        {
            Type type = typeof(T);


            string commandStr = GetSingleSqlDataCommandText<T>(id);

            var tReturnList = ExceteSql(commandStr, command =>
            {
                List<T> tempList = new List<T>();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T tempT = (T)Activator.CreateInstance(type);
                        foreach (PropertyInfo property in type.GetProperties())
                        {
                            property.SetValue(tempT, reader[property.Validate()] is DBNull ? DBNull.Value.ToString() : reader[property.Validate()]);
                        }
                        tempList.Add(tempT);
                    }
                }

                return tempList;
            });

            return tReturnList;
        }

        public static List<T> FindAllData<T>()
        {
            Type type = typeof(T);
            string name = type.Name.ToLower().Substring(0, type.Name.Length - 5);
            string commandStr = $"SELECT * FROM {name}";

            var tReturnList = ExceteSql(commandStr, command =>
            {
                List<T> tempList = new List<T>();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T tempT = (T)Activator.CreateInstance(type);
                        foreach (PropertyInfo property in type.GetProperties())
                        {
                            property.SetValue(tempT, reader[property.Validate()] is DBNull ? DBNull.Value.ToString() : reader[property.Validate()]);
                        }
                        tempList.Add(tempT);
                    }
                }

                return tempList;
            });

            return tReturnList;
        }

        public static bool UpdateData<T>(string sqlStr, int id)
        {
            Type type = typeof(T);
            string name = type.Name.ToLower().Substring(0, type.Name.Length - 5);
            var props = type.GetProperties();
            string commandStr = $"UPDATE {name} SET {sqlStr} WHERE {props.First(p => p.Name == "Id").Validate()}={id}";

            var tReturn = ExceteSql(commandStr, command =>
            {
                int returnInt = command.ExecuteNonQuery();
                return returnInt;
            });

            return tReturn != 0;
        }
        private static T ExceteSql<T>(string sql, Func<MySqlCommand, T> func)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionMySqlStr))
            {
                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    T tResult;
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Transaction = transaction;
                        tResult = func.Invoke(command);
                    }
                    transaction.Commit();
                    return tResult;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private static string GetSingleSqlDataCommandText<T>(int id)
        {
            Type type = typeof(T);
            string name = type.Name.ToLower().Substring(0, type.Name.Length - 5);
            var props = type.GetProperties();
            var result = $"SELECT {string.Join(",", props.Select(p => p.Validate()))} FROM {name} WHERE {props.First(p => p.Name == "Id").Validate()}={id}";
            return result;
        }

        private static string GetSingleSqlDataCommandText<T>(string id)
        {
            Type type = typeof(T);
            string name = type.Name.ToLower().Substring(0, type.Name.Length - 5);
            var props = type.GetProperties();
            var result = $"SELECT {string.Join(",", props.Select(p => p.Validate()))} FROM {name} WHERE {props.First(p => p.Name == "Id").Validate()}=\"{id}\"";
            return result;
        }

    }
}
