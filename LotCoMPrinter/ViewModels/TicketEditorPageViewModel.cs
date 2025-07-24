using CommunityToolkit.Mvvm.ComponentModel;
using LotCom.Enums;
using LotCom.Types;
using LotComPrinter.Models.Datatypes;
using LotComPrinter.Models.Enums;
using LotComPrinter.Models.Exceptions;
using LotComPrinter.Views;

namespace LotComPrinter.ViewModels;

/// <summary>
/// ViewModel providing binding context for the Ticket Editor Page.
/// </summary>
public class TicketEditorPageViewModel : ObservableObject
{
    /// <summary>
    /// The MainPage instance that precedes the Editor in the Navigation Stack.
    /// </summary>
    private readonly MainPage Presenter;

    private TrackedPrintTicket? _editorTicket = null;
    /// <summary>
    /// The PrintTicket currently being edited, with Tracked changes.
    /// </summary>
    public TrackedPrintTicket? EditorTicket
    {
        get { return _editorTicket; }
        set
        {
            _editorTicket = value;
            OnPropertyChanged(nameof(_editorTicket));
            OnPropertyChanged(nameof(EditorTicket));
        }
    }

    /// <summary>
    /// Creates a new ViewModel to bind to a TicketEditorPage.
    /// </summary>
    public TicketEditorPageViewModel(PrintTicket EditorTicket, MainPage Presenter)
    {
        this.Presenter = Presenter;
        this.EditorTicket = new TrackedPrintTicket(EditorTicket);
    }

    /// <summary>
    /// Attempts to create and run a Label Print Job from EditorTicket.Tracked.
    /// Removes EditorTicket from OpenPrintTickets.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="PrintRequestException"></exception>
    public async Task<bool> Print()
    {
        // create a LabelPrintJob from the editing Print Ticket
        if (EditorTicket is null)
        {
            throw new NullReferenceException("Cannot print 'null' PrintTicket.");
        }
        // validate the PrintTicket
        try
        {
            EditorTicket.Tracked = EditorTicket.Tracked.SelfValidate();
        }
        catch (ArgumentException)
        {
            throw;
        }
        PrintJob Job = new PrintJob(EditorTicket.Tracked, PrintJobType.Full);
        bool Printed;
        // attempt to run the Print Job
        try
        {
            Printed = await Job.Run();
        }
        catch (LabelBuildException)
        {
            throw new PrintRequestException("Could not create a Label from the entered information.");
        }
        catch (PrintRequestException)
        {
            throw new PrintRequestException("Failed to execute the print job for the generated Label.");
        }
        // remove the Ticket if the print was successful
        if (Printed)
        {
            bool Removed = Presenter.ViewModel.OpenPrintTickets.Remove(EditorTicket.Untracked);
            // confirm that the EditorTicket exists in OpenPrintTickets and remove it
            if (!Removed)
            {
                throw new NullReferenceException("The Untracked EditorTicket was not found in OpenPrintTickets.");
            }
            await Presenter.ViewModel.SaveOpenPrintTickets();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Attempts to add a blank PartialDataSet object to the editing Print Ticket.
    /// </summary>
    public void AddPartialDataSet()
    {
        if (EditorTicket is not null && EditorTicket.Tracked.HasSpace)
        {
            // create and add a blank PartialDataSet object to the ticket
            EditorTicket.Tracked.AddPartialDataSet
            (
                new PartialDataSet
                (
                    new Quantity(0),
                    Shift.None,
                    new Operator("ABC")
                )
            );
        }
    }

    /// <summary>
    /// Attempts to create and run a Partial Tag Print Job from EditorTicket.Tracked.
    /// </summary>
    /// <param name="PartialSetNumber">The PartialDataSet to use as the source of partial Production Data, either 1 or 2.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="PrintRequestException"></exception>
    public async Task<bool> PrintPartialTag(int PartialSetNumber)
    {
        // ensure the targeted PartialDataSet exists on the editing Print Ticket
        if (PartialSetNumber < 1 || PartialSetNumber > 2)
        {
            throw new ArgumentException
            (
                $"Cannot print PartialDataSet '{PartialSetNumber}' as it is outside the allowed set (1, 2).",
                nameof(PartialSetNumber)
            );
        }
        if (EditorTicket is null)
        {
            throw new NullReferenceException("Cannot print PartialDataSets from 'null' PrintTicket.");
        }
        // validate the target PartialDataSet
        try
        {
            if (PartialSetNumber == 1 && EditorTicket.Tracked.HasFirstPartialDataSet)
            {
                EditorTicket.Tracked.FirstPartialDataSet!.SelfValidate();
            }
            else if (PartialSetNumber == 2 && EditorTicket.Tracked.HasSecondPartialDataSet)
            {
                EditorTicket.Tracked.SecondPartialDataSet!.SelfValidate();
            }
            else
            {
                throw new NullReferenceException($"Cannot print 'null' PartialDataSet Number '{PartialSetNumber}'.");
            }
        }
        catch (NullReferenceException _nullEx)
        {
            throw new NullReferenceException(_nullEx.Message);
        }
        catch (ArgumentException _argEx)
        {
            throw new ArgumentException(_argEx.Message);
        }
        // create a new PrintJob from the PartialDataSet and attempt to run it
        PrintJob Job = new PrintJob(EditorTicket.Tracked, PrintJobType.Partial, PartialSetNumber);
        bool Printed;
        try
        {
            Printed = await Job.Run();
        }
        catch (LabelBuildException)
        {
            throw new PrintRequestException("Could not create a Label from the entered information.");
        }
        catch (PrintRequestException)
        {
            throw new PrintRequestException("Failed to execute the print job for the generated Label.");
        }
        return Printed;
    }
}