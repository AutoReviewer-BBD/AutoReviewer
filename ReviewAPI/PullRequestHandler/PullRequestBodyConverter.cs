using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.PullRequestHandler;

public class PullRequestBodyConverter : JsonConverter<PullRequestBody>
{
    public override PullRequestBody Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
        JsonElement root = jsonDocument.RootElement;

        return new PullRequestBody
        {
            title = root.GetProperty("title").GetString(),
            head = root.GetProperty("head").GetString(),
            Base = root.GetProperty("base").GetString(),
            body = root.GetProperty("body").GetString()
        };
    }

    public override void Write(Utf8JsonWriter writer, PullRequestBody value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("title", value.title);
        writer.WriteString("head", value.head);
        writer.WriteString("base", value.Base);
        writer.WriteString("body", value.body);

        writer.WriteEndObject();
    }
}