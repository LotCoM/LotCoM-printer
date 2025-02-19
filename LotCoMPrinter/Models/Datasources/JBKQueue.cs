using Newtonsoft.Json;

namespace LotCoMPrinter.Models.Datasources;

public static class JBKQueue {

    /// <summary>
    /// Increments the queued JBK # for the passed Model Number.
    /// </summary>
    /// <param name="ModelNumber">The Model # to increment the JBK # of.</param>
    /// <param name="QueueDictionary">The Queue Dictionary read from the JBK Queue file.</param>
    private static void Increment(string ModelNumber, Dictionary<string, int> QueueDictionary) {
        // access the queued JBK number for the Model number
        int Unincremented = QueueDictionary[ModelNumber];
        // if the queued number is 500, reset to 1
        if (Unincremented >= 500) {
            Unincremented = 0;
        }
        // increment the Queued JBK number
        QueueDictionary[ModelNumber] = Unincremented++;
    }

    /// <summary>
    /// Deserializes the JSON file storing the JBK Number queues for Model Numbers, retreives the queued number, 
    /// and increments that Model's queue for the next request.
    /// </summary>
    /// <param name="ModelNumber">The Model Number to request a queued number for.</param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static string Queued(string ModelNumber) {
        // read the JBK queue file
        string QueueFile = File.ReadAllText("\\\\144.133.122.1\\Lot Control Management\\database\\_jbk_queue.json");
        Dictionary<string, int> QueueDictionary = new Dictionary<string, int> {};
        // attempt to deserialize the queue file text into a dictionary
        try {
            JsonConvert.DeserializeAnonymousType(QueueFile, QueueDictionary);
        } catch {
            throw new JsonException($"Failed to deserialize the JBK # queue for the model: {ModelNumber}.");
        }
        // access the JBK number for the Model number key
        int QueuedJBK;
        try {
            QueuedJBK = QueueDictionary[ModelNumber];
        } catch {
            throw new ArgumentException($"Could not find a JBK # queue for the model: {ModelNumber}.");
        }
        // increment the queue
        Increment(ModelNumber, QueueDictionary);
        return QueuedJBK.ToString();
    }
}