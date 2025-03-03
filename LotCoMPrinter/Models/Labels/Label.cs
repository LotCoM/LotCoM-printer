using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using LotCoMPrinter.Models.Services;

namespace LotCoMPrinter.Models.Labels;

# pragma warning disable CA1416 // Validate platform compatibility

public class Label {
    // label dimension constants; (value) = default
    // dimension (square dimension) of label (900)
    private const int LabelDimension = 1800;
    // dimension (square dimension) of QR Code on label (375)
    private const int CodeDimension = 750;
    // size of small text on label (36)
    private const int TextSizeSmall = 54;
    // size of medium text on label
    private const int TextSizeMedium = 96;
    // size of large text on label (306)
    private const int TextSizeLarge = 320;
    // padding of objects on the label (18) 
    private const int LabelInternalPadding = 16;
    // horizontal position of the label heading text
    private const int LabelHeadingX = -72 + LabelInternalPadding;
    // vertical position of the label heading text
    private const int LabelHeadingY = -72 + LabelInternalPadding;
    // horizontal position of the label part name text
    private const int LabelPartNameX = LabelInternalPadding;
    // vertical position of the label part name text
    private const int LabelPartNameY = CodeDimension;
    // left X coordinate of Code
    private const int CodePositionX1 = LabelDimension - CodeDimension - LabelInternalPadding;
    // top Y coordinate of Code
    private const int CodePositionY1 = LabelInternalPadding;
    // horizontal position of the Label's information fields
    private const int LabelFieldsX = LabelInternalPadding * 2;
    // vertical position of the Label's information fields
    private const int LabelFieldsY = CodeDimension + TextSizeMedium + (LabelInternalPadding * 4);

    // private class properties
    private readonly Bitmap _image;
    private readonly System.Drawing.Font _fontSmall;
    private readonly System.Drawing.Font _fontMedium;
    private readonly System.Drawing.Font _fontLarge;
    
    /// <summary>
    /// Creates a new Image of a Production Lot Tracing Label that can be sent to Print Spooling.
    /// </summary>
    /// <exception cref="SystemException"></exception>
    public Label() {
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
    /// Access the current Bitmap image assigned to the Label object.
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
            Surface.DrawString(HeadingText, _fontLarge, Brushes.Black, LabelHeadingX, LabelHeadingY);
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
                // if the field is the operator ID, do not include in the shown text
                if (_field.Contains("Operator")) {
                    continue;
                // if the field is the Part info, do not include the Part Name
                } else if (_field.Contains("Part")) {
                    LabelFieldsBody += $"{_field.Split("\n")[0]}\n";
                // add the field as normal
                } else {
                    LabelFieldsBody += $"{_field}\n";
                }
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