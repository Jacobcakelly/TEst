using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Core.Drawing;

namespace sample_application;

public class PdfChunkTransferGraphs
{
    private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private CalcTransfer _calcTransfer { get; }

    public PdfChunkTransferGraphs(CalcTransfer calcTransfer)
    {
        _calcTransfer = calcTransfer;
    }

    public void DefineParagraph(Document document)
    {
        _logger.Debug("Adding Transfer Graphs");

        PageSetup pageSetup = document.LastSection.PageSetup;
        // FIXME: Figure out how to not do implicit Unit -> float and float -> Unit convertions.
        float graphWidth = pageSetup.PageWidth - pageSetup.LeftMargin - pageSetup.RightMargin;
        float graphHeight = (pageSetup.PageHeight - pageSetup.TopMargin - pageSetup.BottomMargin) / 2;

        _logger.Debug("graphWidth: {width}, graphHeight: {height}", graphWidth, graphHeight);

        Dictionary<string, Dictionary<string, ulong>> dataTransferByDay = _calcTransfer.TransferGraphDataByDay();
        PlotModel graphTransferByDay = GraphTransfer.TransferByTick(dataTransferByDay, DateTimeIntervalType.Days, "Total Transfer By Day");

        // FIXME: Is this where we want to export the PNG & SVG versions of the graph?

        MemoryStream stream = new MemoryStream();

        // FIXME: Do we want the graph to take the entire half page, or have some space below it for text?
        int imageWidth = 1050; // roughly 7 inches at 150 dots per inch, FIXME: Figure out ratio to graphWidth above.
        int imageHeight = (int)(imageWidth * graphHeight / graphWidth); // using the ratio from the page

        PngExporter pngExporter = new PngExporter { Width = imageWidth, Height = imageHeight };
        pngExporter.Export(graphTransferByDay, stream);

        string b64Image = "base64:" + Convert.ToBase64String(stream.ToArray());

        Image image = new Image(b64Image);
        image.Width = graphWidth;
        //image.Height = graphHeight;
        image.LockAspectRatio = true;

        document.LastSection.Add(image);
    }
}
