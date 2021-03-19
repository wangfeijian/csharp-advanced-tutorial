using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
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

            T tReturn = (T)Activator.CreateInstance(type);

            string commandStr = GetSingleSqlDataCommandText<T>(id);

            using (MySqlConnection conn = new MySqlConnection(ConnectionMySqlStr))
            {
                conn.Open();
                using (MySqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = commandStr;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreach (PropertyInfo property in type.GetProperties())
                            {
                                property.SetValue(tReturn, reader[property.Validate()] is DBNull ? DBNull.Value.ToString() : reader[property.Validate()]);
                            }
                        }
                    }
                }
            }

            return tReturn;
        }

        public static T FindSingleData<T>(string id)
        {
            Type type = typeof(T);

            T tReturn = (T)Activator.CreateInstance(type);

            string commandStr = GetSingleSqlDataCommandText<T>(id);

            using (MySqlConnection conn = new MySqlConnection(ConnectionMySqlStr))
            {
                conn.Open();
                using (MySqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = commandStr;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreach (PropertyInfo property in type.GetProperties())
                            {
                                property.SetValue(tReturn, reader[property.Validate()] is DBNull ? DBNull.Value.ToString() : reader[property.Validate()]);
                            }
                        }
                    }
                }
            }

            return tReturn;
        }

        public static List<T> FindAllData<T>(int id)
        {
            Type type = typeof(T);

            List<T> tReturnList = new List<T>();


            string commandStr = GetSingleSqlDataCommandText<T>(id);

            using (MySqlConnection conn = new MySqlConnection(ConnectionMySqlStr))
            {
                conn.Open();
                using (MySqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = commandStr;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T tempT = (T)Activator.CreateInstance(type);
                            foreach (PropertyInfo property in type.GetProperties())
                            {
                                property.SetValue(tempT, reader[property.Validate()] is DBNull ? DBNull.Value.ToString() : reader[property.Validate()]);
                            }
                            tReturnList.Add(tempT);
                        }
                    }
                }
            }

            return tReturnList;
        }

        public static List<T> FindAllData<T>()
        {
            Type type = typeof(T);
            string name = type.Name.ToLower().Substring(0, type.Name.Length - 5);
            List<T> tReturnList = new List<T>();


            string commandStr = $"SELECT * FROM {name}";

            using (MySqlConnection conn = new MySqlConnection(ConnectionMySqlStr))
            {
                conn.Open();
                using (MySqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = commandStr;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T tempT = (T)Activator.CreateInstance(type);
                            foreach (PropertyInfo property in type.GetProperties())
                            {
                                property.SetValue(tempT, reader[property.Validate()] is DBNull ? DBNull.Value.ToString() : reader[property.Validate()]);
                            }
                            tReturnList.Add(tempT);
                        }
                    }
                }
            }

            return tReturnList;
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
