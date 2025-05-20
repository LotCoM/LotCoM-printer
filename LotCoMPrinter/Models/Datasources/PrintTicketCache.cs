using Newtonsoft.Json;

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
    private static readonly string CacheFile = Path.Join(CacheDir, "ticket_cache.txt");

    /// <summary>
    /// Reads the Print Ticket cache file and returns the results as a List of PrintTicket JSON streams.
    /// </summary>
    /// <returns>A List of JSON stream strings.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    private static List<string> Read() 
    {
        // ensure that the cache file system exists
        if (!Directory.Exists(CacheDir)) 
        {
            Directory.CreateDirectory(CacheDir);
        }
        if (!File.Exists(CacheFile)) 
        {
            File.Create(CacheFile).Close();
        }
        // read the cache and convert the text to a List of strings
        string[] Tickets = File.ReadAllLines(CacheFile);
        return Tickets.ToList();
    }

    /// <summary>
    /// Asynchronously reads the Print Ticket cache file and returns the results as a List of PrintTicket JSON streams.
    /// </summary>
    /// <returns>A List of JSON stream strings.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    private static async Task<List<string>> ReadAsync() 
    {
        // ensure that the cache file system exists
        if (!Directory.Exists(CacheDir)) 
        {
            Directory.CreateDirectory(CacheDir);
        }
        if (!File.Exists(CacheFile)) 
        {
            File.Create(CacheFile).Close();
        }
        // read the cache and convert the text to a List of strings
        string[] Tickets = await File.ReadAllLinesAsync(CacheFile);
        return Tickets.ToList();
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
    public static void Save(List<PrintTicket> Tickets) 
    {
        // convert the passed Print Tickets to JSON streams and write them to the cache
        List<string> JSONTickets = Tickets
            .Select(x => x
            .ToJSON())
            .ToList();
        string JSON = "";
        foreach (string _ticket in JSONTickets)
        {
            JSON = $"{JSON}{_ticket}\n";
        }
        File.WriteAllText(CacheFile, JSON);
    }
    
    /// <summary>
    /// Asynchronously writes a List of PrintTicket objects to the cache file.
    /// Converts Tickets to JSON streams to be written.
    /// Slower than the alternative List of strings overload.
    /// </summary>
    /// <param name="Tickets"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public static async Task SaveAsync(List<PrintTicket> Tickets) 
    {
        // convert the passed Print Tickets to JSON streams and write them to the cache
        List<string> JSONTickets = Tickets
            .Select(x => x
            .ToJSON())
            .ToList();
        string JSON = "";
        foreach (string _ticket in JSONTickets)
        {
            JSON = $"{JSON}{_ticket}\n";
        }
        await File.WriteAllTextAsync(CacheFile, JSON);
    }

    /// <summary>
    /// Adds Ticket to the Print Ticket cache and saves to the cache file.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<List<PrintTicket>> Cache(PrintTicket Ticket)
    {
        // get all of the cached PrintTickets and check if the ticket is already cached
        List<PrintTicket> Tickets = await GetAllPrintTicketsAsync();
        bool Hit = false;
        int HitIndex = 0;
        foreach (PrintTicket _ticket in Tickets)
        {
            if (_ticket.Title.Equals(Ticket.Title))
            {
                Hit = true;
                break;
            }
            HitIndex += 1;
        }
        // if the ticket is not already cached, add it to the list
        if (!Hit)
        {
            Tickets.Add(Ticket);
        }
        // otherwise, update the ticket in the cache
        else
        {
            Tickets[HitIndex] = Ticket;
        }
        // save the updated Ticket list
        await SaveAsync(Tickets);
        return Tickets;
    }

    /// <summary>
    /// Removes Ticket from the Print Ticket cache, if it exists.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<List<PrintTicket>> Remove(PrintTicket Ticket) 
    {
        // read the cache, convert Ticket to JSON and check for a match, remove any match, then write the list to the cache
        List<PrintTicket> Tickets = await GetAllPrintTicketsAsync();
        Tickets.Remove(Ticket);
        await SaveAsync(Tickets);
        return Tickets;
    }

    /// <summary>
    /// Returns the PrintTicket object at Index element of the cache file.
    /// </summary>
    /// <param name="Index"></param>
    /// <returns>A single PrintTicket object found at Index in the cache.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="JsonException"></exception>
    public static async Task<PrintTicket> GetPrintTicketAt(int Index)
    {
        // read the cache, confirm Index is in range and attempt to parse and return a PrintTicket object
        List<string> JSON = await ReadAsync();
        if (Index >= JSON.Count)
        {
            throw new ArgumentOutOfRangeException($"The Index '{Index}' is outside the range of the cache file.");
        }
        try
        {
            return await PrintTicket.ParseJSONAsync(JSON[Index]);
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
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static List<PrintTicket> GetAllPrintTickets()
    {
        // read the cache, convert every JSON stream to a PrintTicket, and return that List
        List<string> JSON = Read();
        if (JSON.Count == 0 || JSON[0].Equals(string.Empty))
        {
            return [];
        }
        return JSON
            .Select(PrintTicket.ParseJSON)
            .ToList();
    }

    /// <summary>
    /// Asynchronously parses all PrintTickets in the cache and returns each as a PrintTicket object.
    /// </summary>
    /// <returns>A List of currently cached PrintTicket objects.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<List<PrintTicket>> GetAllPrintTicketsAsync()
    {
        // read the cache, convert every JSON stream to a PrintTicket, and return that List
        List<string> JSON = await ReadAsync();
        if (JSON.Count == 0 || JSON[0].Equals(string.Empty))
        {
            return [];
        }
        IEnumerable<Task<PrintTicket>> ParseTasks = JSON
            .Select(PrintTicket.ParseJSONAsync);
        PrintTicket[] ParseResults = await Task.WhenAll(ParseTasks);
        if (ParseResults is null)
        {
            return [];
        }
        return ParseResults.ToList();
    }
}