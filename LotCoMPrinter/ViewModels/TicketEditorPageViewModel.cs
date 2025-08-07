using CommunityToolkit.Mvvm.ComponentModel;
using LotCom.Enums;
using LotCom.Types;
using LotComPrinter.Models.Datatypes;
using LotComPrinter.Models.Enums;
using LotComPrinter.Models.Exceptions;
using LotComPrinter.Models.Services;
using LotComPrinter.Views;

namespace LotComPrinter.ViewModels;

/// <summary>
/// ViewModel providing binding context for the Ticket Editor Page.
/// </summary>
public class TicketEditorPageViewModel : ObservableObject
{
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
    public TicketEditorPageViewModel(PrintTicket EditorTicket)
    {
        DebugLogger.LogMessage("Starting TicketEditorPageViewModel initialization...", this);
        DebugLogger.LogMessage
        (
            "Source for TicketEditorPage: \"" +
            $"{EditorTicket.ToJSON()}"
            + "\"",
            this
        );
        DebugLogger.LogMessage("Creating new TrackedPrintTicket to use for change tracking...", this);
        try
        {
            this.EditorTicket = new TrackedPrintTicket(EditorTicket);
        }
        catch (Exception _ex)
        {
			DebugLogger.LogError("Failed to create a TrackedPrintTicket.", _ex, this);
			throw;
        }
		DebugLogger.LogMessage("ViewModel created.", this);
		DebugLogger.LogMessage("BindingContext setup complete.", this);
    }

    /// <summary>
    /// Saves all tracked changes to the OpenPrintTickets list.
    /// </summary>
    /// <param name="Previous"></param>
    /// <returns></returns>
    /// <exception cref="SystemException"></exception>
    public async Task SaveTickets(Page Previous)
    {
        // confirm that the passed Page is a MainPage
        if (!Previous.GetType().Equals(typeof(MainPage)))
        {
            throw new SystemException("Cannot navigate to previous MainPage view to save Print Tickets.");
        }
        // convert the Page to MainPage, remove the Ticket, and save the open ticket list
        MainPage Main = (MainPage)Previous;
        try
        {
            await Main.ViewModel.SaveOpenPrintTickets();
        }
        catch (SystemException)
        {
            throw;
        }
    }

    /// <summary>
    /// Goes through the navigation stack to remove the Editor's PrintTicket from the Open Ticket List.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SystemException"></exception>
    public async Task<bool> DeleteTicket(Page Previous)
    {
        // confirm that the passed Page is a MainPage
        if (!Previous.GetType().Equals(typeof(MainPage)))
        {
            throw new SystemException("Cannot navigate to previous MainPage view to remove this Print Ticket.");
        }
        // convert the Page to MainPage, remove the Ticket, and save the open ticket list
        MainPage Main = (MainPage)Previous;
        bool Result = await Main.ViewModel.RemoveOpenPrintTicket(EditorTicket!.Untracked);
        if (!Result)
        {
            return false;
        }
        try
        {
            await SaveTickets(Main);
        }
        catch (SystemException)
        {
            throw;
        }
        return true;
    }

    /// <summary>
    /// Goes through the NavigationStack to add the Editor's Untracked ticket to the Open Ticket List.
    /// </summary>
    /// <param name="Previous"></param>
    /// <returns></returns>
    /// <exception cref="SystemException"></exception>
    public async Task AddTicket(Page Previous)
    {
        // confirm that the passed Page is a MainPage
        if (!Previous.GetType().Equals(typeof(MainPage)))
        {
            throw new SystemException("Cannot navigate to previous MainPage view to add this Print Ticket.");
        }
        // convert the Page to MainPage, add the Ticket, and save the open ticket list
        MainPage Main = (MainPage)Previous;
        try
        {
            await Main.ViewModel.AddNewOpenPrintTicket(EditorTicket!.Untracked);
        }
        catch (SystemException)
        {
            throw;
        }
    }

    /// <summary>
    /// Attempts to create and run a Label Print Job from EditorTicket.Tracked.
    /// Removes EditorTicket from OpenPrintTickets.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="PrintRequestException"></exception>
    /// <exception cref="SystemException"></exception>
    public async Task<bool> PrintTicket(Page Previous)
    {
        // confirm that the passed Page is a MainPage
        if (!Previous.GetType().Equals(typeof(MainPage)))
        {
            throw new SystemException("Cannot navigate to previous MainPage view to print this Print Ticket.");
        }
        // convert the Page to MainPage
        MainPage Main = (MainPage)Previous;
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
        // remove the Ticket from the OpenPrintTicket list
        try
        {
            await DeleteTicket(Main);
        }
        catch (SystemException)
        {
            throw new NullReferenceException("The Untracked EditorTicket was not found in OpenPrintTickets.");
        }
        return Printed;
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