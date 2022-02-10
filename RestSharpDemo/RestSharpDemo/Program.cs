using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace RestSharpDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string url1 = "http://localhost:5000/api/test";
            Console.WriteLine($"{url1} Get测试");
            TestGet(url1);
            Console.WriteLine($"{url1} Post测试");
            TestPost(url1);

            string url2 = "http://localhost:5000/api/testapi";
            Console.WriteLine($"{url2} Get测试");
            TestGet(url2);

            Console.WriteLine($"{url2} Post测试");
            MyClass myClass = new MyClass {str = "0"};
            TestPost(url2,myClass);

            Console.ReadKey();
        }

        static void TestGet(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");

            //request.Timeout = 10000;
            //request.AddHeader("Cache-Control", "no-cache");
            //request.AddParameter("name", "value"); 
            //request.AddUrlSegment("id", "123"); 


            var response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        static void TestPost(string url, MyClass str = null)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            if (str!=null)
            {
                request.AddJsonBody(str);
            }

            var response = client.Execute<dynamic>(request);
            // response.Data可以通过json序列化
            Console.WriteLine(response.Data);
        }
    }

    public class MyClass
    {
        public string str { get; set; }
    }
}
