using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOTProjectApp;

// Provider of global JSON deserialization/serialization settings
public class JsonOptionsProvider
{
    private static Lazy<JsonOptionsProvider> _singleton = new(() => new JsonOptionsProvider());
    public static JsonOptionsProvider Singleton => _singleton.Value;
    
    private JsonSerializerOptions _options;
    private JsonOptionsProvider()
    {
        _options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };
    }

    public JsonSerializerOptions GetGlobalOptions() => _options;
}