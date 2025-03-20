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
    private static async Task<string> ValidateDigitBase(string? Value) {
        string Digits = await Task.Run(() => {
            // validate that the value is not null
            if (Value == null) {
                throw new NullReferenceException();
            }
            // remove any non-digit characters from the value
            return Regex.Replace(Value, @"^[^\d]+$", "");
        });
        return Digits;
    }

    /// <summary>
    /// Validates a Part object as non-null.
    /// </summary>
    /// <param name="SelectedPart"></param>
    /// <exception cref="FormatException"></exception>
    private static async Task ValidatePart(Part? SelectedPart) {
        await Task.Run(() => {
            // validate that the Part is not null
            if (SelectedPart == null) {
                throw new FormatException($"Please select a Part before printing Labels.");
            }
        });
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Replaces all non-digit characters.
    /// </summary>
    /// <param name="Quantity"></param>
    /// <returns>The string as only digits.</returns>
    /// <exception cref="FormatException"></exception>
    private static async Task<string> ValidateQuantity(string? Quantity) {
        Quantity = await Task.Run(async () => {
            // validate and format the Quantity string
            try {
                Quantity = await ValidateDigitBase(Quantity);
            // the quantity was null
            } catch (NullReferenceException) {
                throw new FormatException($"Please enter a Quantity before printing Labels.");
            }
            // ensure that the processed Quantity contains at least one digit
            if (Quantity.Length < 0) {
                throw new FormatException($"Please enter a valid Quantity before printing Labels.");
            }
            return Quantity;
        });
        return Quantity;
    }

    /// <summary>
    /// Validates a DateTime object as non-null.
    /// </summary>
    /// <param name="Date"></param>
    /// <exception cref="FormatException"></exception>
    private static async Task ValidateProductionDate(DateTime? Date) {
        await Task.Run(() => {
            // validate that Date is not null
            if (Date == null) {
                throw new FormatException($"Please select a Production Date before printing Labels.");
            }
        });
    }

    /// <summary>
    /// Validates a string as a non-null Shift Number.
    /// </summary>
    /// <param name="ProductionShift"></param>
    /// <exception cref="FormatException"></exception>
    private static async Task ValidateProductionShiftPicker(string? ProductionShift) {
        await Task.Run(() => {
            // validate that the string is non-null
            if (ProductionShift == null) {
                throw new FormatException($"Please select a Production Shift before printing Labels.");
            }
            // validate that the shift is 1, 2, or 3
            string[] Shifts = ["1", "2", "3"];
            if (!Shifts.Contains(ProductionShift)) {
                throw new FormatException($"Please select a valid Production Shift before printing Labels.");
            }
        });
    }

    /// <summary>
    /// Validates a string as non-null. Enforces two or three length, uppercase character format.
    /// </summary>
    /// <param name="OperatorID"></param>
    /// <returns>The string as an uppercase Operator Initial.</returns>
    /// <exception cref="FormatException"></exception>
    private static async Task<string> ValidateOperatorID(string? OperatorID) {
        OperatorID = await Task.Run(() => {
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
        });
        return OperatorID;
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures format as a JBK Number.
    /// </summary>
    /// <param name="JBKNumber"></param>
    /// <returns>The string formatted as a JBK Number.</returns>
    /// <exception cref="FormatException"></exception>
    private static async Task<string> ValidateJBKNumber(string? JBKNumber, string ProcessType) {
        JBKNumber = await Task.Run(async () => {
            // check if the JBK is assigned at this process or copied (pass-through)
            if (ProcessType.Equals("Originator")) {
                // JBK is assigned by serialization system and will be valid
                JBKNumber = "000";
            }
            // validate and format the JBK Number string
            try {
                JBKNumber = await ValidateDigitBase(JBKNumber);
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
        });
        return JBKNumber;
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures format as a Lot Number.
    /// </summary>
    /// <param name="LotNumber"></param>
    /// <returns>The string formatted as a Lot Number.</returns>
    /// <exception cref="FormatException"></exception>
    private static async Task<string> ValidateLotNumber(string? LotNumber, string ProcessType) {
        LotNumber = await Task.Run(async () => {
            // check if the Lot is assigned at this process or copied (pass-through)
            if (ProcessType.Equals("Originator")) {
                // Lot is assigned by serialization system and will be valid
                LotNumber = "000000000";
            }
            // validate and format the Lot Number string
            try {
                LotNumber = await ValidateDigitBase(LotNumber);
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
        });
        return LotNumber;
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures at least one digit format.
    /// </summary>
    /// <param name="DieNumber"></param>
    /// <returns>The string as only digits.</returns>
    /// <exception cref="FormatException"></exception>
    private static async Task<string> ValidateDieNumber(string? DieNumber) {
        DieNumber = await Task.Run(async () => {
            // validate and format the Die Number string
            try {
                DieNumber = await ValidateDigitBase(DieNumber);
            // the Die Number was null
            } catch (NullReferenceException) {
                throw new FormatException($"Please enter a Die Number before printing Labels.");
            }
            // ensure that the processed Die Number contains at least one digit
            if (DieNumber.Length < 0) {
                throw new FormatException($"Please enter a valid Die Number before printing Labels.");
            // add leading zeroes to enforce nine-length format
            }
            return DieNumber;
        });
        return DieNumber;
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures 3-character, uppercase alphanumeric Model Number format.
    /// </summary>
    /// <param name="ModelNumber"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    private static async Task<string> ValidateModelNumber(string? ModelNumber) {
        ModelNumber = await Task.Run(() => {
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
        });
        return ModelNumber;
    }

    /// <summary>
    /// Retrieves the requirements for SelectedProcess.
    /// </summary>
    /// <param name="SelectedProcess"></param>
    /// <returns></returns>
    /// <exception cref="NullProcessException"></exception>
    /// <exception cref="ArgumentException"></exception>
    private static async Task<List<string>> GetRequirements(Process? SelectedProcess) {
        List<string> Requirements = await Task.Run(() => {
            // ensure Process is non-null
            if (SelectedProcess == null) {
                throw new NullProcessException();
            }
            // retrieve the Process requirements for the Process in the Capture
            List<string> Reqs;
            try {
                Reqs = ProcessRequirements.GetProcessRequirements(SelectedProcess.FullName);
            // the Process selection was null
            } catch (NullProcessException) {
                throw new NullProcessException();
            // the selected Process was invalid (uncommon)
            } catch (ArgumentException) {
                throw new ArgumentException();
            }
            return Reqs;
        });
        return Requirements;
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
    public static async Task<InterfaceCapture> Validate(InterfaceCapture Capture) {
        // retrieve the Process requirements for the Process in the Capture
        List<string> Requirements = await GetRequirements(Capture.SelectedProcess);
        // attempt validations for every required field
        try {
            // validate universally required fields
            await ValidatePart(Capture.SelectedPart);
            Capture.Quantity = await ValidateQuantity(Capture.Quantity);
            await ValidateProductionDate(Capture.ProductionDate);
            await ValidateProductionShiftPicker(Capture.ProductionShift);
            Capture.OperatorID = await ValidateOperatorID(Capture.OperatorID);
            // validate jbk number if required (also adds any needed leading zeroes)
            if (Requirements.Contains("JBKNumberEntry")) {
                Capture.JBKNumber = await ValidateJBKNumber(Capture.JBKNumber, Capture.SelectedProcess.Type);
            };
            // validate lot number if required (also adds any needed leading zeroes)
            if (Requirements.Contains("LotNumberEntry")) {
                Capture.LotNumber = await ValidateLotNumber(Capture.LotNumber, Capture.SelectedProcess.Type);
            };
            // validate deburr jbk number if required (also adds any needed leading zeroes)
            if (Requirements.Contains("DeburrJBKNumberEntry")) {
                // pass ProcessType as "Pass-through" to force non-serialized check
                Capture.DeburrJBKNumber = await ValidateJBKNumber(Capture.DeburrJBKNumber, "Pass-through");
            };
            // validate die number if required
            if (Requirements.Contains("DieNumberEntry")) {
                await ValidateDieNumber(Capture.DieNumber);
            };
            // validate model number if required
            if (Requirements.Contains("ModelNumberEntry")) {
                await ValidateModelNumber(Capture.ModelNumber);
            };
        // a validation failed; pass the fail message to the view model
        } catch (FormatException _ex) {
            throw new FormatException(_ex.Message);
        }
        // the validation measures succeeded; return the modified capture object
        return Capture;
    }
}