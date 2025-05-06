using System.Text.RegularExpressions;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Validates values contained in InterfaceCapture objects, based on their SelectedProcess value.
/// </summary>
public static class InterfaceCaptureValidator 
{
    /// <summary>
    /// Validates a string as non-null, non-empty, and removes all non-digit characters.
    /// </summary>
    /// <param name="Value"></param>
    /// <returns>The string after removing all non-digits (may be empty).</returns>
    /// <exception cref="NullReferenceException"></exception>
    private static string ValidateDigitBase(string? Value) 
    {
        // validate that the value is not null
        if (Value is null) 
        {
            throw new NullReferenceException();
        }
        // remove any non-digit characters from the value
        return Regex.Replace(Value, @"^[^\d]+$", "");
    }

    /// <summary>
    /// Validates a Part object as non-null.
    /// </summary>
    /// <param name="Part"></param>
    /// <exception cref="FormatException"></exception>
    private static void ValidatePart(Part? Part) 
    {
        // validate that the Part is not null
        if (Part is null) 
        {
            throw new FormatException($"Please select a Part before printing Labels.");
        }
    }

    /// <summary>
    /// Validates an integer as non-null.
    /// </summary>
    /// <param name="Quantity"></param>
    /// <returns>The validated Quantity.</returns>
    /// <exception cref="FormatException"></exception>
    private static void ValidateQuantity(int? Quantity) 
    {
        // ensure that the Quantity contains at least one digit
        if (Quantity is not null) {
            throw new FormatException($"Please enter a valid Quantity before printing Labels.");
        }
    }

    /// <summary>
    /// Validates a DateTime object as non-null.
    /// </summary>
    /// <param name="Date"></param>
    /// <exception cref="FormatException"></exception>
    private static void ValidateProductionDate(DateTime? Date) 
    {
        // validate that Date is not null
        if (Date is null) {
            throw new FormatException($"Please select a Production Date before printing Labels.");
        }
    }

    /// <summary>
    /// Validates an integer as a non-null Shift Number.
    /// </summary>
    /// <param name="ProductionShift"></param>
    /// <exception cref="FormatException"></exception>
    private static void ValidateProductionShiftPicker(int? ProductionShift) 
    {
        // validate that the int is non-null
        if (ProductionShift is null) 
        {
            throw new FormatException($"Please select a Production Shift before printing Labels.");
        }
        // validate that the shift is 1, 2, or 3
        if (0 > ProductionShift || ProductionShift > 3) 
            {
            throw new FormatException($"Please select a valid Production Shift before printing Labels.");
        }
    }

    /// <summary>
    /// Validates a string as non-null. Enforces two or three length, uppercase character format.
    /// </summary>
    /// <param name="OperatorID"></param>
    /// <returns>The string as an uppercase Operator Initial.</returns>
    /// <exception cref="FormatException"></exception>
    private static string ValidateOperatorID(string? OperatorID) 
    {
        // validate that the string is non-null
        if (OperatorID is null) 
        {
            throw new FormatException("Please enter Operator Intials (ie. AB, ABC) before printing Labels.");
        }
        // set a regex pattern for 2 or 3 alphabetical characters
        string InitialPattern = @"^[a-zA-Z][a-zA-Z][a-zA-Z]?$";
        Regex InitialRegex = new Regex(InitialPattern);
        // ensure the value matches the regex requirement
        if (!InitialRegex.IsMatch(OperatorID)) 
        {
            throw new FormatException("Please enter valid Operator Intials (ie. AB, ABC) before printing Labels.");
        }
        // cast the string to Uppercase and return it
        return OperatorID.ToUpper();
    }

