using AbstractFactory;

// 现代风格的游戏对象构造
Console.WriteLine("开始构造现代风格的游戏对象");
var modernGame = new GameManager(new ModernFactory());
modernGame.BulidGameFacility();
Console.WriteLine();

// 经典风格的游戏对象构造
Console.WriteLine("开始构造经典风格的游戏对象");
var classicGame = new GameManager(new ClassicFactory());
classicGame.BulidGameFacility();