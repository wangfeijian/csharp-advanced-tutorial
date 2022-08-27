using FactoryMethod;

var carTest = new CarTestFramework();
carTest.BuildTestContext(new HongqiCarFactory());
carTest.DoTest(new HongqiCarFactory());

carTest.BuildTestContext(new BydCarFactory());
carTest.DoTest(new BydCarFactory());