using System.Drawing;
using LotCoMPrinter.Models.Serialization;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Labels;

namespace LotCoMPrinter.Models.Printing;

/// <summary>
/// Creates a Print Job that can generate a Bitmap image Label and spool a print job to the printing system.
/// </summary>
/// <param name="LabelInformation">The Data to be encoded in the Label's QR Code and shown on the Label itself (a validated UI Capture from InterfaceCaptureValidator).</param>
/// <param name="LabelType">Either 'Full' or 'Partial' (from the UI BasketTypePicker control).</param>
public class LabelPrintJob(List<string> LabelInformation, string LabelType, string Header) {
    // private class properties to hold Label data and generated Label Bitmap
    private List<string> _labelInformation = LabelInformation;
    // split out the JBK # value to apply as the header
    private string _header = Header;
    private Bitmap? _label = null;
    private string _labelType = LabelType;

    /// <summary>
    /// Creates a Label object and Bitmap image from the Job's saved information. Stores the generated Bitmap image in _label property.
    /// </summary>
    /// <returns></returns>
    private async Task GenerateLabelImage() {
        // elicit the header (JBK # if serialization is JBK, MM/DD of date if serialization is Lot)
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
    public async Task<bool> Run() {
        // generate a Label from the saved Label information
        await GenerateLabelImage();
        // create a PrintHandler object for the new Label
        bool Printed = false;
        if (_label != null) {
            PrintHandler LabelPrinter = new PrintHandler(_label);
            // try to print the Label
            try {
                await LabelPrinter.PrintLabelAsync();
                Printed = true;
            // handle errors thrown by the PrintLabelAsync() method
            } catch (Exception _ex){
                App.AlertSvc!.ShowAlert(
                    "Failed to Print", "There was an error Printing the Label. Please see management to resolve this issue."
                    + $"\n\nException Message(s): {_ex.InnerException}"
                );
            }
            // prepare the serial number and the serial cache for the end of the print job
            string SerialNumber = _labelInformation[3].Replace("JBK: ", "").Replace("Lot: ", "");
            string PartNumber = _labelInformation[1].Split("\n")[0].Replace("Part: ", "");
            SerialCacheController SerialCache = new SerialCacheController();
            // the print job was successful
            if (Printed) {
                // remove the cached serial number here if the label was full
                if (_labelType == "Full") {
                    await SerialCache.RemoveCachedSerialNumber(SerialNumber, PartNumber);
                }
                // log the successful print job
                await PrintLogger.LogPrintEvent(_labelInformation);
            // the print failed; cache the serial number
            } else {
                await SerialCache.CacheSerialNumber(SerialNumber, PartNumber);
            }
        }
        return Printed;
    }
}