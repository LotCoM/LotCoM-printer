namespace LotCoMPrinter.Models.Printing;

/// <summary>
/// Class providing Display and DPI utility methods.
/// </summary>
public static class DpiUtilities {

    /// <summary>
    /// Retrieves the Dpi density (scale) of the current Device Display.
    /// </summary>
    /// <returns>A double value indicating the scale. This is the percentage / 100 (i.e. 250% scale == 2.5).</returns>
    public static double GetDeviceDpiScale() {
        // retrieve the scale for the current device
        double Scale = DeviceDisplay.Current.MainDisplayInfo.Density;
        return Scale;
    }

}