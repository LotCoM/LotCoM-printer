namespace LotComWatcher.Models.Exceptions;

/// <summary>
/// Special exception type to indicate an error interfacing with the Database.
/// </summary>
/// <param name="Message"></param>
public class DatabaseException(string Message, Exception? InnerException = null) : Exception(message: Message, innerException: InnerException)
{

}