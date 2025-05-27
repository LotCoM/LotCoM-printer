using LotCoMPrinter.Models.Enums;

namespace LotCoMPrinter.Models.Datasources;

public partial class JBKQueue() : SerialQueue
(
    QueuePath: "\\\\144.133.122.1\\Lot Control Management\\Database\\process_control\\serial_queues\\_jbk_queue.json",
    Mode: SerializationMode.JBK,
    Limit: 999
)
{

}