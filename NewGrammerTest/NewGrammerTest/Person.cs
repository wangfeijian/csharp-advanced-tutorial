namespace NewGrammerTest
{
    public record Person(string Name, int Id);

    public record Student : Person, IDisposable
    {
        public Student(string Name, int Id) : base(Name, Id)
        {
        }

        public void Dispose()
        {
            Console.WriteLine($"开始清除对象 {Name}");
        }
    }
}
