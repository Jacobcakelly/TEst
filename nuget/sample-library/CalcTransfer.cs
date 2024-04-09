using System;
using System.Collections.Generic;

namespace sample_library;

public class CalcTransfer : ICalc
{
    public DateTime FirstTimestamp { get; private set; } = DateTime.MaxValue;
    public DateTime LastTimestamp { get; private set; } = DateTime.MinValue;

    private readonly HashSet<string> _nodes = new HashSet<string>();
    private readonly Dictionary<string, DateTime> _byNodeFirstTimestamp = new Dictionary<string, DateTime>();
    private readonly Dictionary<string, DateTime> _byNodeLastTimestamp = new Dictionary<string, DateTime>();
    private readonly Dictionary<string, ulong> _byNodeDown = new Dictionary<string, ulong>();
    private readonly Dictionary<string, ulong> _byNodeUp = new Dictionary<string, ulong>();
    private readonly Dictionary<string, ulong> _byDayDown = new Dictionary<string, ulong>();
    private readonly Dictionary<string, ulong> _byDayUp = new Dictionary<string, ulong>();
    private readonly Dictionary<string, ulong> _byHourDown = new Dictionary<string, ulong>();
    private readonly Dictionary<string, ulong> _byHourUp = new Dictionary<string, ulong>();
    private readonly Dictionary<string, Dictionary<string, ulong>> _byNodeByDayDown = new Dictionary<string, Dictionary<string, ulong>>();
    private readonly Dictionary<string, Dictionary<string, ulong>> _byNodeByDayUp = new Dictionary<string, Dictionary<string, ulong>>();
    private readonly Dictionary<string, Dictionary<string, ulong>> _byNodeByHourDown = new Dictionary<string, Dictionary<string, ulong>>();
    private readonly Dictionary<string, Dictionary<string, ulong>> _byNodeByHourUp = new Dictionary<string, Dictionary<string, ulong>>();

    public ulong EntryCount { get; private set; }
    public ulong TotalDownBytes { get; private set; }
    public ulong TotalUpBytes { get; private set; }

    public void AddLogEntry(IRequestLogEntry entry)
    {
        EntryCount++;

        // Check timestamps
        if (FirstTimestamp.CompareTo(entry.Timestamp) > 0)
        {
            // entry.Timestamp is earlier than FirstTimestamp
            FirstTimestamp = entry.Timestamp;
        }

        if (LastTimestamp.CompareTo(entry.Timestamp) < 0)
        {
            // entry.Timestamp is later than LastTimestamp
            LastTimestamp = entry.Timestamp;
        }

        // Calc day and hour strings
        string dayStr = entry.Timestamp.ToString("yyyy-MM-dd");
        string hourStr = entry.Timestamp.ToString("yyyy-MM-dd HH:00");

        // Add to totals
        TotalDownBytes += entry.DownSizeBytes;
        TotalUpBytes += entry.UpSizeBytes;

        // Add to possible nodes list
        // NOTE: Abusing the result of the "add to set" operation to
        //       create other dictionaries when successful (not already in set).
        if (_nodes.Add(entry.NodeName))
        {
            _byNodeDown[entry.NodeName] = 0;
            _byNodeUp[entry.NodeName] = 0;
            _byNodeFirstTimestamp[entry.NodeName] = DateTime.MaxValue;
            _byNodeLastTimestamp[entry.NodeName] = DateTime.MinValue;
            _byNodeByDayDown[entry.NodeName] = new Dictionary<string, ulong>();
            _byNodeByDayUp[entry.NodeName] = new Dictionary<string, ulong>();
            _byNodeByHourDown[entry.NodeName] = new Dictionary<string, ulong>();
            _byNodeByHourUp[entry.NodeName] = new Dictionary<string, ulong>();
        }

        // Add bytes for node
        _byNodeDown[entry.NodeName] += entry.DownSizeBytes;
        _byNodeUp[entry.NodeName] += entry.UpSizeBytes;

        // Add bytes for day and node
        if (!_byDayDown.ContainsKey(dayStr))
        {
            _byDayDown[dayStr] = 0;
            _byDayUp[dayStr] = 0;
        }
        if (!_byNodeByDayDown[entry.NodeName].ContainsKey(dayStr))
        {
            _byNodeByDayDown[entry.NodeName][dayStr] = 0;
            _byNodeByDayUp[entry.NodeName][dayStr] = 0;
        }
        _byDayDown[dayStr] += entry.DownSizeBytes;
        _byDayUp[dayStr] += entry.UpSizeBytes;
        _byNodeByDayDown[entry.NodeName][dayStr] += entry.DownSizeBytes;
        _byNodeByDayUp[entry.NodeName][dayStr] += entry.UpSizeBytes;

        // Add bytes for hour and node
        if (!_byHourDown.ContainsKey(hourStr))
        {
            _byHourDown[hourStr] = 0;
            _byHourUp[hourStr] = 0;
        }
        if (!_byNodeByHourDown[entry.NodeName].ContainsKey(hourStr))
        {
            _byNodeByHourDown[entry.NodeName][hourStr] = 0;
            _byNodeByHourUp[entry.NodeName][hourStr] = 0;
        }
        _byHourDown[hourStr] += entry.DownSizeBytes;
        _byHourUp[hourStr] += entry.UpSizeBytes;
        _byNodeByHourDown[entry.NodeName][hourStr] += entry.DownSizeBytes;
        _byNodeByHourUp[entry.NodeName][hourStr] += entry.UpSizeBytes;
    }

