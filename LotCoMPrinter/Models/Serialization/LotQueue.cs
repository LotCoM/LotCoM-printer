namespace LotCoMPrinter.Models.Serialization;

public partial class LotQueue() : 
    SerialQueue(QueuePath: "\\\\144.133.122.1\\Lot Control Management\\Database\\process_control\\serial_queues\\_lot_queue.json", 
                Serialization: "Lot", 
                Limit: 999999999) {
}