    /// <summary>
    /// Validates an integer as non-null. Ensures format as a JBK Number.
    /// </summary>
    /// <param name="JBKNumber"></param>
    /// <returns>The formatted JBK Number.</returns>
    /// <exception cref="FormatException"></exception>
    private static int ValidateJBKNumber(int? JBKNumber, OriginationTypes Type) 
    {
        // check if the JBK is assigned at this process or copied (pass-through)
        if (Type == OriginationTypes.Originator) 
        {
            // JBK is assigned by serialization system and will be valid
            JBKNumber = 0;
        }
        // ensure that the processed JBK Number contains at least one positive digit
        if (JBKNumber is null) 
        {
            throw new FormatException("Please enter a JBK Number before printing Labels.");
        }
        if (JBKNumber < 0) 
        {
            throw new FormatException($"Please enter a valid JBK Number before printing Labels.");
        }
        return (int)JBKNumber;
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures format as a Lot Number.
    /// </summary>
    /// <param name="LotNumber"></param>
    /// <returns>The string formatted as a Lot Number.</returns>
    /// <exception cref="FormatException"></exception>
    private static string ValidateLotNumber(string? LotNumber, OriginationTypes Type) 
    {
        // check if the Lot is assigned at this process or copied (pass-through)
        if (Type == OriginationTypes.Originator) 
        {
            // Lot is assigned by serialization system and will be valid
            LotNumber = "000000000";
        }
        // validate and format the Lot Number string
        try 
        {
            LotNumber = ValidateDigitBase(LotNumber);
        // the Lot Number was null
        } 
        catch (NullReferenceException) 
        {
            throw new FormatException($"Please enter a Lot Number before printing Labels.");
        }
        // ensure that the processed Lot Number contains at least one digit
        if (LotNumber.Length < 0) 
        {
            throw new FormatException($"Please enter a valid Lot Number before printing Labels.");
        // add leading zeroes to enforce nine-length format
        } 
        else 
        {
            while (LotNumber.Length < 9) 
            {
                LotNumber = $"0{LotNumber}";
            }
        }
        return LotNumber;
    }

    /// <summary>
    /// Validates an integer as non-null. Ensures at least one digit format.
    /// </summary>
    /// <param name="DieNumber"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    private static int ValidateDieNumber(int? DieNumber) 
    {
        // validate and format the Die Number string
        try 
        {
            DieNumber = int.Parse(ValidateDigitBase(DieNumber.ToString()));
        // the Die Number was null
        } 
        catch (NullReferenceException) 
        {
            throw new FormatException($"Please enter a Die Number before printing Labels.");
        }
        // ensure that the processed Die Number contains at least one positive digit
        if (DieNumber < 0) 
        {
            throw new FormatException($"Please enter a valid Die Number before printing Labels.");
        }
        return (int)DieNumber;
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures at least one digit format.
    /// </summary>
    /// <param name="HeatNumber"></param>
    /// <returns>The string as only digits.</returns>
    /// <exception cref="FormatException"></exception>
    private static string ValidateHeatNumber(string? HeatNumber) 
    {
        // validate and format the Heat Number string
        try 
        {
            HeatNumber = ValidateDigitBase(HeatNumber);
        // the Heat Number was null
        } 
        catch (NullReferenceException) 
        {
            throw new FormatException($"Please enter a Heat Number before printing Labels.");
        }
        // ensure that the processed Heat Number contains at least one digit
        if (HeatNumber.Length < 0) 
        {
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
    private static string ValidateModelNumber(string? ModelNumber) 
    {
        // validate that the string is non-null, non-empty, and that it only contains alnum characters
        if (ModelNumber is null || ModelNumber!.Equals("")) 
        {
            throw new FormatException("Please enter a Model Number before printing Labels.");
        }
        // set a regex pattern for 3 alphanumerical characters
        string ModelPattern = @"^[a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9]$";
        Regex ModelRegex = new Regex(ModelPattern);
        // ensure the value matches the regex requirement
        if (!ModelRegex.IsMatch(ModelNumber)) 
        {
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
    public static InterfaceCapture Validate(InterfaceCapture Capture) 
    {
        // retrieve the Process requirements for the Process in the Capture
        RequiredFields Requirements = Capture.Process.RequiredFields;
        try 
        {
            // validate universally required fields
            ValidatePart(Capture.Part);
            ValidateQuantity(Capture.Quantity);
            ValidateProductionDate(Capture.ProductionDate);
            ValidateProductionShiftPicker(Capture.ProductionShift);
            Capture.OperatorID = ValidateOperatorID(Capture.OperatorID);
            // validate jbk number if required (also adds any needed leading zeroes)
            if (Requirements.JBKNumber) 
            {
                Capture.VariableFields.JBKNumber = ValidateJBKNumber(Capture.VariableFields.JBKNumber, Capture.Process.Type);
            };
            // validate lot number if required (also adds any needed leading zeroes)
            if (Requirements.LotNumber) 
            {
                Capture.VariableFields.LotNumber = ValidateLotNumber(Capture.VariableFields.LotNumber, Capture.Process.Type);
            };
            // validate deburr jbk number if required (also adds any needed leading zeroes)
            if (Requirements.DeburrJBKNumber) 
            {
                // pass ProcessType as "Pass-through" to force non-serialized check
                Capture.VariableFields.DeburrJBKNumber = ValidateJBKNumber(Capture.VariableFields.DeburrJBKNumber, OriginationTypes.PassThrough);
            };
            // validate die number if required
            if (Requirements.DieNumber) 
            {
                ValidateDieNumber(Capture.VariableFields.DieNumber);
            };
            // validate model number if required
            if (Requirements.ModelNumber) 
            {
                ValidateModelNumber(Capture.VariableFields.ModelNumber);
            };
            // validate heat number if required
            if (Requirements.HeatNumber) 
            {
                ValidateHeatNumber(Capture.VariableFields.HeatNumber);
            };
        // a validation failed; pass the fail message to the view model
        } 
        catch (FormatException _ex) 
        {
            throw new FormatException(_ex.Message);
        }
        // the validation measures succeeded; return the modified capture object
        return Capture;
    }
}