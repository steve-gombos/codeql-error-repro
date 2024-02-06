using System.Xml;

namespace ClassLibrary2;

public class Class1
{
    public void Test(Stream stream)
    {
        var settings = new XmlReaderSettings();
        var reader = XmlReader.Create(stream, settings);
    }
}