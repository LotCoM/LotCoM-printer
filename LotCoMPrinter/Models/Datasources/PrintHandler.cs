using System.Drawing.Printing;
using System.Drawing;
using LotCoMPrinter.Models.Services;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Datasources;

# pragma warning disable CA1416 // Validate platform compatibility

/// <summary>
/// Allows the printing of a Label.
/// </summary>
public static class PrintHandler
{
    /// <summary>
    /// Loads the Label Image onto the PrintDocument.
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    private static void LoadLabelImage(object Sender, PrintPageEventArgs e, Label label)
    {
        // draw the Label Image onto the PrintDocument Graphic
        Bitmap Resized = Resizer.ResizeImage(label.GetImage(), 350, 350);
        e.Graphics!.DrawImage(Resized, new System.Drawing.Point(0, 0));
    }

    /// <summary>
    /// Prints the Label passed to the PrintHandler.
    /// </summary>
    /// <exception cref="PrintRequestException"></exception>
    public static async Task<bool> PrintLabelAsync(Label Label)
    {
        return await Task.Run(() =>
        {
            // create a new PrintDocument, retrieve the Default Printer, and set the Document to use that Printer
            PrintDocument PrintDoc = new PrintDocument();
            PrinterSettings Defaults = new PrinterSettings();
            string DefaultPrinterName = Defaults.PrinterName;
            PrintDoc.PrinterSettings.PrinterName = DefaultPrinterName;
            // add the Label Image Loading logic onto the PrintPage handler
            try
            {
                PrintDoc.PrintPage += (sender, e) => LoadLabelImage(sender, e, Label);
            }
            catch (Exception _ex)
            {
                throw new PrintRequestException($"The Print Request could not be completed due to the following exception:\n{_ex.Message}.");
            }
            // start printing the Document
            try
            {
                PrintDoc.Print();
            }
            catch (InvalidPrinterException)
            {
                throw new PrintRequestException("There was an error accessing the Default Printer. Please see management to resolve this issue.");
            }
            catch (Exception _ex)
            {
                throw new PrintRequestException($"The Print Request could not be completed due to the following exception:\n{_ex.Message}.");
            }
            return true;
        });
    }
}
# pragma warning restore CA1416 // Validate platform compatibility