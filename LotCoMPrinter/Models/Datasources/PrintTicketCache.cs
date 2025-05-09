using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datasources;

public static class PrintTicketCache
{
    /// <summary>
    /// The top directory of the Print Ticket cache file system.
    /// </summary>
    private static readonly string CacheDir = Path.Join(FileSystem.AppDataDirectory, "PrintTicketCache");

    /// <summary>
    /// The absolute path of the Print Ticket cache file.
    /// </summary>
    private static readonly string CacheFile = Path.Join(CacheDir, "ticket_cache.json");

    /// <summary>
    /// Reads the Print Ticket cache file and returns the results as a List of PrintTicket JSON streams.
    /// </summary>
    /// <returns>A List of JSON stream strings.</returns>
    /// <exception cref="JsonReaderException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="JsonException"></exception>
    private static async Task<List<string>> Read() 
    {
        // ensure that the cache file system exists
        if (!Directory.Exists(CacheDir)) 
        {
            Directory.CreateDirectory(CacheDir);
        }
        if (!File.Exists(CacheFile)) 
        {
            File.Create(CacheFile).Close();
            File.WriteAllText(CacheFile, "{\"Cache\":[]}");
        }
        // read the cache and convert the JSON text to a List of strings
        JObject JSON = JObject.Parse(await File.ReadAllTextAsync(CacheFile));
        List<string>? Tickets;
        try
        {
            Tickets = (List<string>)JsonConvert.DeserializeObject(JSON["Cache"]!.ToString())!;
        }
        catch
        {
            throw new JsonException("Failed to deserialize the cache file.");
        }
        return Tickets;
    }

    /// <summary>
    /// Writes a List of PrintTicket objects to the cache file.
    /// Converts Tickets to JSON streams to be written.
    /// Slower than the alternative List of strings overload.
    /// </summary>
    /// <param name="Tickets"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    private static async Task Save(List<PrintTicket> Tickets) 
    {
        // convert the passed Print Tickets to JSON streams and write them to the cache
        List<string> JSONTickets = Tickets
            .Select(x => x
            .ToJSON())
            .ToList();
        string JSON = "{Cache:[";
        foreach (string _ticket in JSONTickets)
        {
            JSON = $"{JSON}{_ticket},";
        }
        // remove trailing comma, close JSON stream, and write to the cache
        JSON = JSON[..^1];
        JSON = $"{JSON}]" + "}";
        await File.WriteAllTextAsync(CacheFile, JSON);
    }

    /// <summary>
    /// Writes a List of PrintTickets, as JSON streams, to the cache file.
    /// </summary>
    /// <param name="Tickets"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    private static async Task Save(List<string> Tickets) 
    {
        string JSON = "{Cache:[";
        foreach (string _ticket in Tickets)
        {
            JSON = $"{JSON}{_ticket},";
        }
        // check if tickets were added before removing the trailing comma
        if (Tickets.Count > 0)
        {
            JSON = JSON[..^1];
        }
        // close JSON stream and write to the cache
        JSON = $"{JSON}]" + "}";
        await File.WriteAllTextAsync(CacheFile, JSON);
    }

    /// <summary>
    /// Adds Ticket to the Print Ticket cache and saves to the cache file.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <returns></returns>
    /// <exception cref="JsonReaderException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task Cache(PrintTicket Ticket) 
    {
        // read the cache, convert Ticket to JSON and add it to the list, then write the list to the cache
        List<string> JSON = await Read();
        string TicketString = Ticket.ToJSON();
        if (!JSON.Contains(TicketString))
        {
            JSON.Add(TicketString);
        }
        await Save(JSON);
    }

    /// <summary>
    /// Removes Ticket from the Print Ticket cache, if it exists.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <returns></returns>
    /// <exception cref="JsonReaderException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task Remove(PrintTicket Ticket) 
    {
        // read the cache, convert Ticket to JSON and check for a match, remove any match, then write the list to the cache
        List<string> JSON = await Read();
        string TicketString = Ticket.ToJSON();
        JSON.Remove(TicketString);
        await Save(JSON);
    }

    /// <summary>
    /// Returns the PrintTicket object at Index element of the cache file.
    /// </summary>
    /// <param name="Index"></param>
    /// <returns>A single PrintTicket object found at Index in the cache.</returns>
    /// <exception cref="JsonReaderException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="JsonException"></exception>
    public static async Task<PrintTicket> GetPrintTicketAt(int Index)
    {
        // read the cache, confirm Index is in range and attempt to parse and return a PrintTicket object
        List<string> JSON = await Read();
        if (Index >= JSON.Count)
        {
            throw new ArgumentOutOfRangeException($"The Index '{Index}' is outside the range of the cache file.");
        }
        try
        {
            return await PrintTicket.ParseJSON(JSON[Index]);
        }
        catch
        {
            throw new JsonException($"Failed to parse a PrintTicket from '{JSON[Index]}'.");
        }
    }

    /// <summary>
    /// Parses all PrintTickets in the cache and returns each as a PrintTicket object.
    /// </summary>
    /// <returns>A List of currently cached PrintTicket objects.</returns>
    /// <exception cref="JsonReaderException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<List<PrintTicket>> GetAllPrintTickets()
    {
        // read the cache, convert every JSON stream to a PrintTicket, and return that List
        List<string> JSON = await Read();
        IEnumerable<Task<PrintTicket>> ParseTasks = JSON
            .Select(PrintTicket.ParseJSON);
        PrintTicket[] ParseResults = await Task.WhenAll(ParseTasks);
        if (ParseResults is null)
        {
            return [];
        }
        return ParseResults.ToList();
    }
}