    public TimeSpan Duration()
    {
        return LastTimestamp.Subtract(FirstTimestamp);
    }

    public TimeSpan Duration(string node)
    {
        // FIXME: Not sure the right way to handle a bad `node` value.
        //if (_nodes.Contains(node))
        return _byNodeLastTimestamp[node].Subtract(_byNodeFirstTimestamp[node]);
    }

    public (ulong down, ulong up) MeanAveragePerSecond(string node)
    {
        // FIXME: Should figure out the best way to raise an exception if node not in _nodes.
        double tDeltaSeconds = Duration(node).TotalSeconds;
        if (tDeltaSeconds < 1)
        {
            // FIXME: Handle this with exceptions when exceptions are figured out.
            // FIXME: Logger stuff here.
            return (down: 0, up: 0);
        }
        ulong transferDown = _byNodeDown[node];
        ulong transferUp = _byNodeUp[node];
        ulong resultDown = (ulong)(transferDown / tDeltaSeconds);
        ulong resultUp = (ulong)(transferUp / tDeltaSeconds);
        // FIXME: Logger stuff here.
        return (down: resultDown, up: resultUp);
    }

    public (ulong down, ulong up) MeanAveragePerDays()
    {
        // FIXME: Logger stuff here.
        ulong resultDown = 0;
        ulong resultUp = 0;
        foreach (string nodeName in _nodes)
        {
            (ulong nodeResultDown, ulong nodeResultUp) = MeanAveragePerSecond(nodeName);
            resultDown += (nodeResultDown * 3600 * 24);
            resultUp += (nodeResultUp * 3600 * 24);
        }
        // FIXME: Logger stuff here.
        return (down: resultDown, up: resultUp);
    }

    public (ulong down, ulong up) MeanAveragePerDays(string node)
    {
        (ulong nodeResultDown, ulong nodeResultUp) = MeanAveragePerSecond(node);
        ulong resultDown = (nodeResultDown * 3600 * 24);
        ulong resultUp = (nodeResultUp * 3600 * 24);
        // FIXME: Logger stuff here.
        return (down: resultDown, up: resultUp);
    }

    public (ulong down, ulong up) MeanAveragePerDays(uint days)
    {
        // FIXME: Logger stuff here.
        ulong resultDown = 0;
        ulong resultUp = 0;
        foreach (string nodeName in _nodes)
        {
            (ulong nodeResultDown, ulong nodeResultUp) = MeanAveragePerSecond(nodeName);
            resultDown += (nodeResultDown * 3600 * 24 * days);
            resultUp += (nodeResultUp * 3600 * 24 * days);
        }
        // FIXME: Logger stuff here.
        return (down: resultDown, up: resultUp);
    }

    public (ulong down, ulong up) MeanAveragePerDays(string node, uint days)
    {
        // FIXME: Logger stuff here.
        (ulong nodeResultDown, ulong nodeResultUp) = MeanAveragePerSecond(node);
        ulong resultDown = (nodeResultDown * 3600 * 24 * days);
        ulong resultUp = (nodeResultUp * 3600 * 24 * days);
        // FIXME: Logger stuff here.
        return (down: resultDown, up: resultUp);
    }
}
