namespace LotCoMPrinter.Models.Datasources;

public static class OriginationTypeExtensions
{
    /// <summary>
    /// Converts an OrigininationType to a string.
    /// </summary>
    /// <param name="Type"></param>
    /// <returns></returns>
    public static string ToString(this OriginationType Type)
    {
        if (Type == OriginationType.Originator)
        {
            return "Originator";
        }
        else if (Type == OriginationType.PassThrough)
        {
            return "PassThrough";
        }
        else
        {
            return "None";
        }
    }

    /// <summary>
    /// Attempts to convert a string literal to an OriginationType enum value.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static OriginationType FromString(this string String)
    {
        if (String.Equals("Originator"))
        {
            return OriginationType.Originator;
        }
        else if (String.Equals("Pass-through"))
        {
            return OriginationType.PassThrough;
        }
        else
        {
            throw new ArgumentException($"Cannot convert {String} to an OriginationType.");
        }
    }
}