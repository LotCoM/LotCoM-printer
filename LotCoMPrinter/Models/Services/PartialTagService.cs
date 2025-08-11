using LotComPrinter.Models.Datatypes;
using LotCom.Types.Enums;
using LotCom.Types.Extensions;
using LotCom.Types;
using LotComPrinter.Models.Exceptions;

namespace LotComPrinter.Models.Services;

public static class PartialTagService
{
    /// <summary>
    /// Creates a List of strings that can be added to the Tag as Body text.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <returns></returns>
    private static async Task<List<string>> GenerateBody(PrintTicket Ticket, int PartialSetNumber)
    {
        // compile the Body on a new thread
        return await Task.Run(() =>
        {
            // create a List of PartialDataSet fields to use as Body text and add fields
            List<string> Body = [];
            Body.Add($"Basket Part: {Ticket.Part.PartNumber}");
            Body.Add($"Basket Date: {new Timestamp(Ticket.ProductionDate).Stamp}");
            // retrieve the proper PartialDataSet shift, quantity, and operator
            Shift PartialShift;
            Quantity PartialQuantity;
            Operator PartialOperator;
            if (PartialSetNumber == 1)
            {
                PartialShift = Ticket.SecondaryData!.Shift;
                PartialQuantity = Ticket.SecondaryData!.Quantity;
                PartialOperator = Ticket.SecondaryData!.Operator;
            }
            else
            {
                PartialShift = Ticket.TertiaryData!.Shift;
                PartialQuantity = Ticket.TertiaryData!.Quantity;
                PartialOperator = Ticket.TertiaryData!.Operator;
            }
            Body.Add($"Partial Shift: {ShiftExtensions.ToString(PartialShift)}");
            Body.Add($"Partial Quantity: {PartialQuantity.Value}");
            Body.Add($"Partial Operator: {PartialOperator.Initials}");
            return Body;
        });
    }

    /// <summary>
    /// Generates a Label object that mirrors the appearance of the physical Partial Production Data Tag.
    /// Composition of Asynchronous tasks.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <returns></returns>
    /// <exception cref="LabelBuildException"></exception>
    public static async Task<PartialTag> GenerateTagAsync(PrintTicket Ticket, int PartialSetNumber)
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
        List<string> Body = await GenerateBody(Ticket, PartialSetNumber);
        // apply a header and the Partial Production Data to the Tag
        await Tag.AddHeaderAsync(Ticket.Part.ModelNumber.Code);
        await Tag.AddBodyTitleAsync(Ticket.Part.PartName);
        await Tag.AddBodyTextAsync(Body);
        // return the Tag image
        return Tag;
    }
}