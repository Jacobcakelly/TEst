using System.Diagnostics;
using CommandLine;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace sample_application;

internal class Program
{
    private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public class CommandLineOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Enable DEBUG level logging.")]
        public bool Verbose { get; set; }

        [Option("log_output_file", Required = false, HelpText = "Output logs to this file.")]
        public string? LogOutputFile { get; set; }

        [Option("output_prefix", Required = false, Default = "output", HelpText = "Prefix for the names of output files.")]
        public string OutputPrefix { get; set; }

        [Value(0, Required = true, MetaName = "support_bundle", HelpText = "The path to the support bundle to process.")]
        public required string SupportBundle { get; set; }
    }

    private static int Main(string[] args)
    {
        // Parser
        ParserResult<CommandLineOptions> parsed = Parser.Default.ParseArguments<CommandLineOptions>(args);
        if (parsed.Errors.Any())
        {
            // Parsing failed.  Help should be printed by default.
            return 1;
        }

        // Logging
        LoggingConfiguration loggingConfig = new NLog.Config.LoggingConfiguration();
        LogLevel loggingLevel = parsed.Value.Verbose ? LogLevel.Debug : LogLevel.Info;
        ColoredConsoleTarget loggingTargetConsole = new NLog.Targets.ColoredConsoleTarget("ConsoleLog");
        loggingTargetConsole.Layout = "${longdate}|${level:uppercase=true}|${callsite}|${message}";
        // FIXME: If a file name is set, output info and debug levels to the file instead of console,
        //        and error still goes to console.
        loggingConfig.AddRule(loggingLevel, LogLevel.Fatal, loggingTargetConsole);
        NLog.LogManager.Configuration = loggingConfig;

        // Environment
        _logger.Debug("Args: {@args}", parsed);
        
        // Setup Stopwatch Timers
        Stopwatch watchParseSupportBundle = new Stopwatch();
        Stopwatch watchGenerateDataOutputs = new Stopwatch();
        Stopwatch watchGeneratePdf = new Stopwatch();
        Stopwatch watchTotal = new Stopwatch();
        watchTotal.Start();

        // Parse Support Bundle
        watchParseSupportBundle.Start();
        string supportBundle = Path.GetFullPath(parsed.Value.SupportBundle);
        // FIXME: Handle file doesn't exist exception.

        // - Setup Calcs
        CalcTransfer calcTransfer = new CalcTransfer();

        // - Create Bundle Walker
        BundleWalker bundleWalker = new BundleWalker();
        bundleWalker.AddCallback(calcTransfer.AddLogEntry);

        // - Walk the support bundle
        bundleWalker.WalkSupportBundle(supportBundle);

        watchParseSupportBundle.Stop();

        // Verify Data Imported
        // FIXME: Check counts for calcRequests too
        // FIXME: Skipping the analysis to save time in development
        if ((calcTransfer.EntryCount < 1))
        {
            _logger.Error("No request logs were imported.  Check support bundle.");
            throw new Exception("NO REQUEST LOG ENTRIES");
        }

        // - CalcTransfer
        _logger.Debug("Transfer Nodes: {nodes}", calcTransfer.Nodes);
        _logger.Debug("Transfer Total Entries: {entries}", calcTransfer.EntryCount);
        _logger.Debug("Transfer Total Down: {bytesDown}, Up: {bytesUp}", calcTransfer.TotalDownBytes, calcTransfer.TotalUpBytes);
        _logger.Debug("Transfer By Day (down, up): {bytes}", calcTransfer.MeanAveragePerDays());

        // Generate Data Outputs
        watchGenerateDataOutputs.Start();

        DateTime test1 = DateTime.Parse("2023-04-05 12:34");
        DateTime test2 = DateTime.Parse("2023-04-06 13:00");
        DateTime test3 = DateTime.Parse("2023-04-07");

        _logger.Debug("DateTime Test1: {datetime}", test1);
        _logger.Debug("DateTime Test2: {datetime}", test2);
        _logger.Debug("DateTime Test3: {datetime}", test3);

        watchGenerateDataOutputs.Stop();

        // Generate PDF, CSVs, SVGs, PNGs
        watchGeneratePdf.Start();

        // FIXME: Do better Path handling here.
        string outputDirectory = Path.GetFullPath(".");
        string pdfFileName = Path.Join(outputDirectory, parsed.Value.OutputPrefix);
        PdfGenerator pdfgen = new PdfGenerator(pdfFileName);
        pdfgen.CalcTransferForGraphs = calcTransfer;
        pdfgen.GeneratePdf();

        watchGeneratePdf.Stop();

        // Calculate Times
        watchTotal.Stop();
        _logger.Debug("Support Bundle Parsing Time: {timespan}", watchParseSupportBundle.Elapsed);
        _logger.Debug("Data Generation Time: {timespan}", watchGenerateDataOutputs.Elapsed);
        _logger.Debug("PDF Generation Time: {timespan}", watchGeneratePdf.Elapsed);
        _logger.Debug("Total Time: {timespan}", watchTotal.Elapsed);

        // Happy Return
        NLog.LogManager.Shutdown();
        return 0;
    }
}
