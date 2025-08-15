using LotCom.DataAccess.Services;
using LotCom.Types;
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
        PrintTicket SourceTicket = Job.Source;
        Print Model = SourceTicket.ToPrint();
        // add the Print Model to the Database
        try
        {
            return await PrintService.Create(Model, App.UserAgent);
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    /// <summary>
    /// Attempts to update ExistingSource's Database entry with NewSource's information.
    /// </summary>
    /// <param name="ExistingSource"></param>
    /// <param name="NewSource"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="SystemException"></exception>
    public static async Task<bool> UpdateLog(PrintTicket ExistingSource, PrintTicket NewSource)
    {
        // convert the sources to Print Models
        Print ExistingModel = ExistingSource.ToPrint();
        Print NewModel = NewSource.ToPrint();
        // update the Print Model in the Database
        try
        {
            return await PrintService.Update(ExistingModel.Id, NewModel, App.UserAgent);
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}