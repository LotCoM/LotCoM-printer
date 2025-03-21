using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using LotCoMPrinter.Models.Printing;
using LotCoMPrinter.Models.Services;

namespace LotCoMPrinter.Models.Labels;

# pragma warning disable CA1416 // Validate platform compatibility

public class PartialLabel {
    // label dimension constants; // value = default
    // dimension (square dimension) of label
    private int LabelDimension = 1440; // 1440
    // dimension (square dimension) of QR Code on label
    private int CodeDimension = 600; // 600
    // size of small text on label
    private int TextSizeSmall = 42; // 42
    // size of medium text on label
    private int TextSizeMedium = 76; // 76
    // size of large text on label
    private int TextSizeLarge = 160; // 192
    // padding of objects on the label
    private int LabelInternalPadding = 36; // 12
    // horizontal position of the label heading text
    private int LabelHeadingX;
    // vertical position of the label heading text
    private int LabelHeadingY;
    // horizontal position of the label part name text
    private int LabelPartNameX;
    // vertical position of the label part name text
    private int LabelPartNameY;
    // left X coordinate of Code
    private int CodePositionX1;
    // top Y coordinate of Code
    private int CodePositionY1;
    // horizontal position of the Label's information fields
    private int LabelFieldsX;
    // vertical position of the Label's information fields
    private int LabelFieldsY;

    // private class properties
    private readonly Bitmap _image;
    private readonly System.Drawing.Font _fontSmall;
    private readonly System.Drawing.Font _fontMedium;
    private readonly System.Drawing.Font _fontLarge;
    
    /// <summary>
    /// Creates a new Image of a PARTIAL Production Lot Tracing Label that can be sent to Print Spooling.
    /// </summary>
    /// <exception cref="SystemException"></exception>
    public PartialLabel() {
        // scale the Label dimensions to the current Device's Dpi Scale
        ConfigureLabelDimensions();

        // load a new Label base
        _image = LoadBase();
        
        // load Label design fonts (try to find the Arial Font in the system)
        FontFamily? Arial;
        try {
            Arial = new FontFamily("Arial");
        } catch {
            throw new SystemException("Could not find Arial Font Group in the System.");
        }
        _fontSmall = new System.Drawing.Font(Arial!, TextSizeSmall);
        _fontMedium = new System.Drawing.Font(Arial!, TextSizeMedium);
        _fontLarge = new System.Drawing.Font(Arial!, TextSizeLarge);
    }

    /// <summary>
    /// Converts the Label's internal dimension values to match the DPI scaling of the Device.
    /// Multiplies the default values by the set DPI scale.
    /// </summary>
    private void ConfigureLabelDimensions() {
        // retreive the DPI scale for the device's display
        double DpiScale = DpiUtilities.GetDeviceDpiScale();

        // scale the Label's dimensions to the DPI
        LabelDimension = Convert.ToInt32(LabelDimension * DpiScale);
        CodeDimension = Convert.ToInt32(CodeDimension * DpiScale);
        TextSizeSmall = Convert.ToInt32(TextSizeSmall * DpiScale);
        TextSizeMedium = Convert.ToInt32(TextSizeMedium * DpiScale);
        TextSizeLarge = Convert.ToInt32(TextSizeLarge * DpiScale);
        LabelInternalPadding = Convert.ToInt32(LabelInternalPadding * DpiScale);
        LabelHeadingX = Convert.ToInt32(-56 * DpiScale + LabelInternalPadding);
        LabelHeadingY = Convert.ToInt32(-56 * DpiScale + LabelInternalPadding); 
        CodePositionX1 = LabelDimension - CodeDimension - LabelInternalPadding;
        CodePositionY1 = LabelHeadingY + TextSizeLarge + (LabelInternalPadding * 3);
        LabelPartNameX = LabelInternalPadding;
        LabelPartNameY = CodePositionY1 + CodeDimension;
        LabelFieldsX = LabelInternalPadding;
        LabelFieldsY = LabelPartNameY + (TextSizeMedium * 2) + LabelInternalPadding;
    }

