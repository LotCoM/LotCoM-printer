namespace LotCoMPrinter.Models.Exceptions;

/// <summary>
/// Custom exception to raise when there is a failure in the print logging process 
/// that causes potential loss of print data in the LotCoM Printer application.
/// </summary>
/// <param name="Message"></param>
public partial class PrintLogException(string Message) : Exception(message: Message) {}