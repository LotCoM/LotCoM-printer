namespace LotCoMPrinter.Models.Datasources;

public static class Serializer 
{
    /// <summary>
    /// Retrieves a SerialNumber for Part.
    /// </summary>
    /// <param name="Part"></param>
    /// <param name="Mode"></param>
    /// <returns></returns>
    private static async Task<SerialNumber> GetSerialNumber(Part Part, SerializationModes Mode) 
    {
        // retrieve a new SerialNumber for the Part
        SerialNumber Number;
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
        // get a SerialNumber for this Label
        return await GetSerialNumber(Capture.Part, Capture.Process.Serialization);
    }
}