using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Datasources;

namespace LotCoMPrinter.Models.Options;

/// <summary>
/// Provides structured control over the values of each of the data fields in the variable set.
/// </summary>
/// <param name="JBKNumber"></param>
/// <param name="LotNumber"></param>
/// <param name="DeburrJBKNumber"></param>
/// <param name="DieNumber"></param>
/// <param name="ModelNumber"></param>
/// <param name="HeatNumber"></param>
public partial class VariableFieldSet(int? JBKNumber = null, string? LotNumber = null, int? DeburrJBKNumber = null, int? DieNumber = null, string? ModelNumber = null, string? HeatNumber = null) : ObservableObject()
{
    [ObservableProperty]
    public partial int? JBKNumber { get; set; } = JBKNumber;

    [ObservableProperty]
    public partial string? LotNumber { get; set; } = LotNumber;

    [ObservableProperty]
    public partial int? DeburrJBKNumber { get; set; } = DeburrJBKNumber;

    [ObservableProperty]
    public partial int? DieNumber { get; set; } = DieNumber;

    [ObservableProperty]
    public partial string? ModelNumber { get; set; } = ModelNumber;

    [ObservableProperty]
    public partial string? HeatNumber { get; set; } = HeatNumber;

    /// <summary>
    /// Validates an integer as non-null.
    /// </summary>
    /// <param name="Value"></param>
    /// <exception cref="FormatException"></exception>
    private static async Task ValidatePositiveInteger(int? Value)
    {
        // run a new thread to ensure that the Integer contains at least one positive digit
        await Task.Run(() =>
        {
            if (Value is null || Value < 1)
            {
                throw new FormatException();
            }
        });
    }

    /// <summary>
    /// Validates an integer as non-null. Ensures format as a JBK Number.
    /// </summary>
    /// <param name="Number"></param>
    /// <param name="ProcessType"></param>
    /// <returns>The formatted JBK Number.</returns>
    /// <exception cref="FormatException"></exception>
    private static async Task<int> ValidateJBKNumber(int? Number, OriginationTypes ProcessType)
    {
        // run a new thread to validate the JBK number
        return await Task.Run(() =>
        {
            // ensure that the processed JBK Number contains at least one positive digit
            if (Number is null || Number < 0)
            {
                throw new FormatException();
            }
            // ensure the JBK Number is less than 1000 (999 limit)
            if (Number > 999)
            {
                throw new FormatException();
            }
            return (int)Number;
        });
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures format as a Lot Number.
    /// </summary>
    /// <param name="String"></param>
    /// <param name="ProcessType"></param>
    /// <returns>The string formatted as a Lot Number.</returns>
    /// <exception cref="FormatException"></exception>
    private static async Task<string> ValidateLotNumber(string? String, OriginationTypes ProcessType)
    {
        // run a new thread to validate the Lot Number
        return await Task.Run(async () =>
        {
            // check if the Lot is assigned at this process or copied (pass-through)
            if (ProcessType == OriginationTypes.Originator)
            {
                String = "000000000";
            }
            // validate and format the Lot Number string
            try
            {
                await ValidatePositiveInteger(int.Parse(String!));
            }
            catch (FormatException)
            {
                throw new FormatException();
            }
            // ensure that the processed Lot Number contains at least one digit
            if (String!.Length < 0)
            {
                throw new FormatException();
                // add leading zeroes to enforce nine-length format
            }
            else
            {
                while (String.Length < 9)
                {
                    String = $"0{String}";
                }
            }
            return String;
        });
    }

    /// <summary>
    /// Validates a string as non-null, non-empty. Ensures 3-character, uppercase alphanumeric Model Number format.
    /// </summary>
    /// <param name="Number"></param>
    /// <returns>The formatted Model Number.</returns>
    /// <exception cref="FormatException"></exception>
    private static async Task<string> ValidateModelNumber(string? Number)
    {
        // run a new thread to validate the Model Number
        return await Task.Run(() =>
        {
            // validate that the string is non-null, non-empty, and that it only contains alnum characters
            if (Number is null || Number!.Equals(""))
            {
                throw new FormatException();
            }
            if (!ModelRegex().IsMatch(Number))
            {
                throw new FormatException("Please enter a valid Model Number before printing Labels.");
            }
            // cast the string to Uppercase and return it
            return Number.ToUpper();
        });
    }

    /// <summary>
    /// Validates each of the Variable Field values to ensure proper formatting and value types.
    /// </summary>
    /// <param name="ProcessType"></param>
    /// <returns>A modified (formatted) version of the object calling this method.</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<VariableFieldSet> SelfValidate(Process Process)
    {
        // validate each field and, if faulted, throw an exception with the faulting value
        if (Process.RequiredFields.JBKNumber)
        {
            try
            {
                JBKNumber = await ValidateJBKNumber(JBKNumber, Process.Type);
            }
            catch
            {
                throw new ArgumentException("JBK");
            }
        }
        if (Process.RequiredFields.LotNumber)
        {
            try
            {
                LotNumber = await ValidateLotNumber(LotNumber, Process.Type);
            }
            catch
            {
                throw new ArgumentException("Lot");
            }
        }
        if (Process.RequiredFields.DeburrJBKNumber)
        {
            try
            {
                await ValidateJBKNumber(DeburrJBKNumber, Process.Type);
            }
            catch
            {
                throw new ArgumentException("Deburr JBK");
            }
        }
        if (Process.RequiredFields.DieNumber)
        {
            try
            {
                await ValidatePositiveInteger(DieNumber);
            }
            catch
            {
                throw new ArgumentException("Die");
            }
        }
        if (Process.RequiredFields.HeatNumber)
        {
            try
            {
                await ValidatePositiveInteger(int.Parse(HeatNumber!));
            }
            catch
            {
                throw new ArgumentException("Heat");
            }
        }
        if (Process.RequiredFields.ModelNumber)
        {
            try
            {
                ModelNumber = await ValidateModelNumber(ModelNumber);
            }
            catch
            {
                throw new ArgumentException("Model");
            }
        }
        // validation is okay; return an updated version of the fields
        return this;
    }

    // COMPILED REGEX PATTERNS

    [GeneratedRegex(@"^[a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9]$")]
    private static partial Regex ModelRegex();
}