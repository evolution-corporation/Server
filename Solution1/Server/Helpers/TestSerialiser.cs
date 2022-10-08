using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server.Helpers;

public class TestSerializer: JsonConverter<DateTime>
{
    private readonly JsonSerializerOptions ConverterOptions;
    public TestSerializer(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return JsonSerializer.Deserialize<DateTime>(ref reader, ConverterOptions);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, value, ConverterOptions);
    }
}