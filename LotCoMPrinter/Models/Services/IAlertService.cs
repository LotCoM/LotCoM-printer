namespace LotCoMPrinter.Models.Services;

// via: https://stackoverflow.com/questions/72429055/how-to-displayalert-in-a-net-maui-viewmodel - ToolmakerSteve

public interface IAlertService {
    // ----- async calls (use with "await" - MUST BE ON DISPATCHER THREAD) -----
    // must ensure you are calling from the main thread

    /// <summary>
    /// Shows an alert (popup with a single cancellation button) asynchronously on the MAIN (DISPATCHER) Thread.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    Task ShowAlertAsync(string title, string message, string cancel = "OK");

    /// <summary>
    /// Shows a confirmation (popup with confirm/cancel buttons) asynchronously on the MAIN (DISPATCHER) Thread.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="accept"></param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No");

    // ----- "Fire and forget" calls -----
    // call from anywhere

    /// <summary>
    /// Shows an alert (popup with a single cancellation button) synchronously (Call from anywhere).
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="cancel"></param>
    void ShowAlert(string title, string message, string cancel = "OK");
    
    /// <summary>
    /// Shows a confirmation (popup with confirm/cancel buttons) synchronously (Call from anywhere).
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="callback">Action to perform afterwards.</param>
    /// <param name="accept"></param>
    /// <param name="cancel"></param>
    void ShowConfirmation(string title, string message, Action<bool> callback,
                          string accept = "Yes", string cancel = "No");
}

internal class AlertService : IAlertService
{
    // ----- async calls (use with "await" - MUST BE ON DISPATCHER THREAD) -----

    [Obsolete]
    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        return Application.Current!.MainPage!.DisplayAlert(title, message, cancel);
    }

    [Obsolete]
    public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No")
    {
        return Application.Current!.MainPage!.DisplayAlert(title, message, accept, cancel);
    }


    // ----- "Fire and forget" calls -----

    /// <summary>
    /// "Fire and forget". Method returns BEFORE showing alert.
    /// </summary>
    [Obsolete]
    public void ShowAlert(string title, string message, string cancel = "OK")
    {
        Application.Current!.MainPage!.Dispatcher.Dispatch(async () =>
            await ShowAlertAsync(title, message, cancel)
        );
    }

    /// <summary>
    /// "Fire and forget". Method returns BEFORE showing alert.
    /// </summary>
    /// <param name="callback">Action to perform afterwards.</param>
    [Obsolete]
    public void ShowConfirmation(string title, string message, Action<bool> callback,
                                 string accept="Yes", string cancel = "No")
    {
        Application.Current!.MainPage!.Dispatcher.Dispatch(async () =>
        {
            bool answer = await ShowConfirmationAsync(title, message, accept, cancel);
            callback(answer);
        });
    }
}