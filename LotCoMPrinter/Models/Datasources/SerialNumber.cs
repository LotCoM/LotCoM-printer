using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LotCoMPrinter.Models.Datasources;

public class SerialNumber
{
    /// <summary>
    /// The mode of Serialization that the Serial Number uses.
    /// </summary>
    public SerializationModes Mode {get;} = SerializationModes.None;

    /// <summary>
    /// The Part the Serial Number has been assigned to.
    /// </summary>
    public Part? Part {get;} = null;

    /// <summary>
    /// The Serial Number's literal value.
    /// </summary>
    public object? LiteralValue {get; private set;} = null;

    /// <summary>
    /// The Type of the Serial Number's Value property.
    /// </summary>
    public Type LiteralType {get; private set;} = typeof(object);

    /// <summary>
    /// Confirms that the passed Literal Value can be used to create a Serial Number of the passed Serialization Mode.
    /// </summary>
    /// <returns>true (if successful).</returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    private bool ValidateLiteral()
    {
        if (LiteralValue is null)
        {
            throw new NullReferenceException("Cannot create a Serial Number without a value.");
        }
        if (Mode == SerializationModes.None)
        {
            throw new NullReferenceException("Cannot create a Serial Number without a Serialization Mode.");
        }
        if (Mode == SerializationModes.JBK)
        {
            // set the LiteralType property to int and confirm the Value property is valid
            if (!LiteralType.GetType().Equals(typeof(int)))
            {
                LiteralType = typeof(int);
            }
            if (!LiteralValue.GetType().Equals(LiteralType.GetType()))
            {
                try
                {
                    LiteralValue = int.Parse(LiteralValue.ToString()!);
                }
                catch
                {
                    throw new ArgumentException($"Cannot parse a valid 'int' from '{LiteralValue}' to apply to the Serial Number with SerializationMode 'JBK'.");
                }
            }
        }
        else
        {
            // set the LiteralType property to string and confirm the Value property is valid
            if (!LiteralType.GetType().Equals(typeof(string)))
            {
                LiteralType = typeof(string);
            }
            if (!LiteralValue.GetType().Equals(LiteralType.GetType()))
            {
                try
                {
                    LiteralValue = LiteralValue.ToString();
                }
                catch
                {
                    throw new ArgumentException($"Cannot parse a valid 'string' from '{LiteralValue}' to apply to the Serial Number with SerializationMode 'Lot'.");
                }
            }
        }
        // the Serial Number can be created with the passed Literal Value
        return true;
    }

    /// <summary>
    /// Create a new Serial Number using SerializationMode. 
    /// Verifies that LiteralValue can be used as a Value for a Serial Number using the passed Serialization Mode.
    /// </summary>
    /// <param name="Mode">The mode of Serialization this Serial Number uses.</param>
    /// <param name="LiteralValue">The Value to attempt to apply to this Serial Number.</param>
    /// <exception cref="ArgumentException"></exception>
    public SerialNumber(SerializationModes Mode, Part Part, object LiteralValue)
    {
        this.Mode = Mode;
        this.Part = Part;
        this.LiteralValue = LiteralValue;
        LiteralType = LiteralValue.GetType();
        // ensure that the passed literal value matches the required Serial Mode type
        if (!ValidateLiteral())
        {
            throw new ArgumentException($"Cannot create a Serial Number of this SerializationMode with the passed literal value '{LiteralValue}'");
        }
    }

    /// <summary>
    /// Formats the Serial Number as a JSON string that can be written to a File and parsed as JSON text.
    /// </summary>
    /// <returns></returns>
    public string ToJSON()
    {   
        return 
            "{" +
                $"Mode:{Mode}," +
                "Part:{" +
                    $"PartNumber:{Part!.PartNumber}," +
                    $"Process{Part!.ParentProcess}" +
                "}," +
                $"LiteralValue:{LiteralValue}," +
                $"LiteralType:{LiteralType}" +
            "}";
    }

