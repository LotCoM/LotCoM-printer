namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides extension methods on the SerializationMode enum.
/// </summary>
public static class SerializationModeExtensions
{
    extension(SerializationMode Mode)
    {
        /// <summary>
        /// Converts a SerializationMode to a string.
        /// </summary>
        /// <param name="Mode"></param>
        /// <returns></returns>
        public string ToString()
        {
            if (Mode == SerializationMode.JBK)
            {
                return "JBK";
            }
            else if (Mode == SerializationMode.Lot)
            {
                return "Lot";
            }
            else
            {
                return "None";
            }
        }
    }

    extension(string String)
    {
        /// <summary>
        /// Attempts to convert a string literal to a SerializationMode enum value.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public SerializationMode FromString()
        {
            if (String.Equals("JBK"))
            {
                return SerializationMode.JBK;
            }
            else if (String.Equals("Lot"))
            {
                return SerializationMode.Lot;
            }
            else if (String.Equals("None"))
            {
                return SerializationMode.None;
            }
            else
            {
                throw new ArgumentException($"Cannot convert {String} to a SerializationMode.");
            }
        }
    }
}