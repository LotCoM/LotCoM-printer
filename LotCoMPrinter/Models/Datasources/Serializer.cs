namespace LotCoMPrinter.Models.Datasources;

public static class Serializer 
{
    /// <summary>
    /// Checks the Serial Cache for a SerialNumber for Part.
    /// </summary>
    /// <param name="Part"></param>
    /// <returns>A SerialNumber object (if one was cached for Part).</returns>
    private static async Task<SerialNumber?> CheckCache(Part Part)
    {
        return await SerialCacheController.FindNumberForPart(Part);
        
    }

    /// <summary>
    /// Retrieves a SerialNumber for Part.
    /// </summary>
    /// <param name="Part"></param>
    /// <param name="Mode"></param>
    /// <returns></returns>
    private static async Task<SerialNumber> GetSerialNumber(Part Part, SerializationModes Mode) 
    {
        // check for a cached SerialNumber for Part
        SerialNumber? Number = await CheckCache(Part);
        if (Number is not null)
        {
            return Number;
        }
        // no SerialNumber was cached for Part; retrieve a new one
        if (Mode == SerializationModes.JBK) 
        {
            Number = await new JBKQueue().ConsumeAsync(Part);
        } 
        else 
        {
            Number = await new LotQueue().ConsumeAsync(Part);
        }
        return Number;
    }

    /// <summary>
    /// Assigns a Serial Number to use for a new Label.
    /// </summary>
    /// <param name="Capture">An InterfaceCapture object to use as a source for serialization information.</param>
    /// <returns>A SerialNumber object.</returns>
    public static async Task<SerialNumber?> Serialize(InterfaceCapture Capture) 
    {
        // get a SerialNumber for this Label and cache it
        SerialNumber SerialNumber = await GetSerialNumber(Capture.Part, Capture.Process.Serialization);
        await SerialCacheController.Cache(SerialNumber);
        return SerialNumber;
    }
}