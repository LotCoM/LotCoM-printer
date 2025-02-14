using System.Drawing;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Labels;

public static class LabelGenerator {
    
    /// <summary>
    /// Generates a Label image object that mirrors the appearance of the physical Label.
    /// Composition of Asynchronous tasks.
    /// </summary>
    /// <param name="LabelHeader">The Large text to include on the Label's top-left corner.</param>
    /// <param name="LabelData">The Data to include in the Label's QR Code and on the Label in plain-text.</param>
    /// <remarks>If the Arial font is not found in the System, the method will throw SystemException.</remarks>
    /// <returns></returns>
    /// <exception cref="SystemException"></exception>
    public static async Task<Bitmap> GenerateLabelAsync(string LabelHeader, List<string> LabelData) {    
        // create a new Label
        Label? NewLabel;
        try {
            NewLabel = new Label();
        // the Label failed to configure its fonts from the System
        } catch (SystemException _ex) {
            throw new LabelBuildException($"Failed to construct new Label due to the following exception:\n{_ex.Message}");
        }
        // copy only the values of the data fields to the QR Code data
        List<string> QRCodeData = [];
        foreach(string _field in LabelData) {
            // split the string at the colon and save the second segment (field value)
            QRCodeData.Add(_field.Split(": ")[1]);
        }
        // generate a new QR Code from the Label's data
        QRCode? LabelCode;
        try {
            LabelCode = new QRCode(QRCodeData);
        } catch (ArgumentException _ex) {
            throw new LabelBuildException($"Failed to construct new Label due to the following exception:\n{_ex.Message}");
        }
        // apply the header, the QR Code, and the Label Data to the Label
        await NewLabel.AddHeaderAsync(LabelHeader);
        await NewLabel.AddQRCodeAsync(LabelCode);
        await NewLabel.AddLabelFieldsAsync(LabelData);
        // return the Label image
        return NewLabel.GetImage();
    }
}