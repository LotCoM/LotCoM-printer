namespace LotCoMPrinter.Models.Datasources;

public static class OriginationTypeExtensions
{
    extension(OriginationType Type)
    {
        /// <summary>
        /// Converts an OrigininationType to a string.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public string ToString()
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
    }

    extension(string String)
    {
        /// <summary>
        /// Attempts to convert a string literal to an OriginationType enum value.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public OriginationType FromString()
        {
            if (String.Equals("Originator"))
            {
                return OriginationType.Originator;
            }
            else if (String.Equals("PassThrough"))
            {
                return OriginationType.PassThrough;
            }
            else
            {
                throw new ArgumentException($"Cannot convert {String} to an OriginationType.");
            }
        }
    }
}