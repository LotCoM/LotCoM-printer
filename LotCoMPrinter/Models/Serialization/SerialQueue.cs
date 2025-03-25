using Newtonsoft.Json;

namespace LotCoMPrinter.Models.Serialization;

public class SerialQueue(string QueuePath, string Serialization, int Limit) {
    
    private readonly string _queuePath = QueuePath;
    private readonly string _serialization = Serialization;
    private readonly int _limit = Limit;

    /// <summary>
    /// Reads the Serial Queue file and deserializes it into a Queue Dictionary.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    private async Task<Dictionary<string, int>> DeserializeAsync() {
        // read the Serial Queue file
        string QueueFile = await File.ReadAllTextAsync(_queuePath);
        Dictionary<string, int> QueueDictionary = await Task.Run(() => {
            // attempt to deserialize the Serial Queue file text into a dictionary
            try {
                Dictionary<string, int> Dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(QueueFile)!;
                return Dict;
            } catch {
                throw new JsonException($"Failed to deserialize the {_serialization} # Queue.");
            }
        });
        return QueueDictionary;
    }

    /// <summary>
    /// Overwrites the Serial Queue file with a new version of the Queue.
    /// </summary>
    /// <param name="QueueDictionary">The modified queue dictionary.</param>
    private async Task SaveAsync(Dictionary<string, int> QueueDictionary) {
        // serialize the QueueDictionary to a JSON string
        string Serialized = JsonConvert.SerializeObject(QueueDictionary);
        // write the serialized string to the Serial Queue file
        await File.WriteAllTextAsync(_queuePath, Serialized);
    }

    /// <summary>
    /// Retrieves the currently queued Serial Number for the Part Number WITHOUT incrementing the Queue.
    /// </summary>
    /// <param name="PartNumber">The Part Number to access the Queue of.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> QueuedAsync(string PartNumber) {
        // retrieve the Queue Dictionary
        Dictionary<string, int> QueueDictionary = await DeserializeAsync();
        // access the Serial Number for the Part Number key
        int QueuedSerial;
        try {
            QueuedSerial = QueueDictionary[PartNumber];
        } catch {
            throw new ArgumentException($"Could not find a {_serialization} # Queue for the Part: {PartNumber}.");
        }
        return QueuedSerial;
    }

    /// <summary>
    /// Retrieves the currently queued Serial number for the Part Number, increments that Queue, and overwrites the Queue file.
    /// This method WILL consume a Serial Number from the Queue when called.
    /// </summary>
    /// <param name="PartNumber"></param>
    public async Task<string> ConsumeAsync(string PartNumber) {
        // retrieve the Queue Dictionary
        Dictionary<string, int> QueueDictionary = await DeserializeAsync();
        int Consumed = await Task.Run(() => {
            // access the queued Serial Number for the Part Number
            int Unincremented = QueueDictionary[PartNumber];
            // if the queued Serial Number is at the limit, reset to 1
            if (Unincremented >= _limit) {
                Unincremented = 0;
            }
            // increment the queued Serial Number and save the new Queue version
            QueueDictionary[PartNumber] = Unincremented + 1;
            return Unincremented;
        });
        // save the incremented Queue
        await SaveAsync(QueueDictionary);
        // return the unincremented (consumed) Serial Number
        return Consumed.ToString();
    }
}