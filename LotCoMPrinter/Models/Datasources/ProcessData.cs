using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides controlled access to Process data sources.
/// </summary>
public class ProcessData() 
{
    private const string Path = "\\\\144.133.122.1\\Lot Control Management\\Database\\process_control\\_process_masterlist.json";
        
    /// <summary>
    /// Contains the List of Processes produced by the last LoadData/LoadDataAsync call. 
    /// </summary>
    private List<Process>? CachedProcesses;

    /// <summary>
    /// Contains the List of Departments produced by the last LoadData/LoadDataAsync call. 
    /// </summary>
    private List<Department>? CachedDepartments;

    private bool AreProcessesLoaded => (CachedProcesses is not null) && (CachedProcesses.Count > 0);
    private bool AreDepartmentsLoaded => (CachedDepartments is not null) && (CachedDepartments.Count > 0);

    /// <summary>
    /// Synchronously loads the data from the Process Masterlist data source. Stores this data in the LastRead property.
    /// </summary>
    /// <returns>A JSON dictionary containing the Process Masterlist data.</returns>
    /// <exception cref="JsonException"></exception>
    private static JObject LoadData() 
    {
        // read the masterlist file
        return JObject.Parse(File.ReadAllText(Path));
    }

    /// <summary>
    /// Asynchronously loads the data from the Process Masterlist data source. Stores this data in the LastRead property.
    /// </summary>
    /// <exception cref="JsonException"></exception>
    private static async Task<JObject> LoadDataAsync() 
    {
        // read the masterlist file
        return JObject.Parse(await File.ReadAllTextAsync(Path));
    }

    /// <summary>
    /// Attempts to resolve a Part object from the data in Token.
    /// </summary>
    /// <param name="Token">A JToken object containing Part data.</param>
    /// <param name="ParentProcess">The known Process that the Part should belong to.</param>
    /// <returns>A Part object with data resolved from the JToken.</returns>
    /// <exception cref="FormatException"></exception>
    private Part ResolvePartFromToken(JToken Token, string ParentProcess) 
    {
        // hold variables for each Part object property
        string Number;
        string Name;
        ModelNumber Model;
        // attempt to pull the needed fields from the passed JToken
        try 
        {
            Number = Token["Number"]!.ToString();
            Name = Token["Name"]!.ToString();
            Model = new ModelNumber(Token["Model"]!.ToString());
        // one of the needed fields was not accessible
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Part object.");
        }
        // attempt to construct the Part object from the resolved data
        Part ResolvedPart;
        try 
        {
            ResolvedPart = new Part(ParentProcess, Number, Name, Model);
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Part object.");
        }
        // return the resolved Part object
        return ResolvedPart;
    }

