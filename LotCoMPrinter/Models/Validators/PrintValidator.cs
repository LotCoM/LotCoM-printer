using LotCoMPrinter.Models.Datasources;

namespace LotCoMPrinter.Models.Validators;

/// <summary>
/// Validates and captures the Data entered on the UI.
/// </summary>
public static class PrintValidator {

    private static string? ValidatePicker(Picker PickerControl, string DataField) {
        // validate that the Picker has a selection
        if ((PickerControl.SelectedIndex == -1) || (PickerControl.SelectedItem == null)) {
            // show a warning
            App.AlertSvc!.ShowAlert("Invalid Production Data", $"Please select a {DataField} before printing Labels.");
            throw new FormatException();
        } else {
            // capture the value selected in the PickerControl
            return (string?)PickerControl.ItemsSource[PickerControl.SelectedIndex];
        }
    }

    private static string ValidateLotNumberEntry(Entry LotNumberEntry) {
        // validate that the entry has a value and that it only contains digits
        string Value = LotNumberEntry.Text;
        if (Value == "") {
            // show a warning
            App.AlertSvc!.ShowAlert("Invalid Production Data", "Please enter a Lot Number before printing Labels.");
            throw new FormatException();
        } else {
            // remove whitespace and commas
            Value = Value.Replace(",", "").Replace(" ", "");
            return Value;
        }
    }

    private static string ValidateAsDigits(Entry EntryControl, string DataField) {
        // validate that the entry has a value and that it only contains digits
        string Value = EntryControl.Text;
        if (Value == "") {
            // show a warning
            App.AlertSvc!.ShowAlert("Invalid Production Data", $"Please enter a {DataField} before printing Labels.");
            throw new FormatException();
        } else {
            // remove whitespace and commas
            Value = Value.Replace(",", "").Replace(" ", "");
            if (int.TryParse(Value, out int _)) {
                return Value;
            } else {
                // show a warning
                App.AlertSvc!.ShowAlert("Invalid Production Data", $"Please enter a valid {DataField} before printing Labels.");
                throw new FormatException();
            }
        }
    }

    /// <summary>
    /// Validates the PartValidator's UI Control states and Entries.
    /// </summary>
    /// <param name="Process"></param>
    /// <param name="PartPicker"></param>
    /// <param name="QuantityEntry"></param>
    /// <param name="JBKNumberEntry"></param>
    /// <param name="LotNumberEntry"></param>
    /// <param name="DeburrJBKNumberEntry"></param>
    /// <param name="DieNumberEntry"></param>
    /// <param name="ModelNumberPicker"></param>
    /// <param name="ProductionDatePicker"></param>
    /// <param name="ProductionShiftPicker"></param>
    /// <returns>A Dictionary of data captured from the UI Controls, keyed by their respective Control names.</returns>
    /// <exception cref="FormatException">Raises if any of the required checks are failed.</exception>
    public static Dictionary<string, string> Validate(string Process, Picker PartPicker, Entry QuantityEntry, 
        Entry JBKNumberEntry, Entry LotNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry,
        Picker ModelNumberPicker, DatePicker ProductionDatePicker, Picker ProductionShiftPicker
    ) {
        // retrieve the Process requirements
        List<string> ProcessRequirements = ProcessData.GetProcessRequirements(Process);
        Dictionary<string, string> UIResults = new Dictionary<string, string> {};
        // create values for each of the UI entries
        string? Part;
        string? Quantity;
        string? JBKNumber;
        string? LotNumber;
        string? DeburrJBKNumber;
        string? DieNumber;
        string? ModelNumber;
        string? ProductionShift;
        // attempt all the validations
        try {
            // validate part
            if (ProcessRequirements.Contains("PartPicker")) {
                Part = ValidatePicker(PartPicker, "Part");
                UIResults.Add("Part", Part!);
            };
            // validate quantity
            if (ProcessRequirements.Contains("QuantityEntry")) {
                Quantity = ValidateAsDigits(QuantityEntry, "Quantity");
                UIResults.Add("Quantity", Quantity!);
            };
            // validate jbk number
            if (ProcessRequirements.Contains("JBKNumberEntry")) {
                JBKNumber = ValidateAsDigits(JBKNumberEntry, "JBK Number");
                UIResults.Add("JBKNumber", JBKNumber!);
            };
            // validate lot number
            if (ProcessRequirements.Contains("LotNumberEntry")) {
                LotNumber = ValidateLotNumberEntry(LotNumberEntry);
                UIResults.Add("LotNumber", LotNumber!);
            };
            // validate deburr jbk number
            if (ProcessRequirements.Contains("DeburrJBKNumberEntry")) {
                DeburrJBKNumber = ValidateAsDigits(DeburrJBKNumberEntry, "Deburr JBK Number");
                UIResults.Add("DeburrJBKNumber", DeburrJBKNumber!);
            };
            // validate die number
            if (ProcessRequirements.Contains("DieNumberEntry")) {
                DieNumber = ValidateAsDigits(DieNumberEntry, "Die Number");
                UIResults.Add("DieNumber", DieNumber!);
            };
            // validate model number
            if (ProcessRequirements.Contains("ModelNumberPicker")) {
                ModelNumber = ValidatePicker(ModelNumberPicker, "Model Number");
                UIResults.Add("ModelNumber", ModelNumber!);
            };
            // add the production date; defaults to current day, no need to validate
            UIResults.Add("ProductionDate", ProductionDatePicker.Date.ToLongDateString()!);
            // validate production shift
            if (ProcessRequirements.Contains("ProductionShiftPicker")) {
                ProductionShift = ValidatePicker(ProductionShiftPicker, "Production Shift");
                UIResults.Add("ProductionShift", ProductionShift!);
            };
            // return the constructed UI Dictionary
            return UIResults;
        } catch {
            throw new FormatException();
        }
    }
}