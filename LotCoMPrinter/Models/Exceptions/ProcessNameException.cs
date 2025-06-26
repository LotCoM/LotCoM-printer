namespace LotComWatcher.Models.Exceptions;

/// <summary>
/// Indicates an error referencing an undefined Process name that is not null.
/// </summary>
/// <param name="Message"></param>
public sealed class ProcessNameException(string Message) : Exception(message: Message)
{

}