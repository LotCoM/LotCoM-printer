namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a set of Label dimensions and item positions with defaults and controlled values.
/// </summary>
public class LabelDimensions
{
    /// <summary>
    /// Provides default Label sizes (length of each side as a square Label).
    /// </summary>
    public enum LabelSizes
    {
        /// <summary>
        /// 3.00 x 3.00 inch Label size.
        /// </summary>
        Default = 1440
    }

    /// <summary>
    /// Provides default QR Code sizes (length of each side as a square QR Code).
    /// </summary>
    public enum CodeSizes
    {
        /// <summary>
        /// Roughly 1.20 x 1.20 inch QR Code 
        /// (smallest Code size while preserving peak readability).
        /// </summary>
        Default = 600
    }

    /// <summary>
    /// Provides default text sizes.
    /// </summary>
    public enum TextSizes
    {
        Tiny = 42,
        Small = 76,
        Medium = 128,
        Large = 192
    }

    /// <summary>
    /// Provides default padding (space between items) levels.
    /// </summary>
    public enum PaddingLevels
    {
        Thin = 12,
        Wide = 24,
    }

    /// <summary>
    /// The length of each side of the Label (square).
    /// </summary>
    public LabelSizes Dimension = LabelSizes.Default;

    /// <summary>
    /// The length of each side of the QR Code on the Label (square).
    /// </summary>
    public CodeSizes CodeDimension = CodeSizes.Default;

    /// <summary>
    /// The font size for small text on the Label.
    /// </summary>
    public TextSizes TextSizeSmall = TextSizes.Tiny;

    /// <summary>
    /// The font size for medium text on the Label.
    /// </summary>
    public TextSizes TextSizeMedium = TextSizes.Small;
    
    /// <summary>
    /// The font size for large text on the Label.
    /// </summary>
    public TextSizes TextSizeLarge = TextSizes.Large;

    /// <summary>
    /// Padding width/height applied between each item on the Label.
    /// </summary>
    public PaddingLevels InternalPadding = PaddingLevels.Thin;

    /// <summary>
    /// The horizontal position of the Label's Heading text.
    /// </summary>
    public int HeadingHorizontalAnchor;

    /// <summary>
    /// The vertical position of the Label's Heading text.
    /// </summary>
    public int HeadingVerticalAnchor;
    
    /// <summary>
    /// The horizontal position of the Label's Part Name text.
    /// </summary>
    public int PartNameHorizontalAnchor;

    /// <summary>
    /// The vertical position of the Label's Part Name text.
    /// </summary>
    public int PartNameVerticalAnchor;
    
    /// <summary>
    /// The horizontal (left) position of the QR Code on the Label.
    /// </summary>
    public int CodeHorizontalAnchor;

    /// <summary>
    /// The vertical (top) position of the QR Code on the Label.
    /// </summary>
    public int CodeVerticalAnchor;

    /// <summary>
    /// The horizontal position of the Label's Data Fields text.
    /// </summary>
    public int BodyHorizontalAnchor;

    /// <summary>
    /// The vertical position of the Label's Data Fields text.
    /// </summary>
    public int BodyVerticalAnchor;

    /// <summary>
    /// Creates a Dimensions control structure for a Label object.
    /// </summary>
    /// <param name="Dimension">The length of each side of the Label (square).</param>
    /// <param name="CodeDimension">The length of each side of the QR Code on the Label (square).</param>
    /// <param name="TextSizeSmall">The font size for small text on the Label.</param>
    /// <param name="TextSizeMedium">The font size for medium text on the Label.</param>
    /// <param name="TextSizeLarge">The font size for large text on the Label.</param>
    /// <param name="InternalPadding">Padding width/height applied between each item on the Label.</param>
    /// <param name="HeadingHorizontalAnchor">The horizontal position of the Label's Heading text.</param>
    /// <param name="HeadingVerticalAnchor">The vertical position of the Label's Heading text.</param>
    /// <param name="PartNameHorizontalAnchor">The horizontal position of the Label's Part Name text.</param>
    /// <param name="PartNameVerticalAnchor">The vertical position of the Label's Part Name text.</param>
    /// <param name="CodeHorizontalAnchor">The horizontal (left) position of the QR Code on the Label.</param>
    /// <param name="CodeVerticalAnchor">The vertical (top) position of the QR Code on the Label.</param>
    /// <param name="BodyHorizontalAnchor">The horizontal position of the Label's Data Fields text.</param>
    /// <param name="BodyVerticalAnchor">The vertical position of the Label's Data Fields text.</param>
    public LabelDimensions(LabelSizes? Dimension, CodeSizes? CodeDimension, TextSizes? TextSizeSmall, TextSizes? TextSizeMedium, TextSizes? TextSizeLarge, PaddingLevels? InternalPadding, int HeadingHorizontalAnchor, int HeadingVerticalAnchor, int PartNameHorizontalAnchor, int PartNameVerticalAnchor, int CodeHorizontalAnchor, int CodeVerticalAnchor, int BodyHorizontalAnchor, int BodyVerticalAnchor)
    {
        if (Dimension is not null)
        {
            this.Dimension = (LabelSizes)Dimension;
        }
        if (CodeDimension is not null)
        {
            this.CodeDimension = (CodeSizes)CodeDimension;
        }
        if (TextSizeSmall is not null)
        {
            this.TextSizeSmall = (TextSizes)TextSizeSmall;
        }
        if (TextSizeMedium is not null)
        {
            this.TextSizeMedium = (TextSizes)TextSizeMedium;
        }
        if (TextSizeLarge is not null)
        {
            this.TextSizeLarge = (TextSizes)TextSizeLarge;
        }
        if (InternalPadding is not null)
        {
            this.InternalPadding = (PaddingLevels)InternalPadding;
        }
        this.HeadingHorizontalAnchor = HeadingHorizontalAnchor;
        this.HeadingVerticalAnchor = HeadingVerticalAnchor;
        this.PartNameHorizontalAnchor = PartNameHorizontalAnchor; 
        this.PartNameVerticalAnchor = PartNameVerticalAnchor;
        this.CodeHorizontalAnchor = CodeHorizontalAnchor; 
        this.CodeVerticalAnchor = CodeVerticalAnchor;
        this.BodyHorizontalAnchor = BodyHorizontalAnchor;
        this.BodyVerticalAnchor = BodyVerticalAnchor;
    }
}