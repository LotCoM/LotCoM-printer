using LotCoMPrinter.Models.Datatypes;
using LotCoMPrinter.Models.Exceptions;
using LotCoMPrinter.Models.Extensions;

namespace LotCoMPrinter.Models.Services;

public static class LabelGenerator 
{
    /// <summary>
    /// Creates a QR Code from the data captured by Ticket.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static async Task<QRCode> GenerateCode(PrintTicket Ticket)
    {
        // compile the QR code data on a new thread
        return await Task.Run(() =>
        {
            // create a List of PrintTicket fields to use as QR Code data and add the first required fields
            List<string> Data = [];
            Data.AddRange([Ticket.Process.FullName, Ticket.Part.PartNumber, Ticket.Part.PartName, Ticket.GetCombinedQuantities()]);
            // retrieve the Process Requirements and add Variable Fields where required
            RequiredFields RequiredFields = Ticket.Process.RequiredFields;
            if (RequiredFields.JBKNumber)
            {
                Data.Add(Ticket.VariableFields.JBKNumber!.Formatted);
            }
            if (RequiredFields.LotNumber)
            {
                Data.Add(Ticket.VariableFields.LotNumber!.Formatted);
            }
            if (RequiredFields.DeburrJBKNumber)
            {
                Data.Add(Ticket.VariableFields.DeburrJBKNumber!.Formatted);
            }
            if (RequiredFields.DieNumber)
            {
                Data.Add(Ticket.VariableFields.DieNumber!.Literal.ToString());
            }
            if (RequiredFields.ModelNumber)
            {
                Data.Add(Ticket.VariableFields.ModelNumber!.Code);
            }
            if (RequiredFields.HeatNumber)
            {
                Data.Add(Ticket.VariableFields.HeatNumber!.Literal.ToString());
            }
            // add the remaining required fields and use the data to create a QR code
            Data.AddRange([new Timestamp(Ticket.ProductionDate).Stamp, Ticket.GetCombinedShifts(), Ticket.GetCombinedOperators()]);
            try
            {
                return new QRCode(Data);
            }
            catch
            {
                throw new ArgumentException("There was an error creating a QR Code for the new Label.");
            }
        });
    }

    /// <summary>
    /// Creates a List of strings that can be add to the Label as Body text.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <returns></returns>
    private static async Task<List<string>> GenerateBody(PrintTicket Ticket)
    {
        // compile the Body on a new thread
        return await Task.Run(() =>
        {
            // create a List of PrintTicket fields to use as Body text and add the first required fields
            List<string> Body = [];
            Body.Add($"Process: {Ticket.Process.FullName}");
            Body.Add($"Part: {Ticket.Part.PartNumber}");
            Body.Add($"Quantity: {Ticket.GetTotalQuantity().Value}");
            // retrieve the Process Requirements and add Variable Fields where required
            RequiredFields RequiredFields = Ticket.Process.RequiredFields;
            if (RequiredFields.JBKNumber)
            {
                Body.Add($"JBK #: {Ticket.VariableFields.JBKNumber!.Formatted}");
            }
            if (RequiredFields.LotNumber)
            {
                Body.Add($"Lot #: {Ticket.VariableFields.LotNumber!.Formatted}");
            }
            if (RequiredFields.DeburrJBKNumber)
            {
                Body.Add($"Deburr JBK #: {Ticket.VariableFields.DeburrJBKNumber!.Formatted}");
            }
            if (RequiredFields.DieNumber)
            {
                Body.Add($"Die #: {Ticket.VariableFields.DieNumber!.Literal}");
            }
            if (RequiredFields.ModelNumber)
            {
                Body.Add($"Model #: {Ticket.VariableFields.ModelNumber!.Code}");
            }
            if (RequiredFields.HeatNumber)
            {
                Body.Add($"Heat #: {Ticket.VariableFields.HeatNumber!.Literal}");
            }
            // add the remaining required fields and return the body text
            Body.Add($"Date: {new Timestamp(Ticket.ProductionDate).Stamp}");
            Body.Add($"Shift: {ShiftExtensions.ToString(Ticket.ProductionShift)}");
            return Body;
        });
    }

    /// <summary>
    /// Generates a Label object that mirrors the appearance of the physical Basket Label.
    /// Composition of Asynchronous tasks.
    /// </summary>
    /// <param name="Ticket"></param>
    /// <returns></returns>
    /// <exception cref="LabelBuildException"></exception>
    public static async Task<BasketLabel> GenerateLabelAsync(PrintTicket Ticket)
    {
        // create a new Label
        BasketLabel? Label;
        try
        {
            Label = new BasketLabel();
        }
        catch
        {
            throw new LabelBuildException($"Failed to construct new Label.");
        }
        // create Label Fields and generate a QR Code from the PrintTicket
        QRCode? Code = await GenerateCode(Ticket);
        List<string> Body = await GenerateBody(Ticket);
        // apply the header, the QR Code, and the Label Data to the Label
        await Label.AddHeaderAsync(Ticket.SerialNumber.GetFormattedValue());
        await Label.AddSubHeadingAsync(Ticket.Part.ModelNumber.Code);
        await Label.AddBodyTitleAsync(Ticket.Part.PartName);
        await Label.AddQRCodeAsync(Code);
        await Label.AddBodyTextAsync(Body);
        // return the Label image
        return Label;
    }
}