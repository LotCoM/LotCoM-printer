using System.Drawing;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Labels;

public static class LabelGenerator {
    
    /// <summary>
    /// Generates a Full Label image object that mirrors the appearance of the physical Full Basket Label.
    /// Composition of Asynchronous tasks.
    /// </summary>
    /// <param name="LabelHeader">The Large text to include on the Label's top-left corner.</param>
    /// <param name="LabelData">The Data to include in the Label's QR Code and on the Label in plain-text.</param>
    /// <returns></returns>
    /// <exception cref="LabelBuildException"></exception>
    public static async Task<Bitmap> GenerateFullLabelAsync(string LabelHeader, List<string> LabelData) {    
        // create a new Label
        Label? NewLabel;
        try {
            NewLabel = new Label();
        // the Label failed to configure its fonts from the System
        } catch (SystemException _ex) {
            throw new LabelBuildException($"Failed to construct new Label due to the following exception:\n{_ex.Message}");
        }
        // add the generation timestamp to the Production Date field
        string Hour = $"{DateTime.Now.Hour}";
        if (Hour.Length < 2) {
            Hour = "0" + Hour;
        }
        string Minute = $"{DateTime.Now.Minute}";
        if (Minute.Length < 2) {
            Minute = "0" + Minute;
        }
        string Second = $"{DateTime.Now.Second}";
        if (Second.Length < 2) {
            Second = "0" + Second;
        }
        LabelData[LabelData.Count - 3] = LabelData[LabelData.Count - 3] + $"-{Hour}:{Minute}:{Second}";
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
        await NewLabel.AddPartNameAsync(LabelData[1].Split("\n")[1]);
        await NewLabel.AddQRCodeAsync(LabelCode);
        await NewLabel.AddLabelFieldsAsync(LabelData);
        // return the Label image
        return NewLabel.GetImage();
    }


    /// <summary>
    /// Generates a Partial Label image object that mirrors the appearance of the physical Partial Basket Label.
    /// Composition of Asynchronous tasks.
    /// </summary>
    /// <param name="LabelHeader">The Large text to include on the Label's top-left corner.</param>
    /// <param name="LabelData">The Data to include in the Label's QR Code and on the Label in plain-text.</param>
    /// <returns></returns>
    /// <exception cref="LabelBuildException"></exception>
    public static async Task<Bitmap> GeneratePartialLabelAsync(string LabelHeader, List<string> LabelData) {    
        // create a new Label
        PartialLabel? NewLabel;
        try {
            NewLabel = new PartialLabel();
        // the Label failed to configure its fonts from the System
        } catch (SystemException _ex) {
            throw new LabelBuildException($"Failed to construct new Label due to the following exception:\n{_ex.Message}");
        }
        // add the generation timestamp to the Production Date field
        string Hour = $"{DateTime.Now.Hour}";
        if (Hour.Length < 2) {
            Hour = "0" + Hour;
        }
        string Minute = $"{DateTime.Now.Minute}";
        if (Minute.Length < 2) {
            Minute = "0" + Minute;
        }
        string Second = $"{DateTime.Now.Second}";
        if (Second.Length < 2) {
            Second = "0" + Second;
        }
        LabelData[LabelData.Count - 3] = LabelData[LabelData.Count - 3] + $"-{Hour}:{Minute}:{Second}";
        // copy only the values of the data fields to the QR Code data
        List<string> QRCodeData = ["PARTIAL"];
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
        await NewLabel.AddPartNameAsync(LabelData[1].Split("\n")[1]);
        await NewLabel.AddQRCodeAsync(LabelCode);
        await NewLabel.AddLabelFieldsAsync(LabelData);
        // return the Label image
        return NewLabel.GetImage();
    }
}