string fileName = "D:/Desktop Image/1.jpg";
var attribute = File.GetAttributes(fileName);
Console.WriteLine($"File {fileName} is {attribute}");

File.SetAttributes(fileName, FileAttributes.Archive);
attribute = File.GetAttributes(fileName);
Console.WriteLine($"File {fileName} is {attribute}");

ActionEnum actionEnum = ActionEnum.Read | ActionEnum.Write;
ActionEnum actionEnum2 = ActionEnum.Read | ActionEnum.Delete;
Console.WriteLine(actionEnum);
Console.WriteLine(actionEnum2);

[Flags]
enum ActionEnum
{
    None = 0,
    Read = 0x0001,
    Write = 0x0002,
    ReadWrite = Read | Write,
    Delete = 0x0004,
    Query = 0x0008,
    Sync = 0x0010
}