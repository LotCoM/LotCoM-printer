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
    /// Processes the use of a Serial Number for a Partial Label. 
    /// Checks if there is a Serial Number cached for the Part Number.
    /// If not, consumes and caches the queued Serial Number.
    /// </summary>
    /// <returns></returns>
    private async Task<string?> ProcessSerialNumber() {
        string? SerialNumber = null;
        // run a new CPU thread to get the serial number for this label
        await Task.Run(async () => {
            // save the part number for later use
            string PartNumber = _labelInformation[1].Split("\n")[0];
            // check if there is already a cached number
            SerialNumber = await SerialCache.FindNumberForPart(PartNumber);
            // if there was no serial number found, consume and cache a new one
            if (SerialNumber == null) {
                if (_serializeMode == "JBK") {
                    // consume the queued JBK number for this Model
                    SerialNumber = await JBKQueue.ConsumeAsync(_modelNumber);
                } else {
                    // consume the queued Lot number for this Model and cache it under the part number
                    SerialNumber = await LotQueue.ConsumeAsync(_modelNumber);
                }
                // if the label is a Partial, cache the Serial Number under the part number
                if (_labelType == "Partial") {
                    await SerialCache.CacheSerialNumber(SerialNumber, PartNumber);
                }
            }
        });
        // return the serial number
        return SerialNumber;
    }

    /// <summary>
    /// Creates a Label object and Bitmap image from the Job's saved information. Stores the generated Bitmap image in _label property.
    /// </summary>
    /// <returns></returns>
    private async Task GenerateLabelImage() {
        try {
            // get the Label's Serial Number
            string? SerialNumber = await ProcessSerialNumber();
            if (SerialNumber == null) {
                throw new LabelBuildException("Failed to assign a Serial Number to the Label");
            }
            // assign the Serial Number to the JBK/Lot # field in _labelInformation
            _labelInformation[3] = SerialNumber;
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
                await ProcessSerialNumber();
                // remove the cached serial number here if the label was partial
                if (_labelType == "Partial") {
                    await SerialCache.RemoveCachedSerialNumber(_labelInformation[3], _labelInformation[1].Split("\n")[0]);
                }
            }
        }
    }
}