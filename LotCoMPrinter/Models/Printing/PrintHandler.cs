using System.Drawing.Printing;
using System.Drawing;
using LotCoMPrinter.Models.Services;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Printing;

# pragma warning disable CA1416 // Validate platform compatibility
/// <summary>
/// Allows the printing of a Label Image.
/// </summary>
/// <param name="LabelImageStream">A stream of bytes representing the Label Image.</param>
public class PrintHandler(Bitmap LabelImage) {
    // private class attributes
    private readonly Bitmap _labelImage = LabelImage;

    /// <summary>
    /// Prints the Label passed to the PrintHandler.
    /// </summary>
    /// <exception cref="PrintRequestException"></exception>
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
            try {
                PrintDoc.PrintPage += LoadLabelImage;
            // handle exceptions thrown by the addition of the print handler behavior
            } catch (Exception _ex) {
                throw new PrintRequestException(
                    $"The Print Request could not be completed due to the following exception:\n{_ex.Message}."
                );
            }
            // start printing the Document
            try {
                PrintDoc.Print();
            // the default printer pulled from the Default PrinterSettings object was invalid
            } catch (InvalidPrinterException) {
                throw new PrintRequestException(
                    "There was an error accessing the Default Printer. Please see management to resolve this issue."
                );
            // there was some unexpected printing exception
            } catch (Exception _ex) {
                throw new PrintRequestException(
                    $"The Print Request could not be completed due to the following exception:\n{_ex.Message}."
                );
            }
        });
    }

    /// <summary>
    /// Loads the Label Image onto the PrintDocument.
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private void LoadLabelImage(object Sender, PrintPageEventArgs e) {
        // draw the Label Image onto the PrintDocument Graphic
        Bitmap Resized = Resizer.ResizeImage(_labelImage, 350, 350);
        e.Graphics!.DrawImage(Resized, new System.Drawing.Point(0, 0));
    }
}
# pragma warning restore CA1416 // Validate platform compatibility