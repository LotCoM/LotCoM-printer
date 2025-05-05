using LotCoMPrinter.Models.Options;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Pulls and stores the current state of each Interface control element, "capturing" this unique Interface state.
/// </summary>
/// <param name="Process"></param>
/// <param name="Part"></param>
/// <param name="Quantity"></param>
/// <param name="JBKNumber"></param>
/// <param name="LotNumber"></param>
/// <param name="DeburrJBKNumber"></param>
/// <param name="DieNumber"></param>
/// <param name="HeatNumber"></param>
/// <param name="ModelNumber"></param>
/// <param name="ProductionDate"></param>
/// <param name="ProductionShift"></param>
/// <param name="OperatorID"></param>
/// <returns></returns>
public class InterfaceCapture(Process Process, Part Part, int Quantity, int JBKNumber, string LotNumber, int DeburrJBKNumber, int DieNumber, string ModelNumber, string HeatNumber, DateTime ProductionDate, int ProductionShift, string OperatorID) 
{
    /// <summary>
    /// The Process object selected in the ProcessPicker control at the time of this capture.
    /// </summary>
    public Process Process = Process;

    /// <summary>
    /// The Part object selected in the PartPicker control at the time of this capture.
    /// </summary>
    public Part Part = Part;

    /// <summary>
    /// The Quantity value entered in the QuantityEntry control at the time of this capture.
    /// </summary>
    public int Quantity = Quantity;

    /// <summary>
    /// The Variable Field values entered in the Variable Field controls at the time of this capture.
    /// </summary>
    public VariableFieldSet VariableFields = new VariableFieldSet
    (
        JBKNumber,
        LotNumber,
        DeburrJBKNumber,
        DieNumber,
        ModelNumber,
        HeatNumber
    );

    /// <summary>
    /// The DateTime object selected in the ProductionDatePicker control at the time of this capture.
    /// </summary>
    public DateTime ProductionDate = ProductionDate;

    /// <summary>
    /// The Shift Number value selected in the ProductionShiftPicker control at the time of this capture.
    /// </summary>
    public int ProductionShift = ProductionShift;

    /// <summary>
    /// The Operator Initial value entered in the OperatorIDEntry control at the time of this capture.
    /// </summary>
    public string OperatorID = OperatorID;

    /// <summary>
    /// Formats the InterfaceCapture's properties as a QR Code data List. 
    /// </summary>
    /// <returns></returns>
    public List<string> FormatAsQRCodeData() 
    {
        // create a List of Capture fields to use as QR Code data
        List<string> QRCodeData = [];
        // always add Process, Part Number/Name, Quantity
        QRCodeData.AddRange([Process.FullName, Part.PartNumber, Part.PartName, Quantity.ToString()]);
        // retrieve the Process Requirements
        List<string> RequiredFields = Process.RequiredFields;
        // add inner (variable) Capture data
        if (RequiredFields.Contains("JBKNumber")) 
        {
            QRCodeData.Add(VariableFields.JBKNumber.ToString()!);
        }
        if (RequiredFields.Contains("LotNumber")) 
        {
            QRCodeData.Add(VariableFields.LotNumber!);
        }
        if (RequiredFields.Contains("DeburrJBKNumber")) 
        {
            QRCodeData.Add(VariableFields.DeburrJBKNumber.ToString()!);
        }
        if (RequiredFields.Contains("DieNumber")) 
        {
            QRCodeData.Add(VariableFields.DieNumber.ToString()!);
        }
        if (RequiredFields.Contains("HeatNumber")) 
        {
            QRCodeData.Add(VariableFields.HeatNumber!);
        }
        if (RequiredFields.Contains("ModelNumber")) 
        {
            QRCodeData.Add(VariableFields.ModelNumber!);
        }
        // always add Production Date/Shift and Initials
        QRCodeData.AddRange([new Timestamp(ProductionDate).Stamp, ProductionShift.ToString(), OperatorID]);
        // return the QR Code data
        return QRCodeData;
    } 

    /// <summary>
    /// Formats the InterfaceCapture's properties as a Label Body data List. 
    /// </summary>
    /// <returns></returns>
    public List<string> FormatAsLabelBodyText() 
    {
        // create a List of Capture fields to include in a Label's body text
        List<string> LabelBodyData = [];
        // add universal label fields (front)
        LabelBodyData.AddRange(
        [
            $"Process: {Process.FullName}",
            $"Part #: {Part.PartNumber}",
            $"Quantity: {Quantity}"
        ]);
        // retrieve the Process' requirements
        List<string> Requirements = Process.RequiredFields;
        // add inner (variable) Capture data
        if (Requirements.Contains("JBKNumber")) 
        {
            LabelBodyData.Add(VariableFields.JBKNumber.ToString()!);
        }
        if (Requirements.Contains("LotNumber")) 
        {
            LabelBodyData.Add(VariableFields.LotNumber!);
        }
        if (Requirements.Contains("DeburrJBKNumber")) 
        {
            LabelBodyData.Add(VariableFields.DeburrJBKNumber.ToString()!);
        }
        if (Requirements.Contains("DieNumber")) 
        {
            LabelBodyData.Add(VariableFields.DieNumber.ToString()!);
        }
        if (Requirements.Contains("HeatNumber")) 
        {
            LabelBodyData.Add(VariableFields.HeatNumber!);
        }
        if (Requirements.Contains("ModelNumber")) 
        {
            LabelBodyData.Add(VariableFields.ModelNumber!);
        }
        // add universal label fields (back)
        LabelBodyData.Add($"Prod. Date: {new Timestamp(ProductionDate).Stamp}");
        LabelBodyData.Add($"Prod. Shift: {ProductionShift}");
        // return the Label body fields
        return LabelBodyData;
    }

    /// <summary>
    /// Formats the InterfaceCapture as a comma-separated value (CSV line). The Line includes all required data fields.
    /// </summary>
    /// <returns></returns>
    public string FormatAsCSV() 
    {
        // retrieve the Process' requirements
        List<string> Requirements = Process.RequiredFields;
        // add universal requirements (front)
        string CSVLine = $"{Process.FullName},{Part.PartNumber},{Part.PartName},{Quantity}";
        // add internal, variable fields
        if (Requirements.Contains("JBKNumber")) 
        {
            CSVLine = $"{CSVLine},{VariableFields.JBKNumber}";
        }
        if (Requirements.Contains("LotNumber")) 
        {
            CSVLine = $"{CSVLine},{VariableFields.LotNumber}";
        }
        if (Requirements.Contains("DeburrJBKNumber")) 
        {
            CSVLine = $"{CSVLine},{VariableFields.DeburrJBKNumber}";
        }
        if (Requirements.Contains("DieNumber")) 
        {
            CSVLine = $"{CSVLine},{VariableFields.DieNumber}";
        }
        if (Requirements.Contains("HeatNumber")) 
        {
            CSVLine = $"{CSVLine},{VariableFields.HeatNumber}";
        }
        if (Requirements.Contains("ModelNumber")) 
        {
            CSVLine = $"{CSVLine},{VariableFields.ModelNumber}";
        }
        // add universal requirements (back)
        CSVLine = $"{CSVLine},{new Timestamp(ProductionDate).Stamp},{ProductionShift},{OperatorID}";
        return CSVLine;
    }
}