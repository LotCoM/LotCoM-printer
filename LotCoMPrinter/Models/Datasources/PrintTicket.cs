using CommunityToolkit.Mvvm.ComponentModel;
using LotCoMPrinter.Models.Options;

namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a structure for the creation and maintenance of a Printing Ticket.
/// </summary>
public partial class PrintTicket(Department Department, Process Process, Part Part, string SerialNumber, Timestamp ProductionDate) : ObservableObject()
{
    /// <summary>
    /// The Department that initiated this Print Ticket.
    /// </summary>
    private Department Department = Department;

    /// <summary>
    /// The Process that initiated this Print Ticket.
    /// </summary>
    private Process Process = Process;

    /// <summary>
    /// The Part that this Print Ticket is applied to.
    /// </summary>
    private Part Part = Part;

    /// <summary>
    /// The Serial Number (JBK or Lot Number) applied to this Print Ticket.
    /// </summary>
    private string SerialNumber = SerialNumber;

    /// <summary>
    /// The Date and Time at which this Print Ticket was initiated.
    /// </summary>
    private Timestamp ProductionDate = ProductionDate;

    /// <summary>
    /// The first of the Partial Production Data sets associated with this Print Ticket.
    /// </summary>
    private PartialDataSet? FirstPartialDataSet;

    /// <summary>
    /// The second of the Partial Production Data sets associated with this Print Ticket.
    /// </summary>
    private PartialDataSet? SecondPartialDataSet;

    /// <summary>
    /// Provides a Title for the Print Ticket that gives the crucial information of the Ticket.
    /// </summary>
    public string Title
    {
        get
        {
            return $"{Part.ModelNumber} {Part.PartName}: {SerialNumber}";
        }
    }
}