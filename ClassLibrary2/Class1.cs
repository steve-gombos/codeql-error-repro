using System.Xml;

namespace ClassLibrary2;

public class Class1
{
    public void Test()
    {
        var settings = new XmlReaderSettings();
        var reader = XmlReader.Create("test", settings);
    }
}