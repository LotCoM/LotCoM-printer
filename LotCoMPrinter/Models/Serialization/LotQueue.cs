using Newtonsoft.Json;

namespace LotCoMPrinter.Models.Serialization;

public static class LotQueue {

    private const string _queuePath = "\\\\144.133.122.1\\Lot Control Management\\Database\\process_control\\serial_queues\\_lot_queue.json";

    /// <summary>
    /// Reads the Lot Queue file and deserializes it into a Queue Dictionary.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    private static async Task<Dictionary<string, int>> DeserializeAsync() {
        // read the Lot queue file
        string QueueFile = await File.ReadAllTextAsync(_queuePath);
        Dictionary<string, int> QueueDictionary = await Task.Run(() => {
            // attempt to deserialize the queue file text into a dictionary
            try {
                Dictionary<string, int> Dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(QueueFile)!;
                return Dict;
            } catch {
                throw new JsonException($"Failed to deserialize the Lot # queue.");
            }
        });
        return QueueDictionary;
    }

    /// <summary>
    /// Overwrites the Lot Queue file with a new version of the Queue.
    /// </summary>
    /// <param name="QueueDictionary">The modified queue dictionary.</param>
    private static async Task SaveAsync(Dictionary<string, int> QueueDictionary) {
        // serialize the QueueDictionary to a JSON string
        string Serialized = JsonConvert.SerializeObject(QueueDictionary);
        // write the serialized string to the Lot queue file
        await File.WriteAllTextAsync(_queuePath, Serialized);
    }

    /// <summary>
    /// Retrieves the currently queued Lot number for the Part Number WITHOUT incrementing the Queue.
    /// </summary>
    /// <param name="PartNumber">The Part Number to access the queue of.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<int> QueuedAsync(string PartNumber) {
        // retrieve the Queue Dictionary
        Dictionary<string, int> QueueDictionary = await DeserializeAsync();
        // access the Lot number for the Part Number key
        int QueuedJBK;
        try {
            QueuedJBK = QueueDictionary[PartNumber];
        } catch {
            throw new ArgumentException($"Could not find a Lot # queue for the Part: {PartNumber}.");
        }
        return QueuedJBK;
    }

    /// <summary>
    /// Retrieves the currently queued Lot number for the Part Number, increments that Queue, and overwrites the Queue file.
    /// This method WILL consume a Lot number from the queue when called.
    /// </summary>
    /// <param name="PartNumber"></param>
    public static async Task<string> ConsumeAsync(string PartNumber) {
        // retrieve the Queue Dictionary
        Dictionary<string, int> QueueDictionary = await DeserializeAsync();
        int Consumed = await Task.Run(() => {
            // access the queued Lot number for the Part Number
            int Unincremented = QueueDictionary[PartNumber];
            // if the queued number is 999999999, reset to 1
            if (Unincremented >= 999999999) {
                Unincremented = 0;
            }
            // increment the Queued Lot number and save the new Queue version
            QueueDictionary[PartNumber] = Unincremented + 1;
            return Unincremented;
        });
        // save the incremented queue
        await SaveAsync(QueueDictionary);
        // return the unincremented (consumed) number
        return Consumed.ToString();
    }
}