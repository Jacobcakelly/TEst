using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
using PdfDocument = sample_application.PdfDocument;

namespace sample_application;

public class PdfGenerator
{
    private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public string OutputFilename { get; set; }

    public CalcTransfer? CalcTransferForGraphs { get; set; }

    public PdfGenerator(string outputFilename)
    {
        OutputFilename = outputFilename;
    }

    public void GeneratePdf()
    {
        // NET6FIX
        if (PdfSharp.Capabilities.Build.IsCoreBuild)
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();

        // Create a MigraDoc document.
        PdfDocument pdfDocument = new PdfDocument();
        pdfDocument.CalcTransferForGraphs = CalcTransferForGraphs;
        Document document = pdfDocument.CreateDocument();

        PdfDocumentRenderer renderer = new PdfDocumentRenderer()
        {
            Document = document
        };

        renderer.RenderDocument();

        // Save the document...
        string filename = string.Concat(OutputFilename, ".pdf");
        renderer.PdfDocument.Save(filename);
    }
}