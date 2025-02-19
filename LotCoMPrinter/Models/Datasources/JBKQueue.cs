using Newtonsoft.Json;

namespace LotCoMPrinter.Models.Datasources;

public static class JBKQueue {

    private const string _queuePath = "\\\\144.133.122.1\\Lot Control Management\\database\\_jbk_queue.json";

    /// <summary>
    /// Reads the JBK Queue file and deserializes it into a Queue Dictionary.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    private static Dictionary<string, int> Deserialize() {
        // read the JBK queue file
        string QueueFile = File.ReadAllText(_queuePath);
        Dictionary<string, int> QueueDictionary = new Dictionary<string, int> {};
        // attempt to deserialize the queue file text into a dictionary
        try {
            JsonConvert.DeserializeAnonymousType(QueueFile, QueueDictionary);
        } catch {
            throw new JsonException($"Failed to deserialize the JBK # queue.");
        }
        return QueueDictionary;
    }

    /// <summary>
    /// Overwrites the JBK Queue file with a new version of the Queue.
    /// </summary>
    /// <param name="QueueDictionary">The modified queue dictionary.</param>
    private static void Save(Dictionary<string, int> QueueDictionary) {
        // serialize the QueueDictionary to a JSON string
        string Serialized = JsonConvert.SerializeObject(QueueDictionary);
        // write the serialized string to the JBK queue file
        File.WriteAllText(_queuePath, Serialized);
    }

    /// <summary>
    /// Retrieves the currently queued JBK number for the Model number WITHOUT incrementing the Queue.
    /// </summary>
    /// <param name="ModelNumber">The Model number to access the queue of.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int Queued(string ModelNumber) {
        // retrieve the Queue Dictionary
        Dictionary<string, int> QueueDictionary = Deserialize();
        // access the JBK number for the Model number key
        int QueuedJBK;
        try {
            QueuedJBK = QueueDictionary[ModelNumber];
        } catch {
            throw new ArgumentException($"Could not find a JBK # queue for the model: {ModelNumber}.");
        }
        return QueuedJBK;
    }

    /// <summary>
    /// Retrieves the currently queued JBK number for the Model number, increments that Queue, and overwrites the Queue file.
    /// This method WILL consume a JBK number from the queue when called.
    /// </summary>
    /// <param name="ModelNumber"></param>
    public static void Consume(string ModelNumber) {
        // retrieve the Queue Dictionary
        Dictionary<string, int> QueueDictionary = Deserialize();
        // access the queued JBK number for the Model number
        int Unincremented = QueueDictionary[ModelNumber];
        // if the queued number is 500, reset to 1
        if (Unincremented >= 500) {
            Unincremented = 0;
        }
        // increment the Queued JBK number and save the new Queue version
        QueueDictionary[ModelNumber] = Unincremented++;
        // save the incremented queue
        Save(QueueDictionary);
    }
}