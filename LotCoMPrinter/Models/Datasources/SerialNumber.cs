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
}