    /// <summary>
    /// Attempts to resolve a Process object from the data in Token.
    /// </summary>
    /// <param name="Token">A JToken object containing Part data.</param>
    /// <returns>A Process object with data resolved from the JToken.</returns>
    /// <exception cref="FormatException"></exception>
    private Process ResolveProcessFromToken(JToken Token) 
    {
        // hold variables for each Process object property
        int LineCode;
        string Line;
        string Title;
        OriginationType Type;
        SerializationMode Mode;
        JToken RawParts;
        JToken RawRequirements;
        PassThroughType PassThroughType;
        // attempt to access each field of Data from the Process Token
        try 
        {
            LineCode = int.Parse(Token["LineCode"]!.ToString());
            Line = Token["Line"]!.ToString();
            Title = Token["Title"]!.ToString();
            Type = OriginationTypeExtensions.FromString(Token["Type"]!.ToString());
            Mode = SerializationModeExtensions.FromString(Token["Serialization"]!.ToString());
            RawParts = Token["Parts"]!;
            RawRequirements = Token["Requirements"]!;
            PassThroughType = PassThroughTypeExtensions.FromString(Token["PassThroughHeadingType"]!.ToString());
        // one of the needed fields was not accessible
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Process object.");
        }
        // process and add each part to the parts list individually
        List<Part> Parts = [];
        string ProcessName = $"{LineCode}-{Line}-{Title}";
        try 
        {
            Parts = RawParts
                .Select(x =>
                ResolvePartFromToken(x, ProcessName))
                .ToList();
        // one of the Tokens could not be resolved to a Part
        } 
        catch (Exception _ex) 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Process object, due to the following Part resolution failure: {_ex.Message}");
        }
        // create a RequiredFields object to parse the Process requirements into
        RequiredFields Requirements;
        try
        {
            Requirements = RequiredFields.ParseJSON(RawRequirements.ToString());
        }
        catch
        {
            throw new FormatException($"Could not resolve '{Token}' to a RequiredFields object.");
        }
        // attempt to construct the Process object from the resolved data
        Process ResolvedProcess;
        try 
        {
            ResolvedProcess = new Process(LineCode, Line, Title, Type, Mode, Parts, Requirements, PassThroughType);
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Process object.");
        }
        // return the resolved Process object
        return ResolvedProcess;
    }

    /// <summary>
    /// Attempts to resolve a Department object from the data in Token.
    /// </summary>
    /// <param name="Token">A JToken object containing Department data.</param>
    /// <returns>A Department object with data resolved from the JToken.</returns>
    /// <exception cref="FormatException"></exception>
    private static Department ResolveDepartmentFromToken(JToken Token) 
    {
        // hold variables for each Department object property
        string Title;
        string Code;
        List<string> Lines = [];
        // attempt to access each field of Data from the Department Token
        try 
        {
            Title = Token["Title"]!.ToString();
            Code = Token["Code"]!.ToString();
        // one of the needed fields was not accessible
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Department object.");
        }
        // add each Line to the Lines list individually
        try 
        {
            // resolve a Line from each Token
            Lines = Token["Lines"]!
                .Select(x => x
                .ToString())
                .ToList();
        // one of the Tokens could not be resolved to a Line
        } 
        catch (Exception _ex) 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Department object, due to the following Line resolution failure: {_ex.Message}");
        }
        // attempt to construct the Department object from the resolved data
        Department ResolvedDepartment;
        try 
        {
            ResolvedDepartment = new Department(Title, Code, Lines);
        } 
        catch 
        {
            throw new FormatException($"Could not resolve '{Token}' to a Department object.");
        }
        // return the resolved Process object
        return ResolvedDepartment;
    }

    /// <summary>
    /// Retrieves the list of Processes, as Process objects, from the Process Masterlist.
    /// Stores this list in the CachedProcesses property.
    /// </summary>
    /// <returns>A list of Process objects.</returns>
    /// <exception cref="FileLoadException"></exception>
    public List<Process> GetAllProcesses() 
    {
        if (!AreProcessesLoaded) 
        {
            // load the data from the Masterlist
            JObject NewRead = LoadData();
            // get the list of Processes in the Masterlist
            if (NewRead!["Processes"] is null) 
            {
                throw new FileLoadException("Failed to load the Processes from the Process Masterlist data source.");
            }
            // convert the Process tokens into Process objects
            List<Process> ProcessObjects;
            try
            {
                ProcessObjects = NewRead["Processes"]!
                    .Select(ResolveProcessFromToken)
                    .ToList();
            }
            catch (Exception _ex)
            {
                throw new FormatException($"Failed to load Processes due to the following exception: {_ex.Message}.");
            }
            CachedProcesses = ProcessObjects;
        }
        return CachedProcesses!;
    }

    /// <summary>
    /// Asynchronously retrieves the list of Processes, as Process objects, from the Process Masterlist.
    /// Stores this list in the CachedProcesses property.
    /// </summary>
    /// <returns>A list of Process objects.</returns>
    /// <exception cref="FileLoadException"></exception>
    public async Task<List<Process>> GetAllProcessesAsync() 
    {
        if (!AreProcessesLoaded) 
        {
            CachedProcesses = await Task.Run(async () => 
            {
                // load the data from the Masterlist
                JObject NewRead = await LoadDataAsync();
                // return the list of Processes in the Masterlist
                if (NewRead!["Processes"] is null) 
                {
                    throw new FileLoadException("Failed to load the Processes from the Process Masterlist data source.");
                }
                // convert the Process tokens into Process objects
                List<Process> ProcessObjects;
                try
                {
                    ProcessObjects = NewRead["Processes"]!
                        .Select(ResolveProcessFromToken)
                        .ToList();
                }
                catch (Exception _ex)
                {
                    throw new FormatException($"Failed to load Processes due to the following exception: {_ex.Message}.");
                }
                return ProcessObjects;
            });
        }
        return CachedProcesses!;
    }

    /// <summary>
    /// Retrieves a List of all Departments from the Process Masterlist.
    /// Stores this List in the CachedDepartments property.
    /// </summary>
    /// <returns></returns>
    public List<Department> GetAllDepartments() 
    {
        if (!AreDepartmentsLoaded) 
        {
            // load the data from the Masterlist
            JObject NewRead = LoadData();
            // get the list of Departments in the Masterlist
            if (NewRead!["Departments"] is null) 
            {
                throw new FileLoadException("Failed to load the Departments from the Process Masterlist data source.");
            }
            // convert the Department tokens into Department objects
            List<Department> DepartmentObjects;
            try
            {
                DepartmentObjects = NewRead["Departments"]!
                    .Select(ResolveDepartmentFromToken)
                    .ToList();
            }
            catch (Exception _ex)
            {
                throw new FormatException($"Failed to load Processes due to the following exception: {_ex.Message}.");
            }
            CachedDepartments = DepartmentObjects;
        }
        return CachedDepartments!;
    }

    /// <summary>
    /// Asynchronously retrieves a List of all Departments from the Process Masterlist.
    /// Stores this List in the CachedDepartments property.
    /// </summary>
    /// <returns></returns>
    public async Task<List<Department>> GetAllDepartmentsAsync() 
    {
        if (!AreDepartmentsLoaded) 
        {
            CachedDepartments = await Task.Run(async () => 
            {
                // load the data from the Masterlist
                JObject NewRead = await LoadDataAsync();
                // get the list of Departments in the Masterlist
                if (NewRead!["Departments"] is null) 
                {
                    throw new FileLoadException("Failed to load the Departments from the Process Masterlist data source.");
                }
                // convert the Department tokens into Department objects
                List<Department> DepartmentObjects;
                try
                {
                    DepartmentObjects = NewRead["Departments"]!
                        .Select(ResolveDepartmentFromToken)
                        .ToList();
                }
                catch (Exception _ex)
                {
                    throw new FormatException($"Failed to load Processes due to the following exception: {_ex.Message}.");
                }
                return DepartmentObjects;
            });
        }
        return CachedDepartments!;
    }

    /// <summary>
    /// Synchronously retrieves a list of Process Full Names.
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllProcessNames() 
    {
        GetAllProcesses();
        // create a List of all Process Names
        return CachedProcesses!
            .Select(x => x.FullName)
            .ToList();
    }

    /// <summary>
    /// Asynchronously retrieves a list of Process Full Names.
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetAllProcessNamesAsync() 
    {
        await GetAllProcessesAsync();
        // create a List of all Process Names
        return CachedProcesses!
            .Select(x => x.FullName)
            .ToList();
    }

    /// <summary>
    /// Loads and queries the Process Masterlist data for data connected to ProcessFullName. 
    /// Returns a Process object constructed from the first found match.
    /// </summary>
    /// <param name="ProcessFullName">The FULL name of a Process to query for.</param>
    /// <returns>A Process object.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Process GetIndividualProcess(string ProcessFullName) 
    {
        GetAllProcesses();
        // attempt to access the data for the passed Process
        try 
        {
            return CachedProcesses!
                .Where(x => x.FullName == ProcessFullName)
                .First();
        // no processes matched the name
        } 
        catch 
        {
            throw new ArgumentException($"Could not resolve process '{ProcessFullName}'.");
        }
    }

    /// <summary>
    /// Asynchronously loads and queries the Process Masterlist data for data connected to ProcessFullName. 
    /// Returns a Process object constructed from the first found match.
    /// </summary>
    /// <param name="ProcessFullName">The FULL name of a Process to query for.</param>
    /// <returns>A Process object.</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Process> GetIndividualProcessAsync(string ProcessFullName) 
    {
        await GetAllProcessesAsync();
        // attempt to access the data for the passed Process
        try 
        {
            return CachedProcesses!
                .Where(x => x.FullName == ProcessFullName)
                .First();
        // no processes matched the name
        } 
        catch 
        {
            throw new ArgumentException($"Could not resolve process '{ProcessFullName}'.");
        }
    }

    /// <summary>
    /// Searches for a Department that has a Title the matches DepartmentTitle.
    /// </summary>
    /// <param name="DepartmentTitle"></param>
    /// <returns>A Department object.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Department GetIndividualDepartment(string DepartmentTitle) 
    {
        GetAllDepartments();
        // try to find a match for the passed Title
        List<Department> Matches = CachedDepartments!
            .Where(x => x.Title
            .Equals(DepartmentTitle))
            .ToList();
        if (Matches.Count <= 0) 
        {
            // there was no match, throw an exception
            throw new ArgumentException($"Could not match '{DepartmentTitle}' to a defined Department.");
        }
        // return the first of the Matches
        return Matches[0];
    }

    /// <summary>
    /// Asynchronously searches for a Department that has a Title the matches DepartmentTitle.
    /// </summary>
    /// <param name="DepartmentTitle"></param>
    /// <returns>A Department object.</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Department> GetIndividualDepartmentAsync(string DepartmentTitle) 
    {
        await GetAllDepartmentsAsync();
        return await Task.Run(() => 
        {
            // try to find a match for the passed Title
            List<Department> Matches = CachedDepartments!
                .Where(x => x.Title
                .Equals(DepartmentTitle))
                .ToList();
            if (Matches.Count < 1) 
            {
                // there was no match, throw an exception
                throw new ArgumentException($"Could not match '{DepartmentTitle}' to a defined Department.");
            }
            // return the first of the Matches
            return Matches[0];
        });
    }

    /// <summary>
    /// Retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="ProcessFullName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public List<Part> GetProcessParts(string ProcessFullName) 
    {
        // load the Process' data
        Process Process = GetIndividualProcess(ProcessFullName);
        // no Part data was read
        if (Process.Parts.Count < 1) 
        {
            throw new ArgumentException($"No Part data found for the Process '{ProcessFullName}'.");
        } 
        // return the Part list
        return Process.Parts;
    }

    /// <summary>
    /// Asynchronously retrieves the Process Part list for the specified Process.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve Part Data for.</param>
    /// <returns>A list of Part objects assigned to the Process.</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<List<Part>> GetProcessPartsAsync(string ProcessFullName) 
    {
        // load the Process' data
        Process Process = await GetIndividualProcessAsync(ProcessFullName);
        // no Part data was read
        if (Process.Parts.Count < 1) 
        {
            throw new ArgumentException($"No Part data found for the Process '{ProcessFullName}'.");
        } 
        // return the Part list
        return Process.Parts;
    }

    /// <summary>
    /// Retrieves and formats ProcessFullName's Part list as a list of Displayable strings.
    /// </summary>
    /// <param name="ProcessFullName">Process FULL Name ("Code-Title") to retrieve Part Data for.</param>
    /// <returns>A List of strings.</returns>
    public async Task<List<string>> GetDisplayableProcessPartsAsync(string ProcessFullName) 
    {
        // retrieve the Process' parts
        List<Part> ProcessParts = await GetProcessPartsAsync(ProcessFullName);
        // convert each Part Token into a Displayable string
        List<string> PartStrings = ProcessParts
            .Select(x => $"{x.PartNumber}\n{x.PartName}")
            .ToList();
        // return the converted list
        return PartStrings;
    }

    /// <summary>
    /// Queries for a Part matching PartNumber in ProcessFullName's Part data.
    /// </summary>
    /// <param name="ProcessFullName">The FULL Name ("Code-Title") of the Process to query from.</param>
    /// <param name="PartNumber">The Part Number to query for within ProcessFullName's data.</param>
    /// <returns>A JToken object containing the Part data for PartNumber.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    public Part GetProcessPartData(string ProcessFullName, string PartNumber) 
    {
        // retrieve the Process' Part list
        List<Part> ProcessParts = GetProcessParts(ProcessFullName);
        // no Part data for this Process
        if (ProcessParts.Count == 0) 
        {
            throw new ArgumentException($"No Part data has been assigned to Process '{ProcessFullName}'.");
        }
        // attempt to access the specific Part
        Part? SelectedPart;
        try 
        {
            SelectedPart = ProcessParts
                .Where(x => x.PartNumber
                .Equals(PartNumber))
                .First();
        // Part was not found in the Process' Part list
        } 
        catch 
        {
            throw new ArgumentException($"Part '{PartNumber}' not found assigned to Process '{ProcessFullName}'.");
        }
        return SelectedPart;
    }

    /// <summary>
    /// Asynchronously queries for a Part matching PartNumber in ProcessFullName's Part data.
    /// </summary>
    /// <param name="ProcessFullName">The FULL Name ("Code-Title") of the Process to query from.</param>
    /// <param name="PartNumber">The Part Number to query for within ProcessFullName's data.</param>
    /// <returns>A JToken object containing the Part data for PartNumber.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    public async Task<Part> GetProcessPartDataAsync(string ProcessFullName, string PartNumber) 
    {
        // perform the query on a new CPU thread
        Part PartData = await Task.Run(async () => 
        {
            // retrieve the Process' Part list
            List<Part> ProcessParts = await GetProcessPartsAsync(ProcessFullName);
            // no Part data for this Process
            if (ProcessParts.Count == 0) 
            {
                throw new ArgumentException($"No Part data has been assigned to Process '{ProcessFullName}'.");
            }
            // attempt to access the specific Part
            Part? SelectedPart;
            try 
            {
                SelectedPart = ProcessParts
                    .Where(x => x.PartNumber
                    .Equals(PartNumber))
                    .First();
            // Part was not found in the Process' Part list
            } 
            catch 
            {
                throw new ArgumentException($"Part '{PartNumber}' not found assigned to Process '{ProcessFullName}'.");
            }
            return SelectedPart;
        });
        // return the queried Part data
        return PartData;
    }
}