    /// <summary>
    /// Attempts to parse a SerialNumber object from a JSON formatted string.
    /// </summary>
    /// <remarks>
    /// Throws JsonException if there were any errors parsing any part of the Serial Number from Line.
    /// Throws ArgumentException if the parsed Part Number was not defined for the parsed Process.
    /// </remarks>
    /// <param name="Line"></param>
    /// <returns>A SerialNumber object.</returns>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<SerialNumber> ParseJSON(string Line)
    {
        SerializationModes Mode;
        Part Part;
        object LiteralValue;
        Type LiteralType;
        // parse Line into JSON
        JObject JSON = JObject.Parse(Line);
        JToken? RawMode = JSON["Mode"];
        JToken? RawPart = JSON["Part"];
        JToken? RawLiteralValue = JSON["LiteralValue"];
        JToken? RawLiteralType = JSON["LiteralType"];
        // confirm the Mode key has a valid value and convert it to a SerializationMode
        if (RawMode is null)
        {
            throw new JsonException($"No Serialization Mode found in cached Serial Number '{Line}'.");
        }
        if (RawMode.ToString().Equals("JBK"))
        {
            Mode = SerializationModes.JBK;
        } else if (RawMode.ToString().Equals("Lot"))
        {
            Mode = SerializationModes.Lot;
        } else {
            throw new JsonException($"Invalid Serialization Mode '{RawMode}'");
        }
        // confirm the Part key has a valid value and convert it to a Part
        if (RawPart is null)
        {
            throw new JsonException($"No Part found in cached Serial Number '{Line}'.");
        }
        string? PartNumber;
        string? ProcessName;
        try
        {
            PartNumber = RawPart["PartNumber"]!.ToString();
            ProcessName = RawPart["Process"]!.ToString();
        }
        catch
        {
            throw new JsonException($"Could not parse a Part Number and/or Process from cached Serial Number '{Line}'.");
        }
        if (PartNumber is null || ProcessName is null)
        {
            throw new JsonException($"Could not parse a Part Number and/or Process from cached Serial Number '{Line}'.");
        }
        try
        {
            Part = await new ProcessData().GetProcessPartDataAsync(PartNumber, ProcessName);
        }
        catch
        {
            throw new ArgumentException($"The Part '{PartNumber}' for Process '{ProcessName}' was not defined.");
        }
        // confirm the LiteralType key has a non-null value and is either int or string
        if (RawLiteralType is null) 
        {
            throw new JsonException($"No LiteralType found in cached Serial Number '{Line}'.");
        }
        Type? ParsedType = Type.GetType(RawLiteralType.ToString());
        if (ParsedType is not null && 
            (ParsedType.Equals(typeof(int)) || ParsedType.Equals(typeof(string))))
        {
            LiteralType = ParsedType;
        }
        else
        {
            throw new JsonException($"Invalid LiteralType '{ParsedType}'.");
        }
        // confirm the LiteralValue key has a valid value and that it matches the LiteralType
        if (RawLiteralValue is null) 
        {
            throw new JsonException($"No LiteralValue found in cached Serial Number '{Line}'.");
        }
        if (Mode == SerializationModes.JBK)
        {
            try
            {
                LiteralValue = int.Parse(RawLiteralValue.ToString());
            }
            catch
            {
                throw new JsonException($"Invalid LiteralValue '{RawLiteralValue}' for SerializationMode 'JBK'.");
            }
        }
        else
        {
            LiteralValue = int.Parse(RawLiteralValue.ToString());
        }
        if (!LiteralValue.GetType().Equals(LiteralType))
        {
            throw new JsonException($"LiteralValue '{LiteralValue}' is of an invalid Type for SerializationMode 'JBK'.");
        }
        // use JToken approach from Client app to parse out data fields
        return new SerialNumber(Mode, Part, LiteralValue);
    }
}