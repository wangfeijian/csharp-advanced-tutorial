namespace GeneratorHelloWorld;

// 1、源生成器中的Microsoft.CodeAnalysis.CSharp的版本要与引用项目中的一致
// 2、如果生成器的库中引用了某些库，目标项目中也必须包含这些库
// 3、自己编写的dll可以通更改如下特性实现
//  <ItemGroup>
//      <Analyzer Include = "..\MySourceGenerator\bin\Debug\netstandard2.0\MySourceGenerator.dll" />
//  </ ItemGroup >
// 4、如果直接引用项目需要更改成如下方式
// <ItemGroup>
//     <ProjectReference Include = "..\PathTo\SourceGenerator.csproj"
//                       OutputItemType="Analyzer"
//                       ReferenceOutputAssembly="false" />
// </ItemGroup>
partial class Program
{
    static void Main(string[] args)
    {
        HelloFrom("Generated Code");

        var assem = new AssemblyStation();

        var upload = new UpLoadStation();
        var camera = new CameraStation();
    }

    static partial void HelloFrom(string name);
}

partial class AssemblyStation
{
    partial void ClassCtor()
    {
        Console.WriteLine("组装工站开始构造");
    }
}

partial class UpLoadStation
{
    partial void ClassCtor()
    {
        Console.WriteLine("上传工站开始构造");
    }
}

partial class CameraStation
{
    partial void ClassCtor()
    {
        Console.WriteLine("相机工站开始构造");
    }
}
