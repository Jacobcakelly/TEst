using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace sample_library;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class RLEMethod
{
    public RLEMethod(string value)
    {
        Value = "GET";
        if (Methods.Contains(value))
        {
            Value = value;
        }
    }

    public string Value { get; private set; }
    public string[] Methods { get; } = ["GET", "PUT", "POST", "PATCH", "DELETE", "HEAD", "OPTION"];
    public bool Upload => (Value is "PUT" or "POST" or "PATCH");

    public static RLEMethod GET => new("GET");
    public static RLEMethod PUT => new("PUT");
    public static RLEMethod POST => new("POST");
    public static RLEMethod PATCH => new("PATCH");
    public static RLEMethod DELETE => new("DELETE");
    public static RLEMethod HEAD => new("HEAD");
    public static RLEMethod OPTION => new("OPTION");

    public override string ToString() { return Value; }
}
