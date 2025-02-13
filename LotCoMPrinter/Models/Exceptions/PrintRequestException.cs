namespace LotCoMPrinter.Models.Exceptions;

/// <summary>
/// Custom exception to raise when there is a failure in the print spooling process 
/// that causes a failure in printing in the LotCoM Printer application.
/// </summary>
public partial class PrintRequestException(string Message) : Exception(message: Message) {}