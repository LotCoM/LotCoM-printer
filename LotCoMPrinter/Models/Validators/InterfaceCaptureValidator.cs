using System.Text.RegularExpressions;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Validators;

/// <summary>
/// Validates and captures the Data entered on the UI.
/// </summary>
public static class InterfaceCaptureValidator {

    private static Part? ValidatePartPicker(Picker PartPicker, string DataField) {
        // validate that the Picker has a selection
        if ((PartPicker.SelectedIndex == -1) || (PartPicker.SelectedItem == null)) {
            throw new FormatException($"Please select a {DataField} before printing Labels.");
        }
        // capture the value selected in the PickerControl
        return (Part?)PartPicker.ItemsSource[PartPicker.SelectedIndex];
    }

    private static string ValidateAsDigits(Entry EntryControl, string DataField) {
        // validate that the entry has a value and that it only contains digits
        string Value = EntryControl.Text;
        if (Value == "") {
            throw new FormatException($"Please enter a {DataField} before printing Labels.");
        }
        // remove whitespace and commas
        Value = Value.Replace(",", "").Replace(" ", "");
        if (int.TryParse(Value, out int _)) {
            return Value;
        }
        throw new FormatException($"Please enter a valid {DataField} before printing Labels.");
    }

    private static string ValidateJBKNumber(Entry EntryControl, string JBKType) {
        // validate that the entry has a value and that it only contains digits
        string Value;
        try {
            Value = ValidateAsDigits(EntryControl, JBKType);
        } catch (FormatException _ex) {
            throw new FormatException(_ex.Message);
        }
        // add leading zeroes to enforce formatting (3-length digit)
        while (Value.Length < 3) {
            Value = "0" + Value;
        }
        return Value;
    }

    private static string ValidateLotNumber(Entry LotNumberEntry) {
        // validate that the entry has a value and that it only contains alnum characters
        string Value = LotNumberEntry.Text;
        if (Value == "") {
            throw new FormatException("Please enter a Lot # before printing Labels.");
        }
        // set a regex pattern for 1+ alphanumerical characters
        string LotPattern = @"^[a-zA-Z0-9]+$";
        Regex LotRegex = new Regex(LotPattern);
        // ensure the value matches the regex requirement
        if (!LotRegex.IsMatch(Value)) {
            throw new FormatException("Please enter a valid Lot # before printing Labels.");
        }
        // add leading zeroes to create 9-length format and return the lot #
        while (Value.Length < 9) {
            Value = $"0{Value}";
        }
        return Value;
    }

    private static string ValidateOperatorID(Entry OperatorIDEntry) {
        // validate that the entry has a value and that it only contains characters
        string Value = OperatorIDEntry.Text;
        if (Value == "") {
            throw new FormatException("Please enter Operator Intials (ie. AB, ABC) before printing Labels.");
        }
        // set a regex pattern for 2 or 3 alphabetical characters
        string InitialPattern = @"^[a-zA-Z][a-zA-Z][a-zA-Z]?$";
        Regex InitialRegex = new Regex(InitialPattern);
        // ensure the value matches the regex requirement
        if (!InitialRegex.IsMatch(Value)) {
            throw new FormatException("Please enter Operator Intials (ie. AB, ABC) before printing Labels.");
        }
        // cast the string to Uppercase and return it
        return Value.ToUpper();
    }

    private static string ValidateModelEntry(Entry ModelNumberEntry) {
        // validate that the entry has a value and that it only contains alnum characters
        string Value = ModelNumberEntry.Text;
        if (Value == "") {
            throw new FormatException("Please enter a Model # before printing Labels.");
        }
        // set a regex pattern for 3 alphanumerical characters
        string ModelPattern = @"^[a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9]$";
        Regex ModelRegex = new Regex(ModelPattern);
        // ensure the value matches the regex requirement
        if (!ModelRegex.IsMatch(Value)) {
            throw new FormatException("Please enter a valid Model # before printing Labels.");
        }
        // cast the string to Uppercase and return it
        return Value.ToUpper();
    }

    private static string? ValidateProductionShiftPicker(Picker ProductionShiftPicker) {
        // validate that the Picker has a selection
        if ((ProductionShiftPicker.SelectedIndex == -1) || (ProductionShiftPicker.SelectedItem == null)) {
            throw new FormatException($"Please select a Production Shift before printing Labels.");
        }
        // capture the value selected in the PickerControl
        return (string?)ProductionShiftPicker.ItemsSource[ProductionShiftPicker.SelectedIndex];
    }
    
