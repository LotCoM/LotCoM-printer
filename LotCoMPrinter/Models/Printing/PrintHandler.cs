using System.Drawing.Printing;

namespace LotCoMPrinter.Models.Printing;

# pragma warning disable CA1416 // Validate platform compatibility
/// <summary>
/// Allows the printing of a Label Image.
/// </summary>
/// <param name="LabelImageStream">A stream of bytes representing the Label Image.</param>
public class PrintHandler(Stream LabelImageStream) {
    // private class attributes
    private readonly Stream _labelImageStream = LabelImageStream;

    /// <summary>
    /// Prints the Label passed to the PrintHandler.
    /// </summary>
    public async Task PrintLabelAsync() {
        await Task.Run(() => {
            // create a new PrintDocument to add the Label to
            PrintDocument PrintDoc = new PrintDocument();
            // retrieve the Default Printer Name from a fresh PrinterSettings object
            PrinterSettings Defaults = new PrinterSettings();
            string DefaultPrinterName = Defaults.PrinterName;
            // set the PrintDocument to print using the default printer
            PrintDoc.PrinterSettings.PrinterName = DefaultPrinterName;
            // add the Label Image Loading logic onto the PrintPage handler
            PrintDoc.PrintPage += LoadLabelImage;
            // start printing the Document
            PrintDoc.Print();
            Console.WriteLine("Printing Label");
        });
    }

    /// <summary>
    /// Loads the Label Image onto the PrintDocument.
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void LoadLabelImage(object Sender, PrintPageEventArgs e) {
        // convert the Label Image Bytestream to an Image
        System.Drawing.Image LabelImage = System.Drawing.Image.FromStream(_labelImageStream);
        // draw the Label Image onto the PrintDocument Graphic
        e.Graphics!.DrawImage(LabelImage, new System.Drawing.Point(0, 0));
    }
}
# pragma warning restore CA1416 // Validate platform compatibility