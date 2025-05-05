namespace LotCoMPrinter.Models.Datasources;

public partial class JBKQueue() : 
    SerialQueue(QueuePath: "\\\\144.133.122.1\\Lot Control Management\\Database\\process_control\\serial_queues\\_jbk_queue.json", 
                Mode: SerializationModes.JBK, 
                Limit: 999) {
}