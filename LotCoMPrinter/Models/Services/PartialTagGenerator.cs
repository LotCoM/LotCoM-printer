using LotCoMPrinter.Models.Datatypes;
using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Services;

public static class PartialTagGenerator
{
    /// <summary>
    /// Creates a List of strings that can be added to the Tag as Body text.
    /// </summary>
    /// <param name="Set"></param>
    /// <returns></returns>
    private static async Task<List<string>> GenerateBody(PartialDataSet Set, Part Part)
    {
        // compile the Body on a new thread
        return await Task.Run(() =>
        {
            // create a List of PartialDataSet fields to use as Body text and add fields
            List<string> Body = [];
            Body.Add($"Part: {Part.PartNumber}");
            Body.Add($"Shift: {Set.Shift!.Value}");
            Body.Add($"Quantity: {Set.Quantity}");
            Body.Add($"Operator: {Set.Operator}");
            return Body;
        });
    }

    /// <summary>
    /// Generates a Label object that mirrors the appearance of the physical Partial Production Data Tag.
    /// Composition of Asynchronous tasks.
    /// </summary>
    /// <param name="Set"></param>
    /// <returns></returns>
    /// <exception cref="LabelBuildException"></exception>
    public static async Task<PartialTag> GenerateTagAsync(PartialDataSet Set, Part Part)
    {
        // attempt to instantiate a new PartialTag Label subclass object
        PartialTag? Tag;
        try
        {
            Tag = new PartialTag();
        }
        catch
        {
            throw new LabelBuildException($"Failed to construct new Label.");
        }
        // create Label Body Text from the PartialDataSet
        List<string> Body = await GenerateBody(Set, Part);
        // apply a header and the Partial Production Data to the Tag
        await Tag.AddHeaderAsync(Part.ModelNumber.Code);
        await Tag.AddBodyTitleAsync(Part.PartName);
        await Tag.AddBodyTextAsync(Body);
        // return the Tag image
        return Tag;
    }
}