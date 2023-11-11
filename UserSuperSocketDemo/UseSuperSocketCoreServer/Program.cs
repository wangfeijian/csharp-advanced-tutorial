// See https://aka.ms/new-console-template for more information
using UseSuperSocketCoreServer;

Console.WriteLine("Hello, World!");

SocketServerServices.Instance.RunServer("$");
Console.ReadLine();