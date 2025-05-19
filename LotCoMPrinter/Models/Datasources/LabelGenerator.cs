using LotCoMPrinter.Models.Exceptions;

namespace LotCoMPrinter.Models.Datasources;

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
            // compile the Partial Production Data Sets into single field values
            string FullShift = $"{Ticket.ProductionShift}";
            string FullQuantity = $"{Ticket.ProductionQuantity}";
            string FullOperator = $"{Ticket.ProductionOperator}";
            if (Ticket.HasFirstPartialDataSet)
            {
                FullShift = $"{FullShift}:{Ticket.FirstPartialDataSet!.Shift}";
                FullQuantity = $"{FullQuantity}:{Ticket.FirstPartialDataSet!.Quantity}";
                FullOperator = $"{FullOperator}:{Ticket.FirstPartialDataSet!.Operator}";
            }
            if (Ticket.HasSecondPartialDataSet)
            {
                FullShift = $"{FullShift}:{Ticket.SecondPartialDataSet!.Shift}";
                FullQuantity = $"{FullQuantity}:{Ticket.SecondPartialDataSet!.Quantity}";
                FullOperator = $"{FullOperator}:{Ticket.SecondPartialDataSet!.Operator}";
            }
            // create a List of PrintTicket fields to use as QR Code data and add the first required fields
            List<string> Data = [];
            Data.AddRange([Ticket.Process.FullName, Ticket.Part.PartNumber, Ticket.Part.PartName, FullQuantity]);
            // retrieve the Process Requirements and add Variable Fields where required
            RequiredFields RequiredFields = Ticket.Process.RequiredFields;
            if (RequiredFields.JBKNumber)
            {
                Data.Add(Ticket.VariableFields.JBKNumber.ToString()!);
            }
            if (RequiredFields.LotNumber)
            {
                Data.Add(Ticket.VariableFields.LotNumber!);
            }
            if (RequiredFields.DeburrJBKNumber)
            {
                Data.Add(Ticket.VariableFields.DeburrJBKNumber.ToString()!);
            }
            if (RequiredFields.DieNumber)
            {
                Data.Add(Ticket.VariableFields.DieNumber.ToString()!);
            }
            if (RequiredFields.HeatNumber)
            {
                Data.Add(Ticket.VariableFields.HeatNumber!);
            }
            if (RequiredFields.ModelNumber)
            {
                Data.Add(Ticket.VariableFields.ModelNumber!);
            }
            // add the remaining required fields and use the data to create a QR code
            Data.AddRange([new Timestamp(Ticket.ProductionDate).Stamp, FullShift, FullOperator]);
            try
            {
                return new QRCode(Data);
            }
            catch
            {
                throw new ArgumentException();
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
            // compile the full basket quantity
            int FullQuantity = Ticket.ProductionQuantity;
            if (Ticket.HasFirstPartialDataSet)
            {
                FullQuantity += (int)Ticket.FirstPartialDataSet!.Quantity!;
            }
            if (Ticket.HasSecondPartialDataSet)
            {
                FullQuantity += (int)Ticket.SecondPartialDataSet!.Quantity!;
            }
            // create a List of PrintTicket fields to use as Body text and add the first required fields
            List<string> Body = [];
            Body.Add($"Process: {Ticket.Process.FullName}");
            Body.Add($"Part: {Ticket.Part.PartNumber}");
            Body.Add($"Quantity: {FullQuantity}");
            // retrieve the Process Requirements and add Variable Fields where required
            RequiredFields RequiredFields = Ticket.Process.RequiredFields;
            if (RequiredFields.JBKNumber)
            {
                Body.Add($"JBK #: {Ticket.VariableFields.JBKNumber}");
            }
            if (RequiredFields.LotNumber)
            {
                Body.Add($"Lot #: {Ticket.VariableFields.LotNumber}");
            }
            if (RequiredFields.DeburrJBKNumber)
            {
                Body.Add($"Deburr JBK #: {Ticket.VariableFields.DeburrJBKNumber}");
            }
            if (RequiredFields.DieNumber)
            {
                Body.Add($"Die #: {Ticket.VariableFields.DieNumber}");
            }
            if (RequiredFields.HeatNumber)
            {
                Body.Add($"Heat #: {Ticket.VariableFields.HeatNumber}");
            }
            if (RequiredFields.ModelNumber)
            {
                Body.Add($"Model #: {Ticket.VariableFields.ModelNumber}");
            }
            // add the remaining required fields and return the body text
            Body.Add($"Date: {new Timestamp(Ticket.ProductionDate).Stamp}");
            Body.Add($"Shift: {Ticket.ProductionShift}");
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
        await Label.AddPartNameAsync(Ticket.Part.PartName);
        await Label.AddQRCodeAsync(Code);
        await Label.AddBodyTextAsync(Body);
        // return the Label image
        return Label;
    }
}