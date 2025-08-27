using LotCom.Core.Models;
using LotCom.Database.Services;
using LotComPrinter.Models.Datatypes;

namespace LotComPrinter.Models.Services;

public static class LoggingService
{
    /// <summary>
    /// Sends a PrintJob's information to the database.
    /// </summary>
    /// <param name="Job"></param>
    /// <returns></returns>
    public static async Task<bool> LogPrintEvent(PrintJob Job)
    {
        // extract the PrintTicket from the PrintJob and convert it to a Print Model
        PrintTicket SourceTicket = Job.Source.Tracked;
        Print Model = SourceTicket.ToPrint();
        // add the Print Model to the Database
        try
        {
            return await PrintService.Create(Model, App.Http, App.UserAgent);
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    /// <summary>
    /// Attempts to update UpdatedTickets's Database entry with any changed fields.
    /// </summary>
    /// <param name="UpdatedTicket"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SystemException"></exception>
    public static async Task<bool> UpdateLog(PrintTicket UpdatedTicket)
    {
        // convert the source to a Print Model
        Print Model = UpdatedTicket.ToPrint();
        // update the Print Model in the Database
        try
        {
            return await PrintService.Update(Model.Id, Model, App.Http, App.UserAgent);
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}