using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
    public static async Task<Image<Rgba32>> GenerateLabel(string LabelHeader, List<string> LabelData) {    
        // create a new Label
        try {
            Label NewLabel = new Label();
            // generate a new QR Code from the Label's data
            QRCode LabelCode = new QRCode(LabelData);
            // apply the header, the QR Code, and the Label Data to the Label
            await NewLabel.AddHeaderAsync(LabelHeader);
            await NewLabel.AddQRCodeAsync(LabelCode);
            await NewLabel.AddLabelFieldsAsync(LabelData);
            // return the Label image
            return NewLabel.GetImage();
        // the Label failed to configure its fonts from the System
        } catch (FontFamilyNotFoundException) {
            throw new SystemException("The Arial font could not be found in the System.");
        }
    }
}