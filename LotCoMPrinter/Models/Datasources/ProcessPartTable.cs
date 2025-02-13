namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a watchable object version of the Database table storing part information for the passed Process.
/// </summary>
/// <param name="Process"></param>
public class ProcessPartTable(string Process) {
    // private class properties
    private DateTime _lastWriteTime;
    private readonly string _filePath = $"\\\\144.133.122.1\\Lot Control Management\\Part Control\\{Process.Replace(" ", "")}.txt";

    // public class properties
    public string Process = Process;

    /// <summary>
    /// Checks whether there has been a change to the Process' Part file since the last update.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public bool HasChanges() {
        // check the last write time of the file and compare it to the stored write time
        DateTime CurrentLastWrite = File.GetLastWriteTime(_filePath);
        if (CurrentLastWrite.CompareTo(_lastWriteTime) > 0) {
            // update the internal write time property and return that there was a change
            _lastWriteTime = CurrentLastWrite;
            return true;
        }
        // there was no change since last write
        return false;
    }
}