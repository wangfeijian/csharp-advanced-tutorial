using Newtonsoft.Json;
using Soso.Common.SystemHelp;

namespace Soso.Common.Test.SystemHelp
{
    public class Todo
    {
        [JsonProperty("userId")]
        public int? UserId = null;
        [JsonProperty("id")]
        public int? Id = null;
        [JsonProperty("title")]
        public string? Title = null;
        [JsonProperty("completed")]
        public bool? Completed = null;
    };

    [TestClass]
    public class HttpMethodHelpTest
    {
        [TestMethod]
        public void TestPost()
        {
            var url = "https://jsonplaceholder.typicode.com/todos";
            var todo = new Todo { UserId = 77, Id = 1, Title = "Test post", Completed = false };
            string postData = JsonConvert.SerializeObject(todo, Formatting.None);
            var result = HttpMethodHelp.Post(url, postData);
            var returnTodo = JsonConvert.DeserializeObject<Todo>(result);
            Assert.AreEqual(returnTodo.UserId, 77);
            Assert.AreEqual(returnTodo.Id, 201);
            Assert.AreEqual(returnTodo.Title, "Test post");
            Assert.AreEqual(returnTodo.Completed, false);
        }

        [TestMethod]
        public void TestGet()
        {
            var url = "https://jsonplaceholder.typicode.com/todos/3";
            var result = HttpMethodHelp.Get(url);
            var todo = JsonConvert.DeserializeObject<Todo>(result);
            Assert.AreEqual(todo.UserId, 1);
            Assert.AreEqual(todo.Id, 3);
            Assert.AreEqual(todo.Title, "fugiat veniam minus");
            Assert.AreEqual(todo.Completed, false);
        }

    }
}