    /// <summary>
    /// Validates the UI Control states and entry values.
    /// </summary>
    /// <param name="SelectedProcess"></param>
    /// <param name="PartPicker"></param>
    /// <param name="QuantityEntry"></param>
    /// <param name="JBKNumberEntry"></param>
    /// <param name="LotNumberEntry"></param>
    /// <param name="DeburrJBKNumberEntry"></param>
    /// <param name="DieNumberEntry"></param>
    /// <param name="ModelNumberEntry"></param>
    /// <param name="ProductionDatePicker"></param>
    /// <param name="ProductionShiftPicker"></param>
    /// <param name="OperatorIDEntry"></param>
    /// <returns>A Dictionary of data captured from the UI Controls, keyed by their respective Control names.</returns>
    /// <exception cref="NullProcessException">Thrown if there is no selection in the ProcessPicker Control.</exception>
    /// <exception cref="ArgumentException">Thrown if the Process Requirements could not be retrieved.</exception>
    /// <exception cref="FormatException">Thrown if there is a validation failure.</exception>
    public static List<string> Validate(Process SelectedProcess, Picker PartPicker, Entry QuantityEntry, 
        Entry JBKNumberEntry, Entry LotNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry, 
        Entry ModelNumberEntry, DatePicker ProductionDatePicker, Picker ProductionShiftPicker, Entry OperatorIDEntry
    ) {
        // retrieve the Process requirements
        List<string> Requirements;
        try {
            Requirements = ProcessRequirements.GetProcessRequirements(SelectedProcess.FullName);
        // the Process selection was null
        } catch (NullProcessException) {
            throw new NullProcessException();
        // the selected Process was invalid (uncommon)
        } catch (ArgumentException) {
            throw new ArgumentException();
        }
        List<string> UIResults = [$"Process: {SelectedProcess.FullName}"];
        // create values for each of the UI entries
        Part? Part;
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
                Part = ValidatePartPicker(PartPicker, "Part");
                UIResults.Add($"Part: {Part.PartNumber}\n{Part.PartName}");
            };
            // validate quantity
            if (Requirements.Contains("QuantityEntry")) {
                Quantity = ValidateAsDigits(QuantityEntry, "Quantity");
                UIResults.Add($"Quantity: {Quantity!}");
            };
            // validate jbk number
            if (Requirements.Contains("JBKNumberEntry")) {
                // check if the JBK number is needed
                if (SelectedProcess.Type.Equals("Originator")) {
                    // JBK is assigned by serialization system and will be valid
                    JBKNumber = "000";
                } else {
                    // JBK was entered by operator; validate
                    JBKNumber = ValidateJBKNumber(JBKNumberEntry, "JBK Number");
                }
                // add the JBK Number to the UI Results
                UIResults.Add($"JBK #: {JBKNumber!}");
            };
            // validate lot number
            if (Requirements.Contains("LotNumberEntry")) {
                // check if the JBK number is needed
                if (SelectedProcess.Type.Equals("Originator")) {
                    // Lot is assigned by serialization system and will be valid
                    LotNumber = "000000000";
                } else {
                    // Lot was entered by operator; validate
                    LotNumber = ValidateLotNumber(LotNumberEntry);
                }
                // add the Lot Number to the UI Results
                UIResults.Add($"Lot #: {LotNumber!}");
            };
            // validate deburr jbk number
            if (Requirements.Contains("DeburrJBKNumberEntry")) {
                DeburrJBKNumber = ValidateJBKNumber(DeburrJBKNumberEntry, "Deburr JBK Number");
                UIResults.Add($"Deburr JBK #: {DeburrJBKNumber!}");
            };
            // validate die number
            if (Requirements.Contains("DieNumberEntry")) {
                DieNumber = ValidateAsDigits(DieNumberEntry, "Die Number");
                UIResults.Add($"Die #: {DieNumber!}");
            };
            // validate model number
            if (Requirements.Contains("ModelNumberEntry")) {
                ModelNumber = ValidateModelEntry(ModelNumberEntry);
                UIResults.Add($"Model #: {ModelNumber!}");
            };
            // add the production date; defaults to current day, no need to validate
            UIResults.Add($"Production Date: {ProductionDatePicker.Date.ToShortDateString()!}");
            // validate production shift
            if (Requirements.Contains("ProductionShiftPicker")) {
                ProductionShift = ValidateProductionShiftPicker(ProductionShiftPicker);
                UIResults.Add($"Production Shift: {ProductionShift!}");
            };
            // validate the operator initials
            if (Requirements.Contains("OperatorIDEntry")) {
                OperatorID = ValidateOperatorID(OperatorIDEntry);
                UIResults.Add($"Operator: {OperatorID!}");
            }
            // return the constructed UI Dictionary
            return UIResults;
        // a validation failed; pass the fail message to the view model
        } catch (FormatException _ex) {
            throw new FormatException(_ex.Message);
        }
    }
}