using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;

namespace sample_application;

public class PdfDocument
{
    private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public CalcTransfer? CalcTransferForGraphs { get; set; }

    public Document CreateDocument()
    {
        Document document = new Document();

        document.Info.Author = "JFrog";
        document.Info.Title = "JFrog Support Bundle Analysis";
        document.Info.Subject = "Detailed analysis of a JFrog support bundle.";

        PdfStyles.DefineStyles(document);

        DefineContentSection(document);

        // Adding Contents to Document
        if (CalcTransferForGraphs != null)
        {
            PdfChunkTransferGraphs pdfChunkTransferGraphs = new PdfChunkTransferGraphs(CalcTransferForGraphs);
            pdfChunkTransferGraphs.DefineParagraph(document);
        }

        return document;
    }

    private void DefineContentSection(Document document)
    {
        Section section = document.AddSection();
        
        section.PageSetup = document.DefaultPageSetup.Clone();
        //section.PageSetup.PageFormat = PageFormat.Letter;
        section.PageSetup.PageWidth = Unit.FromInch(8.5); // FIXME: Move this to a better somewhere.
        section.PageSetup.PageHeight = Unit.FromInch(11); // FIXME: Move this to a better somewhere.

        //section.PageSetup.OddAndEvenPagesHeaderFooter = true;
        section.PageSetup.StartingNumber = 1;

        HeaderFooter header = section.Headers.Primary;
        Paragraph hparagraph = header.AddParagraph();
        hparagraph.AddText("Support Bundle Analysis");
        hparagraph.AddTab();

        string assetDirectory = Path.GetFullPath("./assets"); // FIXME: Define asset directory somewhere.
        _logger.Debug("Assets Path: {path}", assetDirectory);
        Image image = hparagraph.AddImage(Path.Join(assetDirectory, "text1.png"));
        image.Width = Unit.FromCentimeter(1);

        Border hrBorder = new Border();
        hrBorder.Width = Unit.FromPoint(1);
        hrBorder.Color = Colors.Black;

        hparagraph.Format.Borders.Bottom = hrBorder.Clone();

        HeaderFooter footer = section.Footers.Primary;
        Paragraph fparagraph = footer.AddParagraph();
        fparagraph.Format.Borders.Top = hrBorder.Clone();

        // Create a paragraph with centered page number. See definition of style "Footer".
        fparagraph.AddTab();
        fparagraph.AddPageField();
    }
}
