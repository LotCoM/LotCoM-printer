using System.Text.RegularExpressions;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Validators;

/// <summary>
/// Validates and captures the Data entered on the UI.
/// </summary>
public static class InterfaceCaptureValidator {

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
        // validate that the entry has a value
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

    private static string ValidateJBKNumber(Entry EntryControl, string JBKType) {
        // validate that the entry has a value and that it only contains digits
        string Value;
        try {
            Value = ValidateAsDigits(EntryControl, JBKType);
        } catch (FormatException) {
            throw new FormatException();
        }
        // add leading zeroes to enforce formatting (3-length digit)
        while (Value.Length < 3) {
            Value = "0" + Value;
        }
        return Value;
    }

    private static string ValidateOperatorID(Entry OperatorIDEntry) {
        // validate that the entry has a value and that it only contains characters
        string Value = OperatorIDEntry.Text;
        if (Value == "") {
            // show a warning
            App.AlertSvc!.ShowAlert("Invalid Operator Initials", "Please enter Operator Intials (ie. AB, ABC) before printing Labels.");
            throw new FormatException();
        } else {
            // set a regex pattern for 2 or 3 alphabetical characters
            string InitialPattern = @"^[a-zA-Z][a-zA-Z][a-zA-Z]?$";
            Regex InitialRegex = new Regex(InitialPattern);
            // ensure the value matches the regex requirement
            if (!InitialRegex.IsMatch(Value)) {
                App.AlertSvc!.ShowAlert("Invalid Operator Initials", "Please enter Operator Intials (ie. AB, ABC) before printing Labels.");
                throw new FormatException();
            }
            // cast the string to Uppercase and return it
            return Value.ToUpper();
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
    public static List<string> Validate(string Process, Picker PartPicker, Entry QuantityEntry, 
        Entry JBKNumberEntry, Entry LotNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry,
        Picker ModelNumberPicker, DatePicker ProductionDatePicker, Picker ProductionShiftPicker, Entry OperatorIDEntry
    ) {
        // retrieve the Process requirements
        List<string> Requirements = [];
        try {
            Requirements = ProcessRequirements.GetProcessRequirements(Process);
        // the Process selection was null
        } catch (NullProcessException) {
            // show a warning
            App.AlertSvc!.ShowAlert("Failed to Print", "Please select a Process before printing Labels.");
        // the selected Process was invalid (uncommon)
        } catch (ArgumentException) {
            // show a warning
            App.AlertSvc!.ShowAlert("Failed to Print", "The selected Process' requirements could not be retrieved. Please see management to resolve this issue.");
        }
        List<string> UIResults = [];
        // create values for each of the UI entries
        string? Part;
        string? Quantity;
        string? JBKNumber;
        string? LotNumber;
        string? DeburrJBKNumber;
        string? DieNumber;
        string? ModelNumber;
        string? ProductionShift;
        string? OperatorID;
        // attempt all the validations
        try {
            // validate part
            if (Requirements.Contains("PartPicker")) {
                Part = ValidatePicker(PartPicker, "Part");
                UIResults.Add($"Part: {Part!}");
            };
            // validate quantity
            if (Requirements.Contains("QuantityEntry")) {
                Quantity = ValidateAsDigits(QuantityEntry, "Quantity");
                UIResults.Add($"Quantity: {Quantity!}");
            };
            // validate jbk number
            if (Requirements.Contains("JBKNumberEntry")) {
                JBKNumber = ValidateJBKNumber(JBKNumberEntry, "JBK Number");
                UIResults.Add($"JBK #: {JBKNumber!}");
            };
            // validate lot number
            if (Requirements.Contains("LotNumberEntry")) {
                LotNumber = ValidateLotNumberEntry(LotNumberEntry);
                UIResults.Add($"Lot #: {LotNumber!}");
            };
            // validate deburr jbk number
            if (Requirements.Contains("DeburrJBKNumberEntry")) {
                DeburrJBKNumber = ValidateJBKNumber(DeburrJBKNumberEntry, "Deburr JBK Number");
                UIResults.Add($"Deburr JBK # {DeburrJBKNumber!}");
            };
            // validate die number
            if (Requirements.Contains("DieNumberEntry")) {
                DieNumber = ValidateAsDigits(DieNumberEntry, "Die Number");
                UIResults.Add($"Die #: {DieNumber!}");
            };
            // validate model number
            if (Requirements.Contains("ModelNumberPicker")) {
                ModelNumber = ValidatePicker(ModelNumberPicker, "Model Number");
                UIResults.Add($"Model #: {ModelNumber!}");
            };
            // add the production date; defaults to current day, no need to validate
            UIResults.Add($"Production Date: {ProductionDatePicker.Date.ToShortDateString()!}");
            // validate production shift
            if (Requirements.Contains("ProductionShiftPicker")) {
                ProductionShift = ValidatePicker(ProductionShiftPicker, "Production Shift");
                UIResults.Add($"Production Shift: {ProductionShift!}");
            };
            // validate the operator initials
            if (Requirements.Contains("OperatorIDEntry")) {
                OperatorID = ValidateOperatorID(OperatorIDEntry);
                UIResults.Add($"Operator: {OperatorID!}");
            }
            // return the constructed UI Dictionary
            return UIResults;
        } catch {
            throw new FormatException();
        }
    }
}