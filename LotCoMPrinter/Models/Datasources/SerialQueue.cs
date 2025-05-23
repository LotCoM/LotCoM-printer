using LotCoMPrinter.Models.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datasources;

public class SerialQueue(string QueuePath, SerializationMode Mode, int Limit) 
{
    /// <summary>
    /// The path to the file containing queues for Mode.
    /// </summary>
    private readonly string QueuePath = QueuePath;

    /// <summary>
    /// The Serialization method for this queue.
    /// </summary>
    private readonly SerializationMode Mode = Mode;

    /// <summary>
    /// The last number allowed for use by this queue.
    /// </summary>
    private readonly int Limit = Limit;

    /// <summary>
    /// Reads the Serial Queue file and deserializes it into a JSON stream.
    /// </summary>
    /// <returns>A JObject object that contains all of the JSON stream from the file.</returns>
    private async Task<JObject> ReadAsync() 
    {
        return JObject.Parse(await File.ReadAllTextAsync(QueuePath));
    }

    /// <summary>
    /// Overwrites the Serial Queue file with a new version of the Queue.
    /// </summary>
    /// <param name="Queue">The modified queue JObject.</param>
    private async Task SaveAsync(JObject Queue) 
    {
        await File.WriteAllTextAsync(QueuePath, JsonConvert.SerializeObject(Queue));
    }

    /// <summary>
    /// Retrieves the currently queued Serial number for the Part Number, increments that Queue, and overwrites the Queue file.
    /// This method WILL consume a Serial Number from the Queue when called.
    /// </summary>
    /// <param name="Part"></param>
    public async Task<SerialNumber> ConsumeAsync(Part Part) 
    {
        // retrieve the Queue
        JObject Queue;
        try
        {
            Queue = await ReadAsync();
        }
        catch
        {
            throw new SerializationException("Failed to read the Serial queue files.");
        }
        // access the queued Serial Number for the Part
        JToken? Raw = Queue[Part.PartNumber];
        if (Raw is null)
        {
            throw new SerializationException($"No Queue found for the Part '{Part.PartNumber} {Part.PartName}'.");
        }
        int Queued;
        try
        {
            Queued = int.Parse(Raw.ToString());
        }
        catch
        {
            throw new SerializationException($"Failed to parse an integer from the Queue for the Part '{Part.PartNumber} {Part.PartName}'.");
        }
        // increment the queue
        if (Queued >= Limit) 
        {
            Queued = 0;
        }
        Queued += 1;
        Queue[Part.PartNumber] = Queued.ToString();
        // save the incremented queue and return the new SerialNumber
        await SaveAsync(Queue);
        return new SerialNumber(Mode, Part, Queued);
    }
}