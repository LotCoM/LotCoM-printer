using System.Text.RegularExpressions;
using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Validators;

/// <summary>
/// Validates values contained in InterfaceCapture objects, based on their SelectedProcess value.
/// </summary>
public static class InterfaceCaptureValidator {
    /// <summary>
    /// Validates a string as non-null, non-empty, and removes all non-digit characters.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns>The string after removing all non-digits (may be empty).</returns>
    /// <exception cref="NullReferenceException"></exception>
    private static string ValidateDigitBase(string? Value) {
        // validate that the value is not null
        if (Value == null) {
            throw new NullReferenceException();
        }
        // remove any non-digit characters from the value
        return Regex.Replace(Value, @"^[^\d]+$", "");
    }

    /// <summary>
    /// Validates a Part object as non-null.
    /// </summary>
    /// <param name="SelectedPart"></param>
    /// <exception cref="FormatException"></exception>
    private static void ValidatePart(Part? SelectedPart) {
        // validate that the Part is not null
        if (SelectedPart == null) {
            throw new FormatException($"Please select a Part before printing Labels.");
        }
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Replaces all non-digit characters.
    /// </summary>
    /// <param name="Quantity"></param>
    /// <returns>The string as only digits.</returns>
    /// <exception cref="FormatException"></exception>
    private static string ValidateQuantity(string? Quantity) {
        // validate and format the Quantity string
        try {
            Quantity = ValidateDigitBase(Quantity);
        // the quantity was null
        } catch (NullReferenceException) {
            throw new FormatException($"Please enter a Quantity before printing Labels.");
        }
        // ensure that the processed Quantity contains at least one digit
        if (Quantity.Length < 0) {
            throw new FormatException($"Please enter a valid Quantity before printing Labels.");
        }
        return Quantity;
    }

    /// <summary>
    /// Validates a DateTime object as non-null.
    /// </summary>
    /// <param name="Date"></param>
    /// <exception cref="FormatException"></exception>
    private static void ValidateProductionDate(DateTime? Date) {
        // validate that Date is not null
        if (Date == null) {
            throw new FormatException($"Please select a Production Date before printing Labels.");
        }
    }

    /// <summary>
    /// Validates a string as a non-null Shift Number.
    /// </summary>
    /// <param name="ProductionShift"></param>
    /// <exception cref="FormatException"></exception>
    private static void ValidateProductionShiftPicker(string? ProductionShift) {
        // validate that the string is non-null
        if (ProductionShift == null) {
            throw new FormatException($"Please select a Production Shift before printing Labels.");
        }
        // validate that the shift is 1, 2, or 3
        string[] Shifts = ["1", "2", "3"];
        if (!Shifts.Contains(ProductionShift)) {
            throw new FormatException($"Please select a valid Production Shift before printing Labels.");
        }
    }

    /// <summary>
    /// Validates a string as non-null. Enforces two or three length, uppercase character format.
    /// </summary>
    /// <param name="OperatorID"></param>
    /// <returns>The string as an uppercase Operator Initial.</returns>
    /// <exception cref="FormatException"></exception>
    private static string ValidateOperatorID(string? OperatorID) {
        // validate that the string is non-null
        if (OperatorID == null) {
            throw new FormatException("Please enter Operator Intials (ie. AB, ABC) before printing Labels.");
        }
        // set a regex pattern for 2 or 3 alphabetical characters
        string InitialPattern = @"^[a-zA-Z][a-zA-Z][a-zA-Z]?$";
        Regex InitialRegex = new Regex(InitialPattern);
        // ensure the value matches the regex requirement
        if (!InitialRegex.IsMatch(OperatorID)) {
            throw new FormatException("Please enter valid Operator Intials (ie. AB, ABC) before printing Labels.");
        }
        // cast the string to Uppercase and return it
        return OperatorID.ToUpper();
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures format as a JBK Number.
    /// </summary>
    /// <param name="JBKNumber"></param>
    /// <returns>The string formatted as a JBK Number.</returns>
    /// <exception cref="FormatException"></exception>
    private static string ValidateJBKNumber(string? JBKNumber, string ProcessType) {
        // check if the JBK is assigned at this process or copied (pass-through)
        if (ProcessType.Equals("Originator")) {
            // JBK is assigned by serialization system and will be valid
            JBKNumber = "000";
        }
        // validate and format the JBK Number string
        try {
            JBKNumber = ValidateDigitBase(JBKNumber);
        // the JBK Number was null
        } catch (NullReferenceException) {
            throw new FormatException($"Please enter a JBK Number before printing Labels.");
        }
        // ensure that the processed JBK Number contains at least one digit
        if (JBKNumber.Length < 0) {
            throw new FormatException($"Please enter a valid JBK Number before printing Labels.");
        // add leading zeroes to enforce three-length format
        } else {
            while (JBKNumber.Length < 3) {
                JBKNumber = $"0{JBKNumber}";
            }
        }
        return JBKNumber;
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures format as a Lot Number.
    /// </summary>
    /// <param name="LotNumber"></param>
    /// <returns>The string formatted as a Lot Number.</returns>
    /// <exception cref="FormatException"></exception>
    private static string ValidateLotNumber(string? LotNumber, string ProcessType) {
        // check if the Lot is assigned at this process or copied (pass-through)
        if (ProcessType.Equals("Originator")) {
            // Lot is assigned by serialization system and will be valid
            LotNumber = "000000000";
        }
        // validate and format the Lot Number string
        try {
            LotNumber = ValidateDigitBase(LotNumber);
        // the Lot Number was null
        } catch (NullReferenceException) {
            throw new FormatException($"Please enter a Lot Number before printing Labels.");
        }
        // ensure that the processed Lot Number contains at least one digit
        if (LotNumber.Length < 0) {
            throw new FormatException($"Please enter a valid Lot Number before printing Labels.");
        // add leading zeroes to enforce nine-length format
        } else {
            while (LotNumber.Length < 9) {
                LotNumber = $"0{LotNumber}";
            }
        }
        return LotNumber;
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures at least one digit format.
    /// </summary>
    /// <param name="DieNumber"></param>
    /// <returns>The string as only digits.</returns>
    /// <exception cref="FormatException"></exception>
    private static string ValidateDieNumber(string? DieNumber) {
        // validate and format the Die Number string
        try {
            DieNumber = ValidateDigitBase(DieNumber);
        // the Die Number was null
        } catch (NullReferenceException) {
            throw new FormatException($"Please enter a Die Number before printing Labels.");
        }
        // ensure that the processed Die Number contains at least one digit
        if (DieNumber.Length < 0) {
            throw new FormatException($"Please enter a valid Die Number before printing Labels.");
        }
        return DieNumber;
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures at least one digit format.
    /// </summary>
    /// <param name="HeatNumber"></param>
    /// <returns>The string as only digits.</returns>
    /// <exception cref="FormatException"></exception>
    private static string ValidateHeatNumber(string? HeatNumber) {
        // validate and format the Heat Number string
        try {
            HeatNumber = ValidateDigitBase(HeatNumber);
        // the Heat Number was null
        } catch (NullReferenceException) {
            throw new FormatException($"Please enter a Heat Number before printing Labels.");
        }
        // ensure that the processed Heat Number contains at least one digit
        if (HeatNumber.Length < 0) {
            throw new FormatException($"Please enter a valid Heat Number before printing Labels.");
        }
        return HeatNumber;
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures 3-character, uppercase alphanumeric Model Number format.
    /// </summary>
    /// <param name="ModelNumber"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    private static string ValidateModelNumber(string? ModelNumber) {
        // validate that the string is non-null, non-empty, and that it only contains alnum characters
        if (ModelNumber == "" || ModelNumber == null) {
            throw new FormatException("Please enter a Model Number before printing Labels.");
        }
        // set a regex pattern for 3 alphanumerical characters
        string ModelPattern = @"^[a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9]$";
        Regex ModelRegex = new Regex(ModelPattern);
        // ensure the value matches the regex requirement
        if (!ModelRegex.IsMatch(ModelNumber)) {
            throw new FormatException("Please enter a valid Model Number before printing Labels.");
        }
        // cast the string to Uppercase and return it
        return ModelNumber.ToUpper();
    }
    
    /// <summary>
    /// Validates an InterfaceCapture object. 
    /// Ensures that all required controls have valid input by comparing the requirements for the 
    /// InterfaceCapture's SelectedProcess property to the object's control property values.
    /// </summary>
    /// <param name="Capture">The InterfaceCapture object to validate.</param>
    /// <exception cref="NullProcessException">Thrown if there is no selection in the ProcessPicker Control.</exception>
    /// <exception cref="ArgumentException">Thrown if the Process Requirements could not be retrieved.</exception>
    /// <exception cref="FormatException">Thrown if there is a validation failure.</exception>
    public static InterfaceCapture Validate(InterfaceCapture Capture) {
        // retrieve the Process requirements for the Process in the Capture
        List<string> Requirements = Capture.SelectedProcess.RequiredFields;
        // attempt validations for every required field
        try {
            // validate universally required fields
            ValidatePart(Capture.SelectedPart);
            Capture.Quantity = ValidateQuantity(Capture.Quantity);
            ValidateProductionDate(Capture.ProductionDate);
            ValidateProductionShiftPicker(Capture.ProductionShift);
            Capture.OperatorID = ValidateOperatorID(Capture.OperatorID);
            // validate jbk number if required (also adds any needed leading zeroes)
            if (Requirements.Contains("JBKNumber")) {
                Capture.JBKNumber = ValidateJBKNumber(Capture.JBKNumber, Capture.SelectedProcess.Type);
            };
            // validate lot number if required (also adds any needed leading zeroes)
            if (Requirements.Contains("LotNumber")) {
                Capture.LotNumber = ValidateLotNumber(Capture.LotNumber, Capture.SelectedProcess.Type);
            };
            // validate deburr jbk number if required (also adds any needed leading zeroes)
            if (Requirements.Contains("DeburrJBKNumber")) {
                // pass ProcessType as "Pass-through" to force non-serialized check
                Capture.DeburrJBKNumber = ValidateJBKNumber(Capture.DeburrJBKNumber, "Pass-through");
            };
            // validate die number if required
            if (Requirements.Contains("DieNumber")) {
                ValidateDieNumber(Capture.DieNumber);
            };
            // validate heat number if required
            if (Requirements.Contains("HeatNumber")) {
                ValidateHeatNumber(Capture.HeatNumber);
            };
            // validate model number if required
            if (Requirements.Contains("ModelNumber")) {
                ValidateModelNumber(Capture.ModelNumber);
            };
        // a validation failed; pass the fail message to the view model
        } catch (FormatException _ex) {
            throw new FormatException(_ex.Message);
        }
        // the validation measures succeeded; return the modified capture object
        return Capture;
    }
}