using System.Drawing;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Validators;

namespace LotCoMPrinter.Models.Labels;

public static class LabelGenerator {
    /// <summary>
    /// Generates a Full Label image object that mirrors the appearance of the physical Full Basket Label.
    /// Composition of Asynchronous tasks.
    /// </summary>
    /// <param name="Capture">An InterfaceCapture object.</param>
    /// <param name="LabelHeader">The Large text to include on the Label's top-left corner.</param>
    /// <returns></returns>
    /// <exception cref="LabelBuildException"></exception>
    public static async Task<Bitmap> GenerateFullLabelAsync(InterfaceCapture Capture, string LabelHeader) {
        // create a new Label
        FullLabel? NewLabel;
        try {
            NewLabel = new FullLabel();
        // the Label failed to configure its fonts from the System
        } catch (SystemException _ex) {
            throw new LabelBuildException($"Failed to construct new Label due to the following exception:\n{_ex.Message}");
        }
        // create a List of Label Fields from the Capture and generate a QR Code from that List
        List<string> QRCodeData = Capture.FormatAsQRCodeData();
        QRCode? LabelCode = new QRCode(QRCodeData);
        // apply the header, the QR Code, and the Label Data to the Label
        await NewLabel.AddHeaderAsync(LabelHeader);
        await NewLabel.AddPartNameAsync(Capture.SelectedPart!.PartName);
        await NewLabel.AddQRCodeAsync(LabelCode);
        await NewLabel.AddLabelFieldsAsync(Capture.FormatAsLabelBodyText());
        // return the Label image
        return NewLabel.GetImage();
    }

    /// <summary>
    /// Generates a Partial Label image object that mirrors the appearance of the physical Partial Basket Label.
    /// Composition of Asynchronous tasks.
    /// </summary>
    /// <param name="Capture">An InterfaceCapture object.</param>
    /// <param name="LabelHeader">The Large text to include on the Label's top-left corner.</param>
    /// <returns></returns>
    /// <exception cref="LabelBuildException"></exception>
    public static async Task<Bitmap> GeneratePartialLabelAsync(InterfaceCapture Capture, string LabelHeader) {
        // create a new Label
        PartialLabel? NewLabel;
        try {
            NewLabel = new PartialLabel();
        // the Label failed to configure its fonts from the System
        } catch (SystemException _ex) {
            throw new LabelBuildException($"Failed to construct new Label due to the following exception:\n{_ex.Message}");
        }
        // create a List of Label Fields from the Capture and generate a QR Code from that List
        List<string> QRCodeData = Capture.FormatAsQRCodeData();
        QRCode? LabelCode = new QRCode(QRCodeData);
        // apply the header, the QR Code, and the Label Data to the Label
        await NewLabel.AddHeaderAsync(LabelHeader);
        await NewLabel.AddPartNameAsync(Capture.SelectedPart!.PartName);
        await NewLabel.AddQRCodeAsync(LabelCode);
        await NewLabel.AddLabelFieldsAsync(Capture.FormatAsLabelBodyText());
        // return the Label image
        return NewLabel.GetImage();
    }
}