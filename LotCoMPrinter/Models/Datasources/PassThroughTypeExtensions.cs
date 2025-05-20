namespace LotCoMPrinter.Models.Datasources;

public static class PassThroughTypeExtensions
{
    extension(PassThroughType Type)
    {
        /// <summary>
        /// Converts a PassThroughType to a string.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public string ToString()
        {
            if (Type == PassThroughType.JBK)
            {
                return "JBK";
            }
            else if (Type == PassThroughType.Lot)
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
        /// Attempts to convert a string literal to an PassThroughType enum value.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public PassThroughType FromString()
        {
            if (String.Equals("JBK"))
            {
                return PassThroughType.JBK;
            }
            else if (String.Equals("Lot"))
            {
                return PassThroughType.Lot;
            }
            else if (String.Equals("None"))
            {
                return PassThroughType.None;
            }
            else
            {
                throw new ArgumentException($"Cannot convert {String} to a PassThroughType.");
            }
        }
    }
}