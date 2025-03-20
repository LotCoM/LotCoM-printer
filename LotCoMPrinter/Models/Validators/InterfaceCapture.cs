using LotCoMPrinter.Models.Datasources;

namespace LotCoMPrinter.Models.Validators;

/// <summary>
/// Holds a "capture" of the User Interface. 
/// Contains the states and values of all Interface controls at the time of the capture.
/// </summary>
public class InterfaceCapture {
    /// <summary>
    /// The Process object selected in the ProcessPicker control at the time of this capture.
    /// </summary>
    public Process? SelectedProcess;
    /// <summary>
    /// The Part object selected in the PartPicker control at the time of this capture.
    /// </summary>
    public Part? SelectedPart;
    /// <summary>
    /// The Quantity value entered in the QuantityEntry control at the time of this capture.
    /// </summary>
    public string? Quantity;
    /// <summary>
    /// The JBK Number value entered in the JBKNumberEntry control at the time of this capture.
    /// </summary>
    public string? JBKNumber;
    /// <summary>
    /// The Lot Number value entered in the LotNumberEntry control at the time of this capture.
    /// </summary>
    public string? LotNumber;
    /// <summary>
    /// The Deburr JBK Number value entered in the DeburrJBKNumberEntry control at the time of this capture.
    /// </summary>
    public string? DeburrJBKNumber;
    /// <summary>
    /// The Die Number value entered in the DieNumberEntry control at the time of this capture.
    /// </summary>
    public string? DieNumber;
    /// <summary>
    /// The Model Number value entered in the ModelNumberEntry control at the time of this capture.
    /// </summary>
    public string? ModelNumber;
    /// <summary>
    /// The Basket Type value selected in the BasketTypePicker control at the time of this capture.
    /// </summary>
    public string? BasketType;
    /// <summary>
    /// The DateTime object selected in the ProductionDatePicker control at the time of this capture.
    /// </summary>
    public DateTime? ProductionDate;
    /// <summary>
    /// The Shift Number value selected in the ProductionShiftPicker control at the time of this capture.
    /// </summary>
    public string? ProductionShift;
    /// <summary>
    /// The Operator Initial value entered in the OperatorIDEntry control at the time of this capture.
    /// </summary>
    public string? OperatorID;

    /// <summary>
    /// Prepares a new InterfaceCapture object. Call this.Capture() method to populate the object.
    /// </summary>
    public InterfaceCapture() {}

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
    public async Task Capture(Picker ProcessPicker, Picker PartPicker, Entry QuantityEntry, Entry JBKNumberEntry, Entry LotNumberEntry, Entry DeburrJBKNumberEntry, Entry DieNumberEntry, Entry ModelNumberEntry, Picker BasketTypePicker, DatePicker ProductionDatePicker, Picker ProductionShiftPicker, Entry OperatorIDEntry) {
        // use multi-threading to avoid blocking the UI thread
        await Task.Run(() => {
            // capture the values stored in all of the UI control elements
            SelectedProcess = (Process?)ProcessPicker.ItemsSource[ProcessPicker.SelectedIndex];
            SelectedPart = (Part?)PartPicker.ItemsSource[PartPicker.SelectedIndex];
            Quantity = QuantityEntry.Text;
            JBKNumber = JBKNumberEntry.Text;
            LotNumber = LotNumberEntry.Text;
            DeburrJBKNumber = DeburrJBKNumberEntry.Text;
            DieNumber = DieNumberEntry.Text;
            ModelNumber = ModelNumberEntry.Text;
            BasketType = (string?)BasketTypePicker.ItemsSource[BasketTypePicker.SelectedIndex];
            ProductionDate = (DateTime?)ProductionDatePicker.Date;
            ProductionShift = (string?)ProductionShiftPicker.ItemsSource[ProductionShiftPicker.SelectedIndex];
            OperatorID = OperatorIDEntry.Text;
        });
    }
}