    /// <summary>
    /// Creates and returns a new, pure-white bitmap image to use as a Label base.
    /// </summary>
    /// <returns></returns>
    private static Bitmap LoadBase() {
        // create a new white Label Base
        Bitmap Base = new Bitmap(1800, 1800, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        // create a new Drawing Surface and draw all white to the base
        using (Graphics Surface = Graphics.FromImage(Base)) {
            Rectangle ImageSize = new(0, 0, 1800, 1800);
            Surface.FillRectangle(Brushes.White, ImageSize);
        }
        return Base;
    }

    /// <summary>
    /// Access the current Bitmap image assigned to the PartialLabel object.
    /// </summary>
    /// <returns></returns>
    public Bitmap GetImage() {
        return _image;
    }

    /// <summary>
    /// Writes Header text to the top-left corner of the Label.
    /// </summary>
    /// <param name="HeadingText">The text to write.</param>
    /// <returns></returns>
    public async Task AddHeaderAsync(string HeadingText) {
        // start a new CPU thread to apply the header to the LabelBase
        await Task.Run(() => {
            // create a drawing surface to draw the text with
            Graphics Surface = Graphics.FromImage(_image);
            // set the quality properties of the Surface
            Surface.SmoothingMode = SmoothingMode.AntiAlias;
            Surface.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Surface.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Surface.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            // draw the Heading text
            Surface.DrawString($"PARTIAL\n{HeadingText}", _fontLarge, Brushes.Black, LabelHeadingX, LabelHeadingY);
            Surface.Flush();
        });
    }

    /// <summary>
    /// Writes PartName text below the Label Header.
    /// </summary>
    /// <param name="PartName"></param>
    /// <returns></returns>
    public async Task AddPartNameAsync(string PartName) {
        // start a new CPU thread to apply the part name to the LabelBase
        await Task.Run(() => {
            // create a drawing surface to draw the text with
            Graphics Surface = Graphics.FromImage(_image);
            // set the quality properties of the Surface
            Surface.SmoothingMode = SmoothingMode.AntiAlias;
            Surface.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Surface.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Surface.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            // draw the Part Name text
            Surface.DrawString(PartName, _fontMedium, Brushes.Black, LabelPartNameX, LabelPartNameY);
            Surface.Flush();
        });
    }

    /// <summary>
    /// Adds a QR Code, as an image, to the top-right corner of the Label.
    /// </summary>
    /// <param name="LabelCode"></param>
    /// <returns></returns>
    public async Task AddQRCodeAsync(QRCode LabelCode) {
        // start a new CPU thread to apply the QR code to the LabelBase
        await Task.Run(() => {
            // create a drawing surface to draw the text with
            Graphics Surface = Graphics.FromImage(_image);
            // set the quality properties of the Surface
            Surface.CompositingMode = CompositingMode.SourceOver;
            Surface.CompositingQuality = CompositingQuality.HighQuality;
            Surface.SmoothingMode = SmoothingMode.AntiAlias;
            Surface.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Surface.PixelOffsetMode = PixelOffsetMode.HighQuality;
            // resize and draw the QR Code
            Bitmap LabelCodeImage = Resizer.ResizeImage(LabelCode.CodeImage!, CodeDimension, CodeDimension);
            Surface.DrawImage(LabelCodeImage, CodePositionX1, CodePositionY1);
            Surface.Flush();
        });
    }

    /// <summary>
    /// Adds the data fields to the mid-to-bottom-left area of the Label.
    /// </summary>
    /// <param name="LabelFields"></param>
    /// <returns></returns>
    public async Task AddLabelFieldsAsync(List<string> LabelFields) {
        // start a new CPU thread to apply the Label Fields to the LabelBase
        await Task.Run(() => {
            // combine the LabelFields into a string deliniated by newlines
            string LabelFieldsBody = "";
            foreach (string _field in LabelFields) {
                // add the field to the data body
                LabelFieldsBody += _field + "\n";
            }
            // create a drawing surface to draw the text with
            Graphics Surface = Graphics.FromImage(_image);
            // set the quality properties of the Surface
            Surface.SmoothingMode = SmoothingMode.AntiAlias;
            Surface.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Surface.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Surface.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            // draw the Heading text
            Surface.DrawString(LabelFieldsBody, _fontSmall, Brushes.Black, LabelFieldsX, LabelFieldsY);
            Surface.Flush();
        });
    }
}
# pragma warning restore CA1416 // Validate platform compatibility