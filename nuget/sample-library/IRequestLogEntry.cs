using System;

namespace sample_library;

public interface IRequestLogEntry
{
    string NodeName { get; }
    string LogName { get; }
    DateTime Timestamp { get; }
    // FIXME: How to do C# enums?
    RLEMethod Method { get; }
    string Path { get; }
    // FIXME: How to do C# enums?
    string StatusCode { get; }
    uint UpSizeBytes { get; }
    uint DownSizeBytes { get; }
    uint DurationMs { get; }
}
