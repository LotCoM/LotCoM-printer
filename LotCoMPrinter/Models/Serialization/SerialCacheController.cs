using Newtonsoft.Json;

namespace LotCoMPrinter.Models.Serialization;

public class SerialCacheController {
    // Cache system paths
    private static readonly string _cacheDir = Path.Join(FileSystem.AppDataDirectory, "SerialCache");
    private static readonly string _cacheFile = Path.Join(_cacheDir, "serial_cache.json");
    // runtime cache dictionary
    private Dictionary<string, int> _cacheDictionary = [];
    public Dictionary<string, int> CacheDictionary {
        get {return _cacheDictionary;}
        set {_cacheDictionary = value;}
    }

    /// <summary>
    /// Creates a controlled interface with the Serial Cache File system for the Application instance.
    /// </summary>
    public SerialCacheController() {
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
    /// Reads the Cache File and updates the Cache Dictionary in the runtime CacheDictionary property.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    private async Task Read() {
        // open the file and get its contents as a serial cache dictionary
        string CacheFile = await File.ReadAllTextAsync(_cacheFile);
        CacheDictionary = await Task.Run(() => {
            // attempt to deserialize the cache file text into a dictionary
            try {
                Dictionary<string, int> Dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(CacheFile)!;
                return Dict;
            } catch {
                throw new JsonException($"Failed to deserialize the Serial Cache.");
            }
        });
        // check that there was something in the cache
        try {
            bool _ = CacheDictionary.Keys.Count > 0;
        // the key access failed; the dict is empty
        } catch {
            throw new FileLoadException("The Cache file was empty.");
        }
    }

    /// <summary>
    /// Saves the passed Dictionary to the Cache file.
    /// </summary>
    /// <returns></returns>
    private void Save() {
        // serialize the CacheDictionary to a JSON string
        string Serialized = JsonConvert.SerializeObject(CacheDictionary);
        // write the serialized string to the cache file
        File.WriteAllText(_cacheFile, Serialized);
    }

    /// <summary>
    /// Converts CacheDictionary into a List of CachedSerialNumber objects.
    /// </summary>
    /// <returns></returns>
    private async Task<List<CachedSerialNumber>> BuildCacheList() {
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
    private async Task<Dictionary<string, int>> BuildCacheDictionary(List<CachedSerialNumber> CacheList) {
        // run the conversion on a new CPU thread
        Dictionary<string, int> NewCacheDictionary = await Task.Run(() => {
            Dictionary<string, int> Dict = [];
            // convert each List index into a key/value in the cache dictionary
            foreach (CachedSerialNumber _cached in CacheList) {
                Dict.Add(_cached.GetPartNumber(), int.Parse(_cached.GetSerialNumber()));
            }
            return Dict;
        });
        return NewCacheDictionary;
    }

    /// <summary>
    /// Adds a new Cache object to the cache file.
    /// </summary>
    /// <param name="Cachable"></param>
    /// <returns></returns>
    private async Task Cache(CachedSerialNumber Cachable) {
        // add the cache to the dictionary
        if (!CacheDictionary.ContainsKey(Cachable.GetPartNumber())) {  
            CacheDictionary.Add(Cachable.GetPartNumber(), int.Parse(Cachable.GetSerialNumber()));
            // write the cache back to the cache file
            Save();
            // update runtime
            await Read();
        }
    }

    /// <summary>
    /// Compiles a CachedSerialNumber object from the parameters, removes any matching objects from the cache, and saves it.
    /// Deletes the cache files if the cache is empty after the removal.
    /// </summary>
    /// <param name="SerialNumber"></param>
    /// <param name="PartNumber"></param>
    /// <returns></returns>
    private async Task Remove(string SerialNumber, string PartNumber) {
        // convert the Cache Dictionary into a List
        List<CachedSerialNumber> CacheList = await BuildCacheList();
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
        // save the modified cache file
        Save();
        // update runtime
        await Read();
    }

    /// <summary>
    /// Reads the Cache and attempts to find a hit for the Part Number.
    /// </summary>
    /// <param name="PartNumber"></param>
    /// <returns>The cached serial number for the Part, as a string; null if not found.</returns>
    public async Task<string?> FindNumberForPart(string PartNumber) {
        // read the cache into runtime
        await Read();
        // convert the cache dictionary to a List of CachedSerialNumber objects
        List<CachedSerialNumber> CacheList = await BuildCacheList();
        // attempt to find a CachedSerialNumber object for the Part Number
        List<CachedSerialNumber> Hits = CacheList.Where(x => x.IsForPart(PartNumber)).ToList();
        // return the hit if there was one
        if (Hits.Count > 0) {
            return Hits[0].GetSerialNumber();
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
    public async Task CacheSerialNumber(string SerialNumber, string PartNumber) {
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
    public async Task RemoveCachedSerialNumber(string SerialNumber, string PartNumber) {
        // remove leading zeroes (the cache file will not contain them)
        while (SerialNumber[0].Equals('0')) {
            SerialNumber = SerialNumber[1..];
        }
        // remove any occurrences of the cache object
        await Remove(SerialNumber, PartNumber);
    }
}