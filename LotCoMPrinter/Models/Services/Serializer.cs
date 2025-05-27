using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Datatypes;
using LotCoMPrinter.Models.Enums;

namespace LotCoMPrinter.Models.Services;

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
        if (Process.Serialization == SerializationMode.JBK) 
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