using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;


namespace JasonDemo
{
    class Person
    {
        public string Name;
        public int Age;

        public override string ToString()
        {
            return string.Format($"Name:{Name}.Age:{Age}");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //序列化json
            //Person p = new Person() {Name = "wang", Age = 19};
            //string outPut = JsonConvert.SerializeObject(p);
            //File.WriteAllText("test.json",outPut);
            //反序列化
            string inPut = File.ReadAllText("test.json");
            Person p = JsonConvert.DeserializeObject<Person>(inPut);
            Console.WriteLine(p.ToString());
        }
    }
}
