using LotCom.Core.Enums;
using LotCom.Core.Exceptions;
using LotCom.Core.Models;
using LotCom.Core.Types;
using LotCom.Database.Services;
using Newtonsoft.Json;

namespace LotComPrinter.Models.Services;

public static class SerializationService
{
    /// <summary>
    /// Retrieves the appropriate Serial Number, based on Process and Part, from the LotCom Database.
    /// </summary>
    /// <param name="Process"></param>
    /// <param name="Part"></param>
    /// <returns></returns>
    /// <exception cref="SerializationException"></exception>
    public static async Task<SerialNumber?> Serialize(Process Process, Part Part)
    {
        // check if the process is serialized
        if (Process.Serialization == SerializationMode.None)
        {
            return null;
        }
        // retrieve a new SerialNumber for the Part
        SerialNumber Number;
        if (Process.Serialization == SerializationMode.JBK)
        {
            try
            {
                Number = await SerialFeedService.ConsumeJBKNumber(Part.Id, App.UserAgent);
            }
            catch (HttpRequestException)
            {
                throw new SerializationException($"Could not retrieve a JBK Number for the Part Id {Part.Id}.");
            }
            catch (JsonException)
            {
                throw new SerializationException($"Failed to process JSON response.");
            }
        }
        else
        {
            try
            {
                Number = await SerialFeedService.ConsumeLotNumber(Part.Id, App.UserAgent);
            }
            catch (HttpRequestException)
            {
                throw new SerializationException($"Could not retrieve a Lot Number for the Part Id {Part.Id}.");
            }
            catch (JsonException)
            {
                throw new SerializationException($"Failed to process JSON response.");
            }
        }
        return Number;
    }
}