using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace ClassLibrary2;

public class Class1 : XmlSerializerInputFormatter
{
    public Class1(MvcOptions options) : base(options)
    {
    }

    public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (encoding == null) throw new ArgumentNullException(nameof(encoding));
        
        try
        {
            var request = context.HttpContext.Request;
            var settings = new XmlReaderSettings();
            using var xmlReader = XmlReader.Create(request.Body, settings);
            var type = GetSerializableType(context.ModelType);
            var serializer = GetCachedSerializer(type);
            var deserializedObject = serializer.Deserialize(xmlReader);

            return InputFormatterResult.SuccessAsync(deserializedObject);
        }
        catch (Exception)
        {
            return InputFormatterResult.FailureAsync();
        }
    }
}