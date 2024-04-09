namespace sample_library;

public interface ICalc
{
    ulong EntryCount { get; }

    void AddLogEntry(IRequestLogEntry entry);
}