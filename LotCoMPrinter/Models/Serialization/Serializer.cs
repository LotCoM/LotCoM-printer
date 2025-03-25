using LotCoMPrinter.Models.Datasources;
using LotCoMPrinter.Models.Validators;

namespace LotCoMPrinter.Models.Serialization;

public static class Serializer {
    /// <summary>
    /// Checks the Serial Cache for a Cached number under this Part Number and retrieves a new one if no Cache exists.
    /// </summary>
    /// <param name="PartNumber"></param>
    /// <param name="ModelNumber"></param>
    /// <param name="SerializeMode"></param>
    /// <returns></returns>
    private static async Task<string?> GetSerialNumber(string PartNumber, string SerializeMode) {
        // run the serial number retrieval on a new CPU thread
        string SerialNumber = await Task.Run(async () => {
            // create a new SerialCacheController to interact with the Serial Cache Files
            SerialCacheController SerialCache = new SerialCacheController();
            // check if there is already a cached number
            string? Number = await SerialCache.FindNumberForPart(PartNumber);
            // if there was no serial number found, consume and cache a new one
            if (Number == null) {
                if (SerializeMode == "JBK") {
                    // consume the queued JBK number for this Model
                    Number = await new JBKQueue().ConsumeAsync(PartNumber);
                } else {
                    // consume the queued Lot number for this Model and cache it under the part number
                    Number = await new LotQueue().ConsumeAsync(PartNumber);
                }
            }
            return Number;
        });
        return SerialNumber;
    }

    /// <summary>
    /// Formats SerialNumber according to the Serialization Mode format requirements.
    /// </summary>
    /// <param name="SerialNumber"></param>
    /// <param name="SerializeMode"></param>
    /// <returns></returns>
    private static string FormatSerialNumber(string SerialNumber, string SerializeMode) {
        // enforce leading zero-padding format
        if (SerializeMode == "JBK") {
            // enforce 3-length format
            while (SerialNumber.Length < 3) {
                SerialNumber = "0" + SerialNumber;
            }
        } else {
            // enforce 9-length format
            while (SerialNumber.Length < 9) {
                SerialNumber = "0" + SerialNumber;
            }
        }
        return SerialNumber;
    }

    /// <summary>
    /// Assigns a Serial Number to use for a new Label.
    /// If the LabelType is Partial, checks if there is a Serial Number cached for the Part Number.
    /// If not, consumes and caches the queued Serial Number.
    /// If the LabelType is Full, consumes the queued Serial Number.
    /// </summary>
    /// <param name="Capture">An InterfaceCapture object to use as a source for serialization information.</param>
    /// <returns>A Serial Number string.</returns>
    public static async Task<string?> Serialize(InterfaceCapture Capture) {
        // retrieve values from the Capture to improve processing time
        string Serialization = Capture.SelectedProcess!.Serialization;
        Part SelectedPart = Capture.SelectedPart!;
        string PartNumber = SelectedPart!.PartNumber;
        // get the serial number for this label
        string? SerialNumber = await GetSerialNumber(PartNumber, Serialization);
        // format the Serial Number
        SerialNumber = FormatSerialNumber(SerialNumber!, Serialization);
        // if the label is a Partial; cache the Serial Number under the part number
        SerialCacheController SerialCache = new SerialCacheController();
        if (Capture.BasketType == "Partial") {
            await SerialCache.CacheSerialNumber(SerialNumber, PartNumber);
        // the label is a full label; remove its serial number from the Cache
        } else {
            await SerialCache.RemoveCachedSerialNumber(SerialNumber, PartNumber);
        }
        // return the serial number
        return SerialNumber;
    }
}