using System.IO.Compression;

namespace sample_application;

public class BundleWalker
{
    private static string[] _tbfNames = {
        "artifactory-request.",
        "./artifactory-request.",
        "archived/artifactory-request.",
        "./archived/artifactory-request."
    };

    private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private List<Action<IRequestLogEntry>> _callbacks = new List<Action<IRequestLogEntry>>();

    public void AddCallback(Action<IRequestLogEntry> cb)
    {
        _callbacks.Add(cb);
    }

    public void WalkSupportBundle(string supportBundlePath)
    {
        _logger.Debug("Processing Support Bundle: {path}", supportBundlePath);
        using (ZipArchive sb = ZipFile.OpenRead(supportBundlePath))
        {
            foreach (ZipArchiveEntry sbEntry in sb.Entries)
            {
                _logger.Debug("Checking sbEntry filename: {filename}", sbEntry.FullName);
                if (sbEntry.FullName.Contains("artifactory/") && sbEntry.FullName.EndsWith(".zip"))
                {
                    WalkPerNodeZip(sbEntry);
                }
            }
        }
    }

    private void WalkPerNodeZip(ZipArchiveEntry perNodeZip)
    {
        _logger.Info("Walking per node zip file: {filename}", perNodeZip.FullName);
        // These are the "per node" zip files
        using (ZipArchive sbi = new ZipArchive(perNodeZip.Open(), ZipArchiveMode.Read))
        {
            foreach (ZipArchiveEntry sbiEntry in sbi.Entries)
            {
                _logger.Debug("Checking sbiEntry filename: {filename}", sbiEntry.FullName);
                if (sbiEntry.FullName.Contains("artifactory/logs/"))
                {
                    List<string> sbifContentLines = new List<string>();
                    // Version 7.* Logs
                    if (sbiEntry.FullName.Contains("artifactory-request") && !sbiEntry.FullName.Contains("-out"))
                    {
                        _logger.Info("Version 7.* Log File in Internal ZIP File: {filename}", sbiEntry.Name);
                        if (sbiEntry.FullName.EndsWith(".log"))
                        {
                            // Regular old Log File
                            using (StreamReader sr = new StreamReader(sbiEntry.Open()))
                            {
                                while (!sr.EndOfStream)
                                {
                                    sbifContentLines.Add(sr.ReadLine());
                                }
                            }
                        }
                        else if(sbiEntry.FullName.EndsWith(".log.gz"))
                        {
                            // GZip Compressed Log File
                            using (GZipStream gzStream = new GZipStream(sbiEntry.Open(), CompressionMode.Decompress))
                            {
                                // using (StreamReader sr = new StreamReader(gzStream))
                                // {
                                //     while (!sr.EndOfStream)
                                //     {
                                //         sbifContentLines.Add(sr.ReadLine());
                                //     }
                                // }
                                sbifContentLines = ReadLinesFromStream(gzStream);
                            }
                        }
                        ProcessLinesSeven(perNodeZip.FullName, sbiEntry.FullName, sbifContentLines);
                    }
                    // Version 6.* Logs
                    if (sbiEntry.FullName.Contains("/request."))
                    {
                        //
                        if (sbiEntry.FullName.EndsWith(".log"))
                        {
                            // Regular old Log File
                            using (StreamReader sr = new StreamReader(sbiEntry.Open()))
                            {
                                while (!sr.EndOfStream)
                                {
                                    sbifContentLines.Add(sr.ReadLine());
                                }
                            }
                        }
                        ProcessLinesSix(perNodeZip.FullName, sbiEntry.FullName, sbifContentLines);
                    }
                }
            }
        }
    }

    private List<string> ReadLinesFromStream(Stream stream)
    {
        List<string> result = new List<string>();
        using StreamReader sr = new StreamReader(stream);
        while (!sr.EndOfStream)
        {
            result.Add(sr.ReadLine());
        }
        return result;
    }

    // FIXME: Add a ReadLinesFromGZipStream method here?

    private void ProcessLinesSeven(string node, string name, IEnumerable<string> lines)
    {
        _logger.Info("Processing {num} lines", lines.Count());
        foreach (string line in lines)
        {
            if (line.StartsWith("dt."))
            {
                // DynaTrace Log
                // FIXME: Add the DynaTrace info here.
                string[] dt_chunks = line.Split(",");
                foreach (string dt_chunk in dt_chunks)
                {
                    if (dt_chunk.StartsWith(" dt.entity.host:  "))
                    {
                        ProcessLineSeven(node, name, dt_chunk.Substring(18));
                    }
                }
            }
            else
            {
                // Normal Artifactory Request Log
                ProcessLineSeven(node, name, line);
            }
        }
    }

    private void ProcessLineSeven(string node, string name, string line)
    {
        try
        {
            RequestLogEntrySeven rle = new RequestLogEntrySeven(node, name, line);
            foreach (Action<IRequestLogEntry> cb in _callbacks)
            {
                cb(rle);
            }
        }
        catch (Exception e)
        {
            _logger.Warn(e, "Unable to process log line: {line}", line);
        }

    }

    private void ProcessLinesSix(string node, string name, IEnumerable<string> lines)
    {
        // do stuff
        _logger.Info("Processing {num} lines", lines.Count());
        foreach (string line in lines)
        {
            // Normal Artifactory Request Log
            ProcessLineSix(node, name, line);
        }
    }

    private void ProcessLineSix(string node, string name, string line)
    {
        try
        {
            RequestLogEntrySix rle = new RequestLogEntrySix(node, name, line);
            foreach (Action<IRequestLogEntry> cb in _callbacks)
            {
                cb(rle);
            }
        }
        catch (Exception e)
        {
            _logger.Warn(e, "Unable to process log line: {line}", line);
        }

    }
}
