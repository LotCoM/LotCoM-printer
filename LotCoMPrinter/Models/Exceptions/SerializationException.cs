namespace LotCoMPrinter.Models.Exceptions;

/// <summary>
/// Custom exception to raise when there is a failure in the Serialization of a Print Ticket.
/// </summary>
public partial class SerializationException(string Message) : Exception(message: Message) {}