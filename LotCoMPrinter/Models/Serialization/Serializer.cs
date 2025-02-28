using LotCoMPrinter.Models.Datasources;

namespace LotCoMPrinter.Models.Serialization;

public class Serializer() {
    /// <summary>
    /// Assigns a Serial Number to use for a new Label.
    /// If the LabelType is Partial, checks if there is a Serial Number cached for the Part Number.
    /// If not, consumes and caches the queued Serial Number.
    /// If the LabelType is Full, consumes the queued Serial Number.
    /// </summary>
    /// <param name="Part">Selection from the PartPicker control.</param>
    /// <param name="ModelNumber">Text from the ModelNumber control.</param>
    /// <param name="SerializeMode">Either 'JBK' or 'Lot'.</param>
    /// <param name="LabelType">Selection from the BasketTypePicker control.</param>
    /// <returns></returns>
    public async Task<string?> Serialize(string Part, string ModelNumber, string SerializeMode, string LabelType) {
        string? SerialNumber = null;
        // run a new CPU thread to get the serial number for this label
        await Task.Run(async () => {
            // remove the part name from the part info
            string PartNumber = Part.Split("\n")[0];
            // check if there is already a cached number
            SerialNumber = await SerialCache.FindNumberForPart(PartNumber);
            // if there was no serial number found, consume and cache a new one
            if (SerialNumber == null) {
                if (SerializeMode == "JBK") {
                    // consume the queued JBK number for this Model
                    SerialNumber = await JBKQueue.ConsumeAsync(ModelNumber);
                    // enforce 3-length format
                    while (SerialNumber.Length < 3) {
                        SerialNumber = "0" + SerialNumber;
                    }
                } else {
                    // consume the queued Lot number for this Model and cache it under the part number
                    SerialNumber = await LotQueue.ConsumeAsync(ModelNumber);
                    // enforce 9-length format
                    while (SerialNumber.Length < 9) {
                        SerialNumber = "0" + SerialNumber;
                    }
                }
                // if the label is a Partial, cache the Serial Number under the part number
                if (LabelType == "Partial") {
                    await SerialCache.CacheSerialNumber(SerialNumber, PartNumber);
                }
            }
        });
        // return the serial number
        return SerialNumber;
    }
}