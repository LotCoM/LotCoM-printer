using System.Drawing;
using System.Reflection;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Validators;

namespace LotCoMPrinter.Models.Labels;

public static class LabelGenerator {
    /// <summary>
    /// Formats a DateTime object as a string like MM/DD/YYYY-HH:MM:SS.
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    private static string GetAsTimestamp(DateTime Date) {
        // retrieve and format each timestamp field
        string Month = $"{DateTime.Now.Month}";
        if (Month.Length < 2) {
            Month = "0" + Month;
        }
        string Day = $"{DateTime.Now.Day}";
        if (Day.Length < 2) {
            Day = "0" + Day;
        }
        string Year = $"{DateTime.Now.Year}";
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
        // return the timestamp as a string
        return $"{Month}/{Day}/{Year}-{Hour}:{Minute}:{Second}";
    }

    /// <summary>
    /// Compiles the required data for the Process into a List of strings.
    /// </summary>
    /// <param name="Capture"></param>
    /// <returns></returns>
    private static async Task<List<string>> GetQRCodeData(InterfaceCapture Capture) {
        List<string> QRCodeData = [];
        await Task.Run(() => {
            // retrieve the requirements for the SelectedProcess
            List<string> Requirements = ProcessRequirements.GetProcessRequirements(Capture.SelectedProcess!.FullName);
            string[] CodeExemptions = ["BasketType"];
            // add each required field, in order, to a QR code data list (only if it is not exempt)
            foreach (PropertyInfo _property in Capture.GetType().GetProperties()) {
                // save the property's name
                string _name = _property.Name;
                if (Requirements.Contains(_name) && !CodeExemptions.Contains(_name)) {
                    // retrieve the Process Name from the SelectedProcess
                    if (_name.Equals("SelectedProcess")) {
                        QRCodeData.Add(Capture.SelectedProcess!.FullName);
                    // retrieve the Part Number from SelectedPart
                    } else if (_name.Equals("SelectedPart")) {
                        QRCodeData.Add(Capture.SelectedPart!.PartNumber);
                        QRCodeData.Add(Capture.SelectedPart!.PartName);
                    // retrieve the mm/dd/yyyy-hh:mm:ss string from ProductionDate
                    } else if (_name.Equals("ProductionDate")) {
                        QRCodeData.Add(GetAsTimestamp(Capture.ProductionDate));
                    // just add the value of the property on the Capture object
                    } else {
                        QRCodeData.Add((string)_property.GetValue(Capture)!);
                    }
                }
            }
        });
        return QRCodeData;
    }

    /// <summary>
    /// Formats an InterfaceCapture's data into a Label Field text body (list of strings).
    /// </summary>
    /// <param name="Capture"></param>
    /// <returns></returns>
    private static async Task<List<string>> GetPrintedLabelFields(InterfaceCapture Capture) {
        List<string> LabelFields = [];
        await Task.Run(() => {
            // retrieve the requirements for the SelectedProcess
            List<string> Requirements = ProcessRequirements.GetProcessRequirements(Capture.SelectedProcess!.FullName);
            string[] PrintExemptions = ["SelectedProcess", "BasketType", "OperatorID"];
            // add each required field based on the need to print
            foreach (PropertyInfo _property in Capture.GetType().GetProperties()) {
                // save the property's name
                string _name = _property.Name;
                // only add the property to the list if it is required and not exempt from printing
                if (Requirements.Contains(_name) && !PrintExemptions.Contains(_name)) {
                    // retrieve the Part Number from SelectedPart
                    if (_name.Equals("SelectedPart")) {
                        LabelFields.Add(Capture.SelectedPart!.PartNumber);
                    // retrieve the mm/dd/yyyy-hh:mm:ss string from ProductionDate
                    } else if (_name.Equals("ProductionDate")) {
                        LabelFields.Add(GetAsTimestamp(Capture.ProductionDate));
                    // just add the value of the property on the Capture object
                    } else {
                        LabelFields.Add((string)_property.GetValue(Capture)!);
                    }
                }
            }
        });
        return LabelFields;
    }

    /// <summary>
    /// Generates a Full Label image object that mirrors the appearance of the physical Full Basket Label.
    /// Composition of Asynchronous tasks.
    /// </summary>
    /// <param name="Capture">An InterfaceCapture object.</param>
    /// <param name="LabelHeader">The Large text to include on the Label's top-left corner.</param>
    /// <returns></returns>
    /// <exception cref="LabelBuildException"></exception>
    public static async Task<Bitmap> GenerateFullLabelAsync(InterfaceCapture Capture, string LabelHeader) {
        Bitmap Label = await Task.Run(async () => {
            // create a new Label
            FullLabel? NewLabel;
            try {
                NewLabel = new FullLabel();
            // the Label failed to configure its fonts from the System
            } catch (SystemException _ex) {
                throw new LabelBuildException($"Failed to construct new Label due to the following exception:\n{_ex.Message}");
            }
            // create a List of Label Fields from the Capture and generate a QR Code from that List
            List<string> QRCodeData = await GetQRCodeData(Capture);
            QRCode? LabelCode = new QRCode(QRCodeData);
            // apply the header, the QR Code, and the Label Data to the Label
            await NewLabel.AddHeaderAsync(LabelHeader);
            await NewLabel.AddPartNameAsync(Capture.SelectedPart!.PartName);
            await NewLabel.AddQRCodeAsync(LabelCode);
            await NewLabel.AddLabelFieldsAsync(await GetPrintedLabelFields(Capture));
            // return the Label image
            return NewLabel.GetImage();
        });
        return Label;
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
        Bitmap Label = await Task.Run(async () => {
            // create a new Label
            PartialLabel? NewLabel;
            try {
                NewLabel = new PartialLabel();
            // the Label failed to configure its fonts from the System
            } catch (SystemException _ex) {
                throw new LabelBuildException($"Failed to construct new Label due to the following exception:\n{_ex.Message}");
            }
            // create a List of Label Fields from the Capture and generate a QR Code from that List
            List<string> QRCodeData = ["PARTIAL"];
            QRCodeData.AddRange(await GetQRCodeData(Capture));
            QRCode? LabelCode = new QRCode(QRCodeData);
            // apply the header, the QR Code, and the Label Data to the Label
            await NewLabel.AddHeaderAsync(LabelHeader);
            await NewLabel.AddPartNameAsync(Capture.SelectedPart!.PartName);
            await NewLabel.AddQRCodeAsync(LabelCode);
            await NewLabel.AddLabelFieldsAsync(await GetPrintedLabelFields(Capture));
            // return the Label image
            return NewLabel.GetImage();
        });
        return Label;
    }
}