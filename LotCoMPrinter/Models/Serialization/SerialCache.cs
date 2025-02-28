using Newtonsoft.Json;

namespace LotCoMPrinter.Models.Serialization;

public static class SerialCache {

    private static readonly string _cacheDir = Path.Join(FileSystem.AppDataDirectory, "SerialCache");
    private static readonly string _cacheFile = Path.Join(_cacheDir, "serial_cache.json");

    /// <summary>
    /// Creates the Cache filing system if it does not already exist.
    /// </summary>
    private static void CreateCache() {
        // create the cache directory
        if (!Directory.Exists(_cacheDir)) {
            Directory.CreateDirectory(_cacheDir);
        }
        // create the cache file
        if (!File.Exists(_cacheFile)) {
            File.Create(_cacheFile);
        }
    }

    /// <summary>
    /// Returns whether the passed Dictionary has any contents.
    /// </summary>
    /// <param name="CacheDictionary">The Dictionary built from the Cache File.</param>
    /// <returns></returns>
    private static bool HasContents(Dictionary<string, int> CacheDictionary) {
        // return if there are any keys in the dictionary (contains any cached serials)
        try {
            return CacheDictionary.Keys.Count > 0;
        // the key access failed; the dict is empty
        } catch {
            return false;
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
        // check that there was something in the cache
        if (HasContents(CacheDictionary)) {
            return CacheDictionary;
        // the file was empty
        } else {
            throw new FileLoadException("The Cache file was empty.");
        }
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
    /// Converts a List of CachedSerialNumber objects into a Cache Dictionary.
    /// </summary>
    /// <param name="CacheList"></param>
    /// <returns></returns>
    private static async Task<Dictionary<string, int>> BuildCacheDictionary(List<CachedSerialNumber> CacheList) {
        // run the conversion on a new CPU thread
        Dictionary<string, int> CacheDictionary = await Task.Run(() => {
            Dictionary<string, int> Dict = [];
            // convert each List index into a key/value in the cache dictionary
            foreach (CachedSerialNumber _cached in CacheList) {
                Dict.Add(_cached.GetPartNumber(), int.Parse(_cached.GetSerialNumber()));
            }
            return Dict;
        });
        return CacheDictionary;
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
        Dictionary<string, int> CacheDictionary = [];
        try {
            CacheDictionary = await Read();
        // the empty file is not a problem here
        } catch {}
        // add the cache to the dictionary  
        CacheDictionary.Add(Cachable.GetPartNumber(), int.Parse(Cachable.GetSerialNumber()));
        // write the cache back to the cache file
        await Save(CacheDictionary);
    }

    /// <summary>
    /// Compiles a CachedSerialNumber object from the parameters, removes any matching objects from the cache, and saves it.
    /// Deletes the cache files if the cache is empty after the removal.
    /// </summary>
    /// <param name="SerialNumber"></param>
    /// <param name="PartNumber"></param>
    /// <returns></returns>
    private static async Task Remove(string SerialNumber, string PartNumber) {
        // confirm the cache file exists
        if (!Directory.Exists(_cacheDir) || !File.Exists(_cacheFile)) {
            // cannot remove a cached object from non-existent cache
            return;
        }
        // read the cache
        Dictionary<string, int> CacheDictionary = [];
        try {
            CacheDictionary = await Read();
            // convert the Cache Dictionary into a List
            List<CachedSerialNumber> CacheList = await BuildCacheList(CacheDictionary);
            // remove the matching cached object
            for (int i = 0; i < CacheList.Count; i += 1) {
                CachedSerialNumber _cached = CacheList[i];
                // if the numbers match, remove the cached item
                if (_cached.GetPartNumber().Equals(PartNumber) && _cached.GetSerialNumber().Equals(SerialNumber)) {
                    CacheList.RemoveAt(i);
                    break;
                }
            }
            // convert the List back to a Dictionary
            CacheDictionary = await BuildCacheDictionary(CacheList);
        // cannot remove anything from an empty file
        } catch {}
        // check if the modified cache dictionary has any contents
        if (!HasContents(CacheDictionary)) {
            // delete the cache files
            File.Delete(_cacheFile);
            Directory.Delete(_cacheDir);
        // there are cached objects remaining; save the cache file
        } else {
            await Save(CacheDictionary);
        }
    }

    /// <summary>
    /// Reads the Cache and attempts to find a hit for the Part Number.
    /// </summary>
    /// <param name="PartNumber"></param>
    /// <returns>The cached serial number for the Part, as a string; null if not found.</returns>
    public static async Task<string?> FindNumberForPart(string PartNumber) {
        // confirm that the cache exists
        if (!Directory.Exists(_cacheDir) || !File.Exists(_cacheFile)) {
            return null;
        }
        // the cache file exists; read it
        Dictionary<string, int> CacheDictionary = [];
        try {
            CacheDictionary = await Read();
        // the empty file will not contain cached serial numbers
        } catch {
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
    /// Adds a Serial Number to the Cache.
    /// </summary>
    /// <param name="SerialNumber"></param>
    /// <param name="PartNumber"></param>
    /// <returns></returns>
    public static async Task CacheSerialNumber(string SerialNumber, string PartNumber) {
        // create a new CachedSerialNumber object
        CachedSerialNumber NewCache = new CachedSerialNumber(SerialNumber, PartNumber);
        // cache the number
        await Cache(NewCache);
    }

    /// <summary>
    /// Removes a Serial Number from the Cache.
    /// </summary>
    /// <param name="SerialNumber"></param>
    /// <param name="PartNumber"></param>
    /// <returns></returns>
    public static async Task RemoveCachedSerialNumber(string SerialNumber, string PartNumber) {
        // remove any occurrences of the cache object
        await Remove(SerialNumber, PartNumber);
    }
}