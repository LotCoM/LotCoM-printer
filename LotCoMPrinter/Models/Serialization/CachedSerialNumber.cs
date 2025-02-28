namespace LotCoMPrinter.Models.Serialization;

/// <summary>
/// Represents a serial number cached in the serial number cache system.
/// </summary>
/// <param name="SerialNumber">The Serial Number value of the cached item.</param>
/// <param name="PartNumber">The Part Number this cached item applies to.</param>
public class CachedSerialNumber(string SerialNumber, string PartNumber) {

    private string _serialNumber = SerialNumber;
    private string _partNumber = PartNumber;

    /// <summary>
    /// Returns the serial number cached by this object.
    /// </summary>
    /// <returns></returns>
    public string GetSerialNumber() {
        return _serialNumber;
    }

    /// <summary>
    /// Returns the part number this object is cached for.
    /// </summary>
    /// <returns></returns>
    public string GetPartNumber() {
        return _partNumber;
    }

    /// <summary>
    /// Checks whether this cached number is saved for the given Part Number.
    /// </summary>
    /// <param name="PartNumber">The Part Number to check as a match.</param>
    /// <returns></returns>
    public bool IsForPart(string PartNumber) {
        // compare part numbers and return if matching
        return PartNumber.Equals(_partNumber);
    }
}