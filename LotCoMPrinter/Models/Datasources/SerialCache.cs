using Newtonsoft.Json;

namespace LotCoMPrinter.Models.Datasources;

public static class SerialCache {

    private static readonly string _cacheDir = Path.Join(FileSystem.AppDataDirectory, "SerialCache");
    private static readonly string _cacheFile = Path.Join(_cacheDir, "serial_cache.json");

    /// <summary>
    /// Checks if the cache directory exists.
    /// </summary>
    /// <returns></returns>
    private static bool ConfirmCacheDirectory() {
        return Directory.Exists(_cacheDir);
    }

    /// <summary>
    /// Checks if the cache file exists.
    /// </summary>
    /// <returns></returns>
    private static bool ConfirmCacheFile() {
        return File.Exists(_cacheFile);
    }

    /// <summary>
    /// Creates the Cache filing system if it does not already exist.
    /// </summary>
    private static void CreateCache() {
        // create the cache directory
        if (!ConfirmCacheDirectory()) {
            Directory.CreateDirectory(_cacheDir);
        }
        // create the cache file
        if (!ConfirmCacheDirectory()) {
            File.Create(_cacheFile);
        }
    }

    /// <summary>
    /// Reads the Cache File and returns it as a Dictionary of (string: int).
    /// </summary>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    private static async Task<Dictionary<string, int>> Read() {
        // open the file and get its contents as a serial cache dictionary
        string CacheFile = await File.ReadAllTextAsync(_cacheFile);
        Dictionary<string, int> CacheDictionary = await Task.Run(() => {
            // attempt to deserialize the cache file text into a dictionary
            try {
                Dictionary<string, int> Dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(CacheFile)!;
                return Dict;
            } catch {
                throw new JsonException($"Failed to deserialize the Serial Cache.");
            }
        });
        return CacheDictionary;
    }

    /// <summary>
    /// Saves the passed Dictionary to the Cache file.
    /// </summary>
    /// <param name="CacheDictionary"></param>
    /// <returns></returns>
    private static async Task Save(Dictionary<string, int> CacheDictionary) {
        // serialize the CacheDictionary to a JSON string
        string Serialized = JsonConvert.SerializeObject(CacheDictionary);
        // write the serialized string to the cache file
        await File.WriteAllTextAsync(_cacheFile, Serialized);
    }

    /// <summary>
    /// Returns whether the passed Dictionary has any contents.
    /// </summary>
    /// <param name="CacheDictionary">The Dictionary built from the Cache File.</param>
    /// <returns></returns>
    private static bool HasContents(Dictionary<string, int> CacheDictionary) {
        // return if there are any keys in the dictionary (contains any cached serials)
        return CacheDictionary.Keys.Count == 0;
    }

    /// <summary>
    /// Converts a Dictionary of caches into a List of CachedSerialNumber objects.
    /// </summary>
    /// <param name="CacheDictionary"></param>
    /// <returns></returns>
    private static async Task<List<CachedSerialNumber>> BuildCacheList(Dictionary<string, int> CacheDictionary) {
        // run the conversion on a new CPU thread
        List<CachedSerialNumber> CacheList = await Task.Run(() => {
            List<CachedSerialNumber> List = [];
            // convert each key/value in the cache dictionary to a Cache object
            foreach (string _key in CacheDictionary.Keys) {
                List.Add(new CachedSerialNumber(CacheDictionary[_key].ToString(), _key));
            }
            return List;
        });
        return CacheList;
    }

    /// <summary>
    /// Adds a new Cache object to the cache file.
    /// </summary>
    /// <param name="Cachable"></param>
    /// <returns></returns>
    private static async Task Cache(CachedSerialNumber Cachable) {
        // confirm the cache file exists and create it if not
        CreateCache();
        // read the cache
        Dictionary<string, int> CacheDictionary = await Read();
        // add the cache to the dictionary  
        CacheDictionary.Add(Cachable.GetPartNumber(), int.Parse(Cachable.GetSerialNumber()));
        // write the cache back to the cache file
        await Save(CacheDictionary);
    }

    /// <summary>
    /// Reads the Cache and attempts to find a hit for the Part Number.
    /// </summary>
    /// <param name="PartNumber"></param>
    /// <returns>The cached serial number for the Part, as a string; null if not found.</returns>
    public static async Task<string?> FindNumberForPart(string PartNumber) {
        // confirm that the cache exists
        if (!ConfirmCacheDirectory() || !ConfirmCacheFile()) {
            return null;
        }
        // the cache file exists; read it and check whether it contains any numbers
        Dictionary<string, int> CacheDictionary = await Read();
        if (!HasContents(CacheDictionary)) {
            // cache is empty, will not have a number for any parts
            return null;
        }
        // convert the cache dictionary to a List of CachedSerialNumber objects
        List<CachedSerialNumber> CacheList = await BuildCacheList(CacheDictionary);
        // attempt to find a CachedSerialNumber object for the Part Number
        List<CachedSerialNumber> Hit = CacheList.Where(x => x.IsForPart(PartNumber)).ToList();
        // return the hit if there was one
        if (Hit.Count > 0) {
            return Hit[0].GetSerialNumber();
        }
        // no hit was found for the number, return nothing
        return null;
    }

    /// <summary>
    /// Converts SerialNumber and PartNumber into a CachedSerialNumber object, adds that object to the Cache, and updates the cache file.
    /// </summary>
    /// <param name="SerialNumber"></param>
    /// <param name="PartNumber"></param>
    /// <returns></returns>
    public static async Task<bool> CacheSerialNumber(string SerialNumber, string PartNumber) {
        // create a new CachedSerialNumber object
        CachedSerialNumber NewCache = new CachedSerialNumber(SerialNumber, PartNumber);
        // cache the number
        try {
            await Cache(NewCache);
            return true;
        } catch {
            return false;
        }
    }
}