using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace LotCoMPrinter.Models.Labels;

public class Label {
    // label dimension constants
    private const int LabelDimension = 900;     // dimension (square dimension) of label
    private const int CodeDimension = 375;      // dimension (square dimension) of QR Code on label
    private const int TextSizeSmall = 36;       // size of small text on label
    private const int TextSizeLarge = 306;      // size of large text on label
    private const int TextPadding = 18;         // padding of text objects on the label
    private const int LabelHeadingX = 0;       // horizontal position of the label heading text
    private const int LabelHeadingY = 0;        // vertical position of the label heading text
    private const int CodePositionX1 = LabelDimension - CodeDimension;  // left X coordinate of Code
    private const int CodePositionY1 = 0;       // top Y coordinate of Code
    private const int LabelFieldsX = 10;        // horizontal position of the Label's information fields
    private const int LabelFieldsY = CodeDimension + TextSizeSmall;  // vertical position of the Label's information fields

    // private class properties
    private readonly Image<Rgba32> _base;
    private readonly SixLabors.Fonts.Font _fontSmall;
    private readonly TextOptions _fontSmallOptions;
    private readonly SixLabors.Fonts.Font _fontLarge;
    private readonly TextOptions _fontLargeOptions;
    
    /// <summary>
    /// Creates a new Image of a Production Lot Tracing Label that can be sent to Print Spooling.
    /// </summary>
    /// <exception cref="FontFamilyNotFoundException"></exception>
    public Label() {
        // load a new Label base
        _base = LoadBase();
        // load Label design fonts (try to find the Arial Font in the system)
        if (!SystemFonts.TryGet("Arial", out FontFamily Arial)) {
            throw new FontFamilyNotFoundException($"Couldn't find font Arial");
        }
        // create a new Arial font at the Small Textsize and set its character options
        _fontSmall = Arial.CreateFont(TextSizeSmall, FontStyle.Regular);
        _fontSmallOptions = new TextOptions(_fontSmall) {Dpi = 72, KerningMode = KerningMode.Standard};
        // create a new Arial font at the Large Textsize and set its character options
        _fontLarge = Arial.CreateFont(TextSizeLarge, FontStyle.Bold);
        _fontLargeOptions = new TextOptions(_fontLarge) {Dpi = 72, KerningMode = KerningMode.Standard};
    }

    /// <summary>
    /// Creates and returns a new, pure-white bitmap image to use as a Label base.
    /// </summary>
    /// <returns></returns>
    private static Image<Rgba32> LoadBase() {
        // create a new white Label Base
        var NewBase = new Image<Rgba32>(LabelDimension, LabelDimension);
        NewBase.Mutate(x => x.BackgroundColor(SixLabors.ImageSharp.Color.White));
        return NewBase;
    }

    /// <summary>
    /// Access the current image assigned to the Label object.
    /// </summary>
    /// <returns></returns>
    public Image<Rgba32> GetImage() {
        return _base;
    }

    /// <summary>
    /// Writes Header text to the top-left corner of the Label.
    /// </summary>
    /// <param name="HeadingText">The text to write.</param>
    /// <returns></returns>
    public async Task AddHeaderAsync(string HeadingText) {
        // start a new CPU thread to apply the header to the LabelBase
        await Task.Run(() => {
            // measure the minimum size of the text in the configured Font
            var HeadingSize = TextMeasurer.MeasureSize(HeadingText, _fontLargeOptions);
            // write the Header onto the LabelBase image
            _base.Mutate(x => x.DrawText(HeadingText, _fontLarge, SixLabors.ImageSharp.Color.Black,
                new SixLabors.ImageSharp.PointF(LabelHeadingX + TextPadding, LabelHeadingY + TextPadding)
            ));
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
            // draw the QR bytes onto the LabelBase
            _base.Mutate(x => x.DrawImage(
                LabelCode.AsImage(), 
                new SixLabors.ImageSharp.Point(CodePositionX1, CodePositionY1), 1.0f));
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
                LabelFieldsBody += _field + "\n";
            }
            // measure the minimum size of the text in the configured Font
            var LabelFieldsSize = TextMeasurer.MeasureSize(LabelFieldsBody, _fontSmallOptions);
            // write the Header onto the LabelBase image
            _base.Mutate(x => x.DrawText(LabelFieldsBody, _fontSmall, SixLabors.ImageSharp.Color.Black,
                new SixLabors.ImageSharp.PointF(LabelFieldsX + TextPadding, LabelFieldsY + TextPadding)
            ));
        });
    }
}