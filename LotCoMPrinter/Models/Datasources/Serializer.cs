namespace LotCoMPrinter.Models.Datasources;

public static class Serializer 
{
    /// <summary>
    /// Assigns a Serial Number to use for a new Print Ticket.
    /// </summary>
    /// <param name="Process"></param>
    /// <param name="Part"></param>
    /// <returns></returns>
    public static async Task<SerialNumber> Serialize(Process Process, Part Part) 
    {
        // retrieve a new SerialNumber for the Part
        SerialNumber Number;
        if (Process.Serialization == SerializationModes.JBK) 
        {
            Number = await new JBKQueue().ConsumeAsync(Part);
        } 
        else 
        {
            Number = await new LotQueue().ConsumeAsync(Part);
        }
        return Number;
    }
}