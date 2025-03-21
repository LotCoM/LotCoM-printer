using LotCoMPrinter.Models.Datasources;

namespace LotCoMPrinter.Models.Validators;

/// <summary>
/// Pulls and stores the current state of each Interface control element, "capturing" this unique Interface state.
/// </summary>
/// <param name="ProcessPicker"></param>
/// <param name="PartPicker"></param>
/// <param name="QuantityEntry"></param>
/// <param name="JBKNumberEntry"></param>
/// <param name="LotNumberEntry"></param>
/// <param name="DeburrJBKNumberEntry"></param>
/// <param name="DieNumberEntry"></param>
/// <param name="ModelNumberEntry"></param>
/// <param name="BasketTypePicker"></param>
/// <param name="ProductionDatePicker"></param>
/// <param name="ProductionShiftPicker"></param>
/// <param name="OperatorIDEntry"></param>
/// <returns></returns>
public class InterfaceCapture(Picker ProcessPicker, Picker PartPicker, Entry QuantityEntry, Entry JBKNumberEntry, Entry LotNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry, Entry ModelNumberEntry, Picker BasketTypePicker, DatePicker ProductionDatePicker, Picker ProductionShiftPicker, Entry OperatorIDEntry) {
    /// <summary>
    /// The Process object selected in the ProcessPicker control at the time of this capture.
    /// </summary>
    /// // capture the values stored in all of the UI control elements
    public Process SelectedProcess = (Process?)ProcessPicker.ItemsSource[ProcessPicker.SelectedIndex]!;
    /// <summary>
    /// The Part object selected in the PartPicker control at the time of this capture.
    /// </summary>
    public Part SelectedPart = (Part?)PartPicker.ItemsSource[PartPicker.SelectedIndex]!;
    /// <summary>
    /// The Quantity value entered in the QuantityEntry control at the time of this capture.
    /// </summary>
    public string Quantity = QuantityEntry.Text;
    /// <summary>
    /// The JBK Number value entered in the JBKNumberEntry control at the time of this capture.
    /// </summary>
    public string JBKNumber = JBKNumberEntry.Text;
    /// <summary>
    /// The Lot Number value entered in the LotNumberEntry control at the time of this capture.
    /// </summary>
    public string LotNumber = LotNumberEntry.Text;
    /// <summary>
    /// The Deburr JBK Number value entered in the DeburrJBKNumberEntry control at the time of this capture.
    /// </summary>
    public string DeburrJBKNumber = DeburrJBKNumberEntry.Text;
    /// <summary>
    /// The Die Number value entered in the DieNumberEntry control at the time of this capture.
    /// </summary>
    public string DieNumber = DieNumberEntry.Text;
    /// <summary>
    /// The Model Number value entered in the ModelNumberEntry control at the time of this capture.
    /// </summary>
    public string ModelNumber = ModelNumberEntry.Text;
    /// <summary>
    /// The Basket Type value selected in the BasketTypePicker control at the time of this capture.
    /// </summary>
    public string BasketType = (string?)BasketTypePicker.ItemsSource[BasketTypePicker.SelectedIndex]!;
    /// <summary>
    /// The DateTime object selected in the ProductionDatePicker control at the time of this capture.
    /// </summary>
    public DateTime ProductionDate = ProductionDatePicker.Date;
    /// <summary>
    /// The Shift Number value selected in the ProductionShiftPicker control at the time of this capture.
    /// </summary>
    public string ProductionShift = (string?)ProductionShiftPicker.ItemsSource[ProductionShiftPicker.SelectedIndex]!;
    /// <summary>
    /// The Operator Initial value entered in the OperatorIDEntry control at the time of this capture.
    /// </summary>
    public string OperatorID = OperatorIDEntry.Text;

    /// <summary>
    /// Formats the InterfaceCapture's properties as a QR Code data List. 
    /// </summary>
    /// <returns></returns>
    public List<string> FormatAsQRCodeData() {
        // create a List of Capture fields to use as QR Code data
        List<string> QRCodeData = [];
        // if the Capture is for a Partial Label, add the PARTIAL flag first
        if (BasketType.Equals("Partial")) {
            QRCodeData.Add("PARTIAL");
        }
        // always add Process, Part Number/Name, Quantity
        QRCodeData.Add(SelectedProcess.FullName);
        QRCodeData.Add(SelectedPart.PartNumber);
        QRCodeData.Add(SelectedPart.PartName);
        // retrieve the Process Requirements
        List<string> RequiredFields = SelectedProcess.RequiredFields;
        // add inner (variable) Capture data
        List<string> InnerData = [JBKNumber, LotNumber, DeburrJBKNumber, DieNumber, ModelNumber];
        foreach (string _data in InnerData) {
            // only add if the field has a value and is in the Process Requirements
            if (_data != "" && RequiredFields.Contains(nameof(_data))) {
                QRCodeData.Add(_data);
            }
        }
        // always add Production Date/Shift and Initials
        QRCodeData.Add(new Timestamp(ProductionDate).Stamp);
        QRCodeData.Add(ProductionShift);
        // return the QR Code data
        return QRCodeData;
    } 

    /// <summary>
    /// Formats the InterfaceCapture's properties as a Label Body data List. 
    /// </summary>
    /// <returns></returns>
    public List<string> FormatAsLabelBodyText() {
        // create a List of Capture fields to include in a Label's body text
        List<string> LabelBodyData = [];
        // format Label body based on Label type
        bool IsPartial = BasketType.Equals("Partial");
        // add universal label fields (front)
        LabelBodyData.Add($"Process: {SelectedProcess.FullName}");
        LabelBodyData.Add($"Part #: {SelectedPart.PartNumber}");
        LabelBodyData.Add($"Quantity: {Quantity}");
        // add inner (variable) Capture data if Label is full
        if (!IsPartial) {
            // retrieve the Process Requirements
            List<string> RequiredFields = SelectedProcess.RequiredFields;
            // add inner (variable) Capture data
            if (JBKNumber != "" && RequiredFields.Contains("JBKNumber")) {
                LabelBodyData.Add($"JBK #: {JBKNumber}");
            }
            if (LotNumber != "" && RequiredFields.Contains("LotNumber")) {
                LabelBodyData.Add($"Lot #: {LotNumber}");
            }
            if (DeburrJBKNumber != "" && RequiredFields.Contains("DeburrJBKNumber")) {
                LabelBodyData.Add($"Deburr JBK #: {DeburrJBKNumber}");
            }
            if (DieNumber != "" && RequiredFields.Contains("DieNumber")) {
                LabelBodyData.Add($"Die #: {DieNumber}");
            }
            if (ModelNumber != "" && RequiredFields.Contains("ModelNumber")) {
                LabelBodyData.Add($"Model #: {ModelNumber}");
            }
        }
        // add universal label fields (back)
        Console.WriteLine($"Adding Date {new Timestamp(ProductionDate).Stamp}.");
        LabelBodyData.Add($"Prod. Date: {new Timestamp(ProductionDate).Stamp}");
        Console.WriteLine($"Adding Shift {ProductionShift}.");
        LabelBodyData.Add($"Prod. Shift: {ProductionShift}");
        // return the Label body fields
        return LabelBodyData;
    }
}