using BuilderPattern;
using System.Configuration;
using System.Reflection;

// 普通实现
//House house = GameManager.CreateHouse(new RomanHouseBuilder());

// 通过配置文件实现
string? assemblyName = ConfigurationManager.AppSettings["BuilderAssembly"];
string? buildName = ConfigurationManager.AppSettings["BuilderClass"];

if (string.IsNullOrEmpty(assemblyName)|| string.IsNullOrEmpty(buildName))
{
    return;
}

Assembly assembly = Assembly.Load(assemblyName);
Type t = assembly?.GetType($"{assemblyName}.{buildName}");

if (t == null)
{
    return;
}

Builder builder = Activator.CreateInstance(t) as Builder;
House house = GameManager.CreateHouse(builder);