#region File attributes test
//string fileName = "D:/Desktop Image/1.jpg";
//var attribute = File.GetAttributes(fileName);
//Console.WriteLine($"File {fileName} is {attribute}");

//File.SetAttributes(fileName, FileAttributes.Archive);
//attribute = File.GetAttributes(fileName);
//Console.WriteLine($"File {fileName} is {attribute}");

//ActionEnum actionEnum = ActionEnum.Read | ActionEnum.Write;
//ActionEnum actionEnum2 = ActionEnum.Read | ActionEnum.Delete;
//Console.WriteLine(actionEnum);
//Console.WriteLine(actionEnum2);

//[Flags]
//enum ActionEnum
//{
//    None = 0,
//    Read = 0x0001,
//    Write = 0x0002,
//    ReadWrite = Read | Write,
//    Delete = 0x0004,
//    Query = 0x0008,
//    Sync = 0x0010
//}
#endregion

GetIntCount(12340563798);

public void GetIntCount(long number)
{
    Dictionary<long, long> result = new Dictionary<long, long>();

    while (number / 10 != 0)
    {
        if (result.Keys.Contains(number % 10))
        {
            result[number % 10]++;
            number = number / 10;
            continue;
        }
        result.Add(number % 10, 1);
        number = number / 10;
    }

    if (result.Keys.Contains(number))
    {
        result[number]++;
    }
    else
    {
        result.Add(number, 1);
    }

    foreach (var item in result)
    {
        Console.WriteLine($"key:{item.Key}, value:{item.Value}");
    }
}