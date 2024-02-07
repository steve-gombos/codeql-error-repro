using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.WebUtilities;

namespace ClassLibrary2;

public class Class1<T> : XmlSerializerInputFormatter
{
    private readonly MvcOptions _options;
    private readonly int _maxErrors;

    public Class1(MvcOptions options, int maxErrors = 5) : base(options)
    {
        _options = options;
        _maxErrors = maxErrors;
    }

    protected override bool CanReadType(Type type)
    {
        var schemas = SchemaAccess.GetSchemaSet(type);
        return schemas.Count > 0;
    }
    
    protected XmlReader CreateXmlReader(Stream readStream, Encoding encoding, InputFormatterContext context)
    {
        var errors = new List<string>();
        var schemas = SchemaAccess.GetSchemaSet<T>();
        var settings = new XmlReaderSettings();
        settings.DtdProcessing = DtdProcessing.Prohibit;
        settings.ValidationType = ValidationType.Schema;
        settings.Schemas.Add(schemas);
        settings.ValidationEventHandler += (sender, args) =>
        {
            errors.Add(args.Message);
            context.ModelState.AddModelError("error", args.Message);
        };
        return XmlReader.Create(readStream, settings);
    }
    
    public async override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (encoding == null) throw new ArgumentNullException(nameof(encoding));
        
        var request = context.HttpContext.Request;
        var suppressInputFormatterBuffering = _options?.SuppressInputFormatterBuffering ?? false;

        if (!request.Body.CanSeek && !suppressInputFormatterBuffering)
        {
            request.EnableBuffering();
            await request.Body.DrainAsync(CancellationToken.None);
            request.Body.Seek(0L, SeekOrigin.Begin);
        }
        
        try
        {
            using(var xmlReader = CreateXmlReader(request.Body, encoding, context))
            {
                var type = GetSerializableType(context.ModelType);
                var serializer = GetCachedSerializer(type);
                var deserializedObject = serializer.Deserialize(xmlReader);

                if (type != context.ModelType)
                {
                    if (deserializedObject is IUnwrappable unwrappable)
                        deserializedObject = unwrappable.Unwrap(context.ModelType);
                }

                return await InputFormatterResult.SuccessAsync(deserializedObject);
            }
        }
        catch (Exception)
        {
            return await InputFormatterResult.FailureAsync();
        }
    }
}