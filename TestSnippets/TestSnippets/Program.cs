using TestSnippets;

//使用XmlDocument操作
//XmlReadWrite.CreateXmlFile("d:/test.xml", "Test");
//XmlReadWrite.AddXmlNodeAndSetAttribute("d:/test.xml", "Test", "TestParam", "Param");
//XmlReadWrite.GetXmlNodeAttributeValue("d:/test.xml", "/Test/TestParam", "name");
//XmlReadWrite.GetXmlNodeAttributeValue("d:/test.xml", "/Test/TestParam", "version");

//使用XDocument操作
//XmlReadWriteUseXDocument.CreateXmlFile("d:/testX.xml", "TestXml");
//XmlReadWriteUseXDocument.AddXmlNodeAndSetAttribute("d:/testX.xml", "TestXml", "TestParm","Param");
XmlReadWriteUseXDocument.GetXmlNodeAttributeValue("d:/testX.xml", "Param", "name");
XmlReadWriteUseXDocument.GetXmlNodeAttributeValue("d:/testX.xml", "Param", "version");