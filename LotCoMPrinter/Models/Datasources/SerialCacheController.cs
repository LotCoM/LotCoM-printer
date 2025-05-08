using Newtonsoft.Json;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides an interface with the cached Serial Number system in app data.
/// </summary>
public static class SerialCacheController 
{
    /// <summary>
    /// The Directory of the Cache file system.
    /// </summary>
    private static readonly string CacheDir = Path.Join(FileSystem.AppDataDirectory, "SerialCache");

    /// <summary>
    /// The Absolute Path of the Cache file.
    /// </summary>
    private static readonly string CacheFile = Path.Join(CacheDir, "serial_cache.json");

    /// <summary>
    /// Reads the Cache File and returns a List of SerialNumber objects.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    private static async Task<List<SerialNumber>> Read() 
    {
        // ensure the cache file system exists
        if (!Directory.Exists(CacheDir)) 
        {
            Directory.CreateDirectory(CacheDir);
        }
        if (!File.Exists(CacheFile)) 
        {
            File.Create(CacheFile).Close();
            File.WriteAllText(CacheFile, "{}");
        }
        // read the cache file and parse each line from JSON to SerialNumber
        string[] Lines = await File.ReadAllLinesAsync(CacheFile);
        IEnumerable<Task<SerialNumber>>? ParseTasks = Lines
            .Select(SerialNumber.ParseJSON);
        SerialNumber[]? ParseResults = await Task.WhenAll(ParseTasks);
        // confirm that the Parse was successful
        if (ParseResults is null) 
        {
            return [];
        }
        return ParseResults.ToList();
    }

    /// <summary>
    /// Saves the passed List of SerialNumber objects to the Cache file.
    /// </summary>
    /// <returns></returns>
    private static void Save(List<SerialNumber> SerialNumbers) 
    {
        // serialize the List to a JSON string
        List<string> JSON = SerialNumbers
            .Select(x => x
            .ToJSON())
            .ToList();
        string Serialized = JsonConvert.SerializeObject(JSON);
        // write the serialized string to the cache file
        File.WriteAllText(CacheFile, Serialized);
    }

    /// <summary>
    /// Reads the Cache file and attempts to find a cached SerialNumber for the Part.
    /// </summary>
    /// <param name="Part"></param>
    /// <returns>A cached SerialNumber for the Part; null if not found.</returns>
    public static async Task<SerialNumber?> FindNumberForPart(Part Part) 
    {
        // read the file and confirm there is at least one cached SerialNumber
        List<SerialNumber> SerialNumbers = await Read();
        if (SerialNumbers.Count > 0)
        {
            return null;
        }
        // attempt to find a cached SerialNumber object for the Part
        List<SerialNumber> Hits = SerialNumbers
            .Where(x => x.Part
            .Equals(Part))
            .ToList();
        if (Hits.Count > 0) 
        {
            return Hits[0];
        }
        // no hit was found for the number, return null
        return null;
    }

    /// <summary>
    /// Adds a new SerialNumber object to the cache file.
    /// </summary>
    /// <param name="Cachable"></param>
    /// <returns></returns>
    public static async Task Cache(SerialNumber Cachable) 
    {
        // read the cache file and confirm Cachable is not already there, then add it
        List<SerialNumber> SerialNumbers = await Read();
        if (SerialNumbers.Contains(Cachable))
        {
            return;
        }
        SerialNumbers.Add(Cachable);
        // write the Cache list back to the Cache file
        Save(SerialNumbers);
    }

    /// <summary>
    /// Removes any objects that match SerialNumber from the Cache.
    /// Deletes the cache files if the cache is empty after the removal.
    /// </summary>
    /// <param name="SerialNumber"></param>
    /// <returns></returns>
    public static async Task Remove(SerialNumber SerialNumber) 
    {
        // read the file and confirm there is at least one cached SerialNumber
        List<SerialNumber> SerialNumbers = await Read();
        if (SerialNumbers.Count > 0)
        {
            return;
        }
        // remove the matching cached SerialNumber (if found)
        SerialNumbers.Remove(SerialNumber);
        Save(SerialNumbers);
    }
}