using System.Drawing;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Datasources;

public static class LabelGenerator 
{
    /// <summary>
    /// Generates a Label image object that mirrors the appearance of the physical Basket Label.
    /// Composition of Asynchronous tasks.
    /// </summary>
    /// <param name="Capture">An InterfaceCapture object.</param>
    /// <param name="LabelHeader">The Large text to include on the Label's top-left corner.</param>
    /// <returns></returns>
    /// <exception cref="LabelBuildException"></exception>
    public static async Task<Bitmap> GenerateLabelAsync(InterfaceCapture Capture, string LabelHeader) 
    {
        // create a new Label
        BasketLabel? Label;
        try 
        {
            Label = new BasketLabel();
        // the Label failed to configure its fonts from the System
        } 
        catch (SystemException _ex) 
        {
            throw new LabelBuildException($"Failed to construct new Label due to the following exception:\n{_ex.Message}");
        }
        // create a List of Label Fields from the Capture and generate a QR Code from that List
        List<string> QRCodeData = Capture.FormatAsQRCodeData();
        QRCode? LabelCode = new QRCode(QRCodeData);
        // apply the header, the QR Code, and the Label Data to the Label
        await Label.AddHeaderAsync(LabelHeader);
        await Label.AddPartNameAsync(Capture.Part.PartName);
        await Label.AddQRCodeAsync(LabelCode);
        await Label.AddLabelFieldsAsync(Capture.FormatAsLabelBodyText());
        // return the Label image
        return Label.GetImage();
    }
}