using System.Drawing;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Labels;

namespace LotCoMPrinter.Models.Printing;

/// <summary>
/// Creates a Print Job that can generate a Bitmap image Label and spool a print job to the printing system.
/// </summary>
/// <param name="LabelInformation">The Data to be encoded in the Label's QR Code and shown on the Label itself (a validated UI Capture from InterfaceCaptureValidator).</param>
public class LabelPrintJob(List<string> LabelInformation, string SerializeMode, string ModelNumber, string LabelType) {
    // private class properties to hold Label data and generated Label Bitmap
    private List<string> _labelInformation = LabelInformation;
    // split out the JBK # value to apply as the header
    private string _header = LabelInformation[3].Split(":")[1].Replace(" ", "");
    private Bitmap? _label = null;
    private string _serializeMode = SerializeMode;
    private string _modelNumber = ModelNumber;
    private string _labelType = LabelType;

    /// <summary>
    /// Creates a Label object and Bitmap image from the Job's saved information. Stores the generated Bitmap image in _label property.
    /// </summary>
    /// <returns></returns>
    private async Task GenerateLabelImage() {
        try {
            // full label
            if (_labelType == "Full") {
                _label = await LabelGenerator.GenerateFullLabelAsync(_header, _labelInformation);
            // partial label
            } else {
                _label = await LabelGenerator.GeneratePartialLabelAsync(_header, _labelInformation);
            }
        // there was an unexpected error in the Label generation
        } catch (LabelBuildException _ex) {
            App.AlertSvc!.ShowAlert(
                "Failed to Print", "There was an error Printing the Label. Please see management to resolve this issue."
                + $"\n\nException Message(s): {_ex.Message}"
            );
        }
    }

    /// <summary>
    /// Runs the Print Job (creates a Handler for the Job and spools it to the system's printing system).
    /// </summary>
    /// <returns></returns>
    public async Task Run() {
        // generate a Label from the saved Label information
        await GenerateLabelImage();
        // create a PrintHandler object for the new Label
        if (_label != null) {
            PrintHandler LabelPrinter = new PrintHandler(_label);
            // try to print the Label
            bool Printed;
            try {
                await LabelPrinter.PrintLabelAsync();
                Printed = true;
            // handle errors thrown by the PrintLabelAsync() method
            } catch (Exception _ex){
                App.AlertSvc!.ShowAlert(
                    "Failed to Print", "There was an error Printing the Label. Please see management to resolve this issue."
                    + $"\n\nException Message(s): {_ex.InnerException}"
                );
                Printed = false;
            }
            // consume the queued serializing number (only if the print was successful)
            if (Printed) {
                string SerialNumber;
                if (_serializeMode == "JBK") {
                    SerialNumber = await JBKQueue.ConsumeAsync(_modelNumber);
                } else {
                    SerialNumber = await LotQueue.ConsumeAsync(_modelNumber);
                }
                // if partial, store the queued number for use in the full label
                if (_labelType == "Partial") {
                    // do caching actions
                } 
            }
        }
    }
}