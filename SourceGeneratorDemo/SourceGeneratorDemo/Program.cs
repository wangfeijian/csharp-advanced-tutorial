namespace SourceGeneratorDemo;

partial class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        HelloFrom("Generated Code");
    }

    static partial void HelloFrom(string name);
}