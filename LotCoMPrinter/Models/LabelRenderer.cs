using System.Reflection;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace LotCoMPrinter.Models;

public static class LabelRenderer<T> {
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
    private const int ProcessTitleX = 10;            // horizontal position of the process title
    private const int ProcessTitleY = CodeDimension; // vertical position of the process title
    private const int LabelFieldsX = 10;        // horizontal position of the Label's information fields
    private const int LabelFieldsY = ProcessTitleY + TextSizeSmall;  // vertical position of the Label's information fields
    
    /// <summary>
    /// Renders a visual recreation a Label object that mirrors the appearance of the physical Label.
    /// Composition of Asynchronous tasks:
    /// 1: Elicit necessary Label information from Label object
    /// 2: Generate a new QRCode from the Label Fields
    /// 3: Generate a new Arial Font group
    /// 4: Generate a new Label Base Bitmap Image
    /// 5: Apply the Heading to the Label
    /// 6: Apply the QRCode to the Label
    /// 7: Apply the Process Title to the Label
    /// 8: Apply the Label Fields body to the Label
    /// </summary>
    /// <remarks>Throws an Exception if the Arial font group cannot be found on the machine.</remarks>
    /// <param name="LabelToRender">The Label object to Render and use as the source of Label information.</param>
    /// <returns>A byte[] containing the image in bytes.</returns>
    /// <exception cref="FontFamilyNotFoundException"></exception>
    public static async Task<byte[]> RenderLabel(T LabelToRender) {
        // configure the Renderer
        if (LabelToRender == null) {
            throw new ArgumentException("Cannot Render null.");
        }
        string? ProcessTitle = typeof(T).GetProperties().ElementAt(0).GetValue(LabelToRender)?.ToString();
        if (ProcessTitle == null) {
            throw new ArgumentException($"Could not access ProcessTitle of {LabelToRender}");
        }
        // TEMPORARY!
        string HeadingText = "123";     
        // get Label information
        List<string> LabelFields = await GetLabelFieldsAsync(LabelToRender);
        // create an Arial font group (Small & Large)
        Tuple<SixLabors.Fonts.Font, TextOptions, SixLabors.Fonts.Font, TextOptions> FontGroup = await ConfigureFontsAsync();
        // create Label base Image and color it white
        SixLabors.ImageSharp.Image NewLabel = await GenerateNewBaseAsync();
        // apply the Label heading text
        NewLabel = await ApplyLabelHeadingAsync(NewLabel, FontGroup.Item3, FontGroup.Item4, HeadingText);
        // create a new QRCode object
        QRCode LabelCode = await GenerateQRCodeAsync(LabelFields);
        // apply the QR Code
        NewLabel = await ApplyQRCodeAsync(NewLabel, LabelCode);
        // apply the Process Title
        NewLabel = await ApplyProcessTitleAsync(NewLabel, ProcessTitle, FontGroup.Item1, FontGroup.Item2);
        // apply the Label Fields text
        NewLabel = await ApplyLabelFieldsAsync(NewLabel, LabelFields, FontGroup.Item1, FontGroup.Item2);
        // convert the final Label Image to a Stream and save it to the Renderer
        MemoryStream ByteStream = new();
        NewLabel.Save(ByteStream, new PngEncoder());
        return ByteStream.ToArray();
    }

    /// <summary>
    /// Elicits and converts all fields of the Label to a Collection of strings that can be encoded.
    /// </summary>
    /// <param name="LabelToConvert">The Label object to Render.</param>
    /// <returns></returns>
    private static async Task<List<string>> GetLabelFieldsAsync(T LabelToConvert) {
        // validate that LabelToRender is accessible and not null
        if (LabelToConvert == null) {
            throw new ArgumentException("Cannot access null.");
        }
        // retrieve the list of Properties of the Label
        IEnumerable<PropertyInfo> Properties = LabelToConvert.GetType().GetProperties();
        // convert the value of each of the Label's fields to a string and store it
        List<string> FieldStrings = [];
        foreach (PropertyInfo _property in Properties) {
            // utilize multi-threading to achieve this conversion faster
            await Task.Run(() => {
                // skip AsDisplayable, Scanner properties
                if (_property.Name.Equals("AsDisplayable") || _property.Name.Equals("Scanner")) {
                // avoid an InvalidCastException when converting LabelFields on a ScrapLabel
                } else if (_property.Name == "LabelFields") {
                    #pragma warning disable CS8602
                    // add each field in LabelFields to the Field Strings list
                    foreach (string? _field in (string[]?)_property.GetValue(LabelToConvert)) {
                        FieldStrings.Add($"Field: {_field}");
                    }
                    #pragma warning restore CS8602
                // single string property value, append normally
                } else {
                    FieldStrings.Add($"{_property.Name}: {_property.GetValue(LabelToConvert)}");
                }
            });
        }
        return FieldStrings;
    }

    /// <summary>
    /// Creates and configures a new Arial font group of Small and Large fonts with TextOptions for each.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FontFamilyNotFoundException"></exception>
    private static async Task<Tuple<SixLabors.Fonts.Font, TextOptions, SixLabors.Fonts.Font, TextOptions>> ConfigureFontsAsync() {
        // create a new CPU thread to find the Arial font and configure the fonts
        return await Task.Run(() => {
            // try to find the Arial Font in the system
            if (!SystemFonts.TryGet("Arial", out FontFamily Arial)) {
                throw new FontFamilyNotFoundException($"Couldn't find font Arial");
            }
            // create a new Arial font at the Small Textsize
            SixLabors.Fonts.Font FontSmall = Arial.CreateFont(TextSizeSmall, FontStyle.Regular);
            // set the kerning and DPI (sizing) of the small font characters
            TextOptions FontSmallOptions = new TextOptions(FontSmall) {Dpi = 72, KerningMode = KerningMode.Standard};
            // create a new Arial font at the Large Textsize
            SixLabors.Fonts.Font FontLarge = Arial.CreateFont(TextSizeLarge, FontStyle.Bold);
            // set the kerning and DPI (sizing) of the font characters
            TextOptions FontLargeOptions = new TextOptions(FontLarge) {Dpi = 72, KerningMode = KerningMode.Standard};
            // yield the resulting font group
            return new Tuple<SixLabors.Fonts.Font, TextOptions, SixLabors.Fonts.Font, TextOptions> (
                FontSmall, FontSmallOptions, FontLarge, FontLargeOptions);
        });
    }

    /// <summary>
    /// Creates a new 900x900 (3x3in) White Bitmap Image to use as a Label Base.
    /// </summary>
    /// <returns></returns>
    private static async Task<SixLabors.ImageSharp.Image> GenerateNewBaseAsync() {
        // create a new CPU thread to generate a new Label Base Bitmap Image
        return await Task.Run(() => {
            // create a new white Label Base
            var NewBase = new Image<Rgba32>(LabelDimension, LabelDimension);
            NewBase.Mutate(x => x.BackgroundColor(SixLabors.ImageSharp.Color.White));
            return NewBase;
        });
    }

    /// <summary>
    /// Generates a new QR Code containing all of the fields on the Label, encoded and formatted.
    /// </summary>
    /// <param name="ProcessTitle">The name of the Process producing the Label.</param>
    /// <param name="LabelFields">The Fields of information to encode in the QR Code.</param>
    /// <returns></returns>
    private static async Task<QRCode> GenerateQRCodeAsync(List<string> LabelFields) {
        // create a new QR Code
        return await Task.Run(() => {
            return new QRCode(LabelFields);
        });
    }

    /// <summary>
    /// Applies a large Header Text to the top-left corner of the Label. This is usually a JBK or Lot Number.
    /// </summary>
    /// <param name="LabelBase">The base Image used to construct the Label.</param>
    /// <param name="Font">The Large Font configured by the Renderer.</param>
    /// <param name="Options">The Large Font Options configured by the Renderer.</param>
    /// <param name="HeadingText">The string to be displayed at the Heading of the Label.</param>
    /// <returns></returns>
    private static async Task<SixLabors.ImageSharp.Image> ApplyLabelHeadingAsync(
        SixLabors.ImageSharp.Image LabelBase, SixLabors.Fonts.Font Font, TextOptions Options, string HeadingText
    ) {
        // start a new CPU thread to apply the header to the LabelBase
        await Task.Run(() => {
            // measure the minimum size of the text in the configured Font
            var HeadingSize = TextMeasurer.MeasureSize(HeadingText, Options);
            // write the Header onto the LabelBase image
            LabelBase.Mutate(x => x.DrawText(
                HeadingText, 
                Font,
                SixLabors.ImageSharp.Color.Black,
                new SixLabors.ImageSharp.PointF(LabelHeadingX + TextPadding, LabelHeadingY + TextPadding)
            ));
        });
        return LabelBase;
    }

    /// <summary>
    /// Applies a QR Code to the top-right corner of the Label.
    /// </summary>
    /// <param name="LabelBase">The base Image used to construct the Label.</param>
    /// <param name="LabelCode">The QR Code to apply to the LabelBase.</param>
    /// <returns></returns>
    private static async Task<SixLabors.ImageSharp.Image> ApplyQRCodeAsync(SixLabors.ImageSharp.Image LabelBase, QRCode LabelCode) {
        // start a new CPU thread to apply the QR code to the LabelBase
        await Task.Run(() => {
            // draw the QR bytes onto the LabelBase
            LabelBase.Mutate(x => x.DrawImage(
                LabelCode.AsImage(), 
                new SixLabors.ImageSharp.Point(CodePositionX1, CodePositionY1), 1.0f));
        });
        return LabelBase;
    }

    /// <summary>
    /// Applies a Process Title to the middle of the Label, below the Heading and above the Label Fields list.
    /// </summary>
    /// <param name="LabelBase">The base Image used to construct the Label.</param>
    /// <param name="ProcessTitle">The Process to apply to the LabelBase; generally the name of the Process producing the Label.</param>
    /// <returns></returns>
    public static async Task<SixLabors.ImageSharp.Image> ApplyProcessTitleAsync(
        SixLabors.ImageSharp.Image LabelBase, string ProcessTitle, SixLabors.Fonts.Font FontSmall, TextOptions FontOptions
    ) {
        // start a new CPU thread to apply the Process Title to the LabelBase
        await Task.Run(() => {
            // measure the minimum size of the text in the configured Font
            var ProcessTitleSize = TextMeasurer.MeasureSize(ProcessTitle, FontOptions);
            // write the Header onto the LabelBase image
            LabelBase.Mutate(x => x.DrawText(
                ProcessTitle, 
                FontSmall,
                SixLabors.ImageSharp.Color.Black,
                new SixLabors.ImageSharp.PointF(ProcessTitleX + TextPadding, ProcessTitleY + TextPadding)
            ));
        });
        return LabelBase;
    }

    /// <summary>
    /// Applies body text obtained from the Label Fields to the bottom half of the LabelBase.
    /// </summary>
    /// <param name="LabelBase"></param>
    /// <param name="LabelFields"></param>
    /// <param name="FontSmall"></param>
    /// <param name="FontOptions"></param>
    /// <returns></returns>
    public static async Task<SixLabors.ImageSharp.Image> ApplyLabelFieldsAsync(
        SixLabors.ImageSharp.Image LabelBase, List<string> LabelFields, SixLabors.Fonts.Font FontSmall, TextOptions FontOptions
    ) {
        // start a new CPU thread to apply the Label Fields to the LabelBase
        await Task.Run(() => {
            // combine the LabelFields into a string deliniated by newlines
            string LabelFieldsBody = "";
            foreach (string _field in LabelFields) {
                LabelFieldsBody += _field + "\n";
            }
            // measure the minimum size of the text in the configured Font
            var LabelFieldsSize = TextMeasurer.MeasureSize(LabelFieldsBody, FontOptions);
            // write the Header onto the LabelBase image
            LabelBase.Mutate(x => x.DrawText(
                LabelFieldsBody, 
                FontSmall,
                SixLabors.ImageSharp.Color.Black,
                new SixLabors.ImageSharp.PointF(LabelFieldsX + TextPadding, LabelFieldsY + TextPadding)
            ));
        });
        return LabelBase;
    }
}