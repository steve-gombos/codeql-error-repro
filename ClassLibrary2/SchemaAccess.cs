using System.Xml.Schema;

namespace ClassLibrary2;

public static class SchemaAccess
{
    public static XmlSchemaSet GetSchemaSet<T>()
    {
        var schemas = new XmlSchemaSet();
        return schemas;
    }
    
    public static XmlSchemaSet GetSchemaSet(Type type)
    {
        var schemas = new XmlSchemaSet();
        return schemas;
    }
}