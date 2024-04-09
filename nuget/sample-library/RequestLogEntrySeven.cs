using System;
using System.Globalization;

namespace sample_library;

public class RequestLogEntrySeven : IRequestLogEntry
{
    /* 
     * This is a single line of a request log for Artifactory Version 7.*
     * Example Lines:
     * - 2023-04-07T04:09:49.449Z|827ad60fcf158238|10.1.202.14|anonymous|GET|/centos/7.9.2009/os/x86_64/repodata/repomd.xml:properties|302|-1|0|0|Artifactory/7.46.10 74610900
     * - 2023-04-07T04:09:49.465Z|a1347a2e5ad0d45b|10.233.37.173|anonymous|GET|/api/pypi/dataengineering-pypi/packages/packages/ec/ab/a440db757401a1e8863c9abb374a77cb2884eda74ffbf555dedcf1fbe7f6/frozenlist-1.3.3-cp38-cp38-manylinux_2_5_x86_64.manylinux1_x86_64.manylinux_2_17_x86_64.manylinux2014_x86_64.whl|200|-1|161300|6|pip/21.0.1 {"ci":null,"cpu":"x86_64","distro":{"id":"focal","libc":{"lib":"glibc","version":"2.31"},"name":"Ubuntu","version":"20.04"},"implementation":{"name":"CPython","version":"3.8.10"},"installer":{"name":"pip","version":"21.0.1"},"openssl_version":"OpenSSL 1.1.1f  31 Mar 2020","python":"3.8.10","setuptools_version":"52.0.0","system":{"name":"Linux","release":"5.4.0-1096-aws"}}
     * - 2023-04-07T04:09:49.478Z|76af5d05747afb2c|10.1.202.14|anonymous|HEAD|/epel/7/x86_64/repodata/repomd.xml|200|-1|4851|2|Artifactory/7.46.10 74610900
     * - 2023-04-05T12:23:37.826Z|3e96f7737265e393|127.0.0.1|anonymous|GET|/api/v1/system/readiness|200|-1|0|1|JFrog-Router/7.42.0-1
     * - 2023-04-05T12:23:37.828Z|fe827b51f5d3ffb9|10.195.80.195|tokenservices-build|POST|/api/docker/tokenservices-docker/v2/key-manager/blobs/uploads/|202|0|0|0|buildkit/v0.11
     * - 2023-04-05T12:23:37.831Z|9bab4816c0b27021|10.195.80.195|tokenservices-build|HEAD|/api/docker/tokenservices-docker/v2/key-manager/blobs/sha256:f91dc3ff8ed55a0cfecf56a6287d4b9f7c0726e4b8269d7c312083d9da6aab87|404|-1|157|24|buildkit/v0.11
     */
    public string NodeName { get; }
    public string LogName { get; }
    public DateTime Timestamp { get; }
    public RLEMethod Method { get; }
    public string Path { get; }
    public string StatusCode { get; }
    public uint UpSizeBytes { get; }
    public uint DownSizeBytes { get; }
    public uint DurationMs { get; }

    public RequestLogEntrySeven(string nodeName, string logName, string inputLine)
    {
        NodeName = nodeName;
        LogName = logName;
        var inputLineSplit = inputLine.Split('|');
        Timestamp = DateTime.Parse(inputLineSplit[0], null, DateTimeStyles.RoundtripKind);
        Method = new RLEMethod(inputLineSplit[4]);
        Path = inputLineSplit[5];
        StatusCode = inputLineSplit[6];
        if (inputLineSplit[7] == "-1")
        {
            UpSizeBytes = 0;
        }
        else
        {
            UpSizeBytes = uint.Parse(inputLineSplit[7]);
        }
        DownSizeBytes = uint.Parse(inputLineSplit[8]);
        DurationMs = uint.Parse(inputLineSplit[9]);
    }
}
