namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a watchable object version of the Database table storing part information for the passed Process.
/// </summary>
/// <param name="Process"></param>
public class ProcessPartTable(string Process) {
    // private class properties
    private readonly string _filePath = $"\\\\144.133.122.1\\Lot Control Management\\Part Control\\{Process}.txt";

    // public class properties
    public string Process = Process;

    /// <summary>
    /// Reads all the parts from the Process Parts file.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileLoadException"></exception>
    public string[] Read() {
        // save a list for parts and read the file
        string[] Parts;
        try {
            Parts = File.ReadAllLines(_filePath);
        } catch {
            throw new FileLoadException($"Failed to read the Process Part file at {_filePath}.");
        }
        // return all but the first two lines (instruction lines)
        try {
            return Parts[2..];
        } catch {
            return [];
        }
    }
}