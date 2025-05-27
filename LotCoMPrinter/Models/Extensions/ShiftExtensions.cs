using LotCoMPrinter.Models.Enums;

namespace LotCoMPrinter.Models.Extensions;

/// <summary>
/// Provides extension methods on the Shift enum.
/// </summary>
public static class ShiftExtensions
{
    /// <summary>
    /// Converts a Shift to a string.
    /// </summary>
    /// <param name="Shift"></param>
    /// <returns></returns>
    public static string ToString(this Shift Shift)
    {
        if (Shift == Shift.First)
        {
            return "1";
        }
        else if (Shift == Shift.Second)
        {
            return "2";
        }
        else if (Shift == Shift.Third)
        {
            return "3";
        }
        else
        {
            throw new ArgumentException($"Cannot convert {Shift} to a string.");
        }
    }
    
    /// <summary>
    /// Attempts to convert a string literal to a Shift enum value.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Shift FromString(this string String)
    {
        if (String.Equals("1"))
        {
            return Shift.First;
        }
        else if (String.Equals("2"))
        {
            return Shift.Second;
        }
        else if (String.Equals("3"))
        {
            return Shift.Third;
        }
        else
        {
            throw new ArgumentException($"Cannot convert {String} to a Shift.");
        }
    }
}