namespace LotCoMPrinter.Models.Exceptions;

/// <summary>
/// Custom exception to raise when the user attempts to print without selecting a Process in the LotCoM Printer application.
/// </summary>
public partial class NullProcessException : Exception {}