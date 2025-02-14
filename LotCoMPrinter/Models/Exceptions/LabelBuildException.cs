namespace LotCoMPrinter.Models.Exceptions;

/// <summary>
/// Custom exception to raise when a Label construction is attempted but fails in the LotCoM Printer application.
/// </summary>
public partial class LabelBuildException(string Message) : Exception(message: Message) {}