using MigraDoc.DocumentObjectModel;

namespace sample_application;

public class PdfStyles
{
    public static void DefineStyles(Document document)
    {
        // Get the predefined style Normal.
        Style style = document.Styles["Normal"] ?? throw new InvalidOperationException("Style Normal not found.");
        // Because all styles are derived from Normal, the next line changes the
        // font of the whole document. Or, more exactly, it changes the font of
        // all styles and paragraphs that do not redefine the font.
        style.Font.Name = "Segoe UI";


        // Title Page Styles
        style = document.Styles.AddStyle("Title", "Normal");
        style.Font.Name = "Times New Roman";
        style.Font.Size = Unit.FromPoint(16);
        style.Font.Color = Colors.Black;
        style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

        style = document.Styles.AddStyle("TitleGreen", "Title");
        style.Font.Color = Color.FromRgb(0x40, 0xBE, 0x46); // FIXME: Define JFrogGreen somewhere.
        style.Font.Bold = true;


        // Heading1 to Heading9 are predefined styles with an outline level. An outline level
        // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks)
        // in PDF.

        style = document.Styles["Heading1"]!;
        style.Font.Name = "Times New Roman";
        style.Font.Size = Unit.FromPoint(16);
        style.Font.Bold = true;
        style.Font.Color = Colors.Black;
        style.ParagraphFormat.PageBreakBefore = true;
        style.ParagraphFormat.SpaceAfter = 6;
        style.ParagraphFormat.KeepWithNext = true;


        // Chunk Totals Styles
        style = document.Styles.AddStyle("ChunkTotals", "Normal");
        style.Font.Name = "Times New Roman";
        style.Font.Size = Unit.FromPoint(12);
        style.Font.Color = Colors.Black;
        style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

        style = document.Styles.AddStyle("ChunkTotalsValue", "ChunkTotals");
        style.Font.Size = Unit.FromPoint(24);
        style.Font.Bold = true;

        style = document.Styles[StyleNames.Header]!;
        style.ParagraphFormat.AddTabStop(Unit.FromCentimeter(16), TabAlignment.Right);

        style = document.Styles[StyleNames.Footer]!;
        style.ParagraphFormat.AddTabStop(Unit.FromCentimeter(8), TabAlignment.Center);
    }
}
