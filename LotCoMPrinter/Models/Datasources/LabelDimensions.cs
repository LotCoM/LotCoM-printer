namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// Provides a set of Label dimensions and item positions with defaults and controlled values.
/// </summary>
public class LabelDimensions
{
    /// <summary>
    /// Provides default Label sizes (length of each side as a square Label).
    /// </summary>
    public static class LabelSizes
    {
        /// <summary>
        /// 3.00 x 3.00 inch Label size.
        /// </summary>
        public const int Default = 1800;
    }

    /// <summary>
    /// Provides default QR Code sizes (length of each side as a square QR Code).
    /// </summary>
    public static class CodeSizes
    {
        /// <summary>
        /// Roughly 1.20 x 1.20 inch QR Code 
        /// (smallest Code size while preserving peak readability).
        /// </summary>
        public const int Default = 700;
    }

    /// <summary>
    /// Provides default text sizes.
    /// </summary>
    public static class TextSizes
    {
        public const int Tiny = 48;
        public const int Small = 76;
        public const int Medium = 128;
        public const int Large = 192;
    }

    /// <summary>
    /// Provides default padding (space between items) levels.
    /// </summary>
    public static class PaddingLevels
    {
        public const int Thin = 20;
        public const int Wide = 32;
    }

    /// <summary>
    /// The length of each side of the Label (square).
    /// </summary>
    public int Dimension = LabelSizes.Default;

    /// <summary>
    /// The length of each side of the QR Code on the Label (square).
    /// </summary>
    public int CodeDimension = CodeSizes.Default;

    /// <summary>
    /// The font size for small text on the Label.
    /// </summary>
    public int TextSizeSmall = TextSizes.Tiny;

    /// <summary>
    /// The font size for medium text on the Label.
    /// </summary>
    public int TextSizeMedium = TextSizes.Small;
    
    /// <summary>
    /// The font size for large text on the Label.
    /// </summary>
    public int TextSizeLarge = TextSizes.Large;

    /// <summary>
    /// Padding width/height applied between each item on the Label.
    /// </summary>
    public int InternalPadding = PaddingLevels.Thin;

    /// <summary>
    /// The horizontal position of the Label's Heading text.
    /// </summary>
    public int HeadingHorizontalAnchor;

    /// <summary>
    /// The vertical position of the Label's Heading text.
    /// </summary>
    public int HeadingVerticalAnchor;

    /// <summary>
    /// The horizontal position of the Label's Sub-Heading text.
    /// This text is the same size as the Heading text.
    /// </summary>
    public int SubHeadingHorizontalAnchor;

    /// <summary>
    /// The vertical position of the Label's Sub-Heading text.
    /// This text is the same size as the Heading text.
    /// </summary>
    public int SubHeadingVerticalAnchor;

    /// <summary>
    /// The horizontal position of the Label's Body Title text.
    /// </summary>
    public int BodyTitleHorizontalAnchor;
    
    /// <summary>
    /// The vertical position of the Label's Body Title text.
    /// </summary>
    public int BodyTitleVerticalAnchor;
    
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
    /// <param name="SubHeadingHorizontalAnchor">The horizontal position of the Label's Sub-heading text.</param>
    /// <param name="SubHeadingVerticalAnchor">The vertical position of the Label's Sub-heading text.</param>
    /// <param name="BodyTitleHorizontalAnchor">The horizontal position of the Label's Body Title text.</param>
    /// <param name="BodyTitleVerticalAnchor">The vertical position of the Label's Body Title text.</param>
    /// <param name="CodeHorizontalAnchor">The horizontal (left) position of the QR Code on the Label.</param>
    /// <param name="CodeVerticalAnchor">The vertical (top) position of the QR Code on the Label.</param>
    /// <param name="BodyHorizontalAnchor">The horizontal position of the Label's Data Fields text.</param>
    /// <param name="BodyVerticalAnchor">The vertical position of the Label's Data Fields text.</param>
    public LabelDimensions(int? Dimension, int? CodeDimension, int? TextSizeSmall, int? TextSizeMedium, int? TextSizeLarge, int? InternalPadding, int HeadingHorizontalAnchor, int HeadingVerticalAnchor, int SubHeadingHorizontalAnchor, int SubHeadingVerticalAnchor, int BodyTitleHorizontalAnchor, int BodyTitleVerticalAnchor, int CodeHorizontalAnchor, int CodeVerticalAnchor, int BodyHorizontalAnchor, int BodyVerticalAnchor)
    {
        if (Dimension is not null)
        {
            this.Dimension = (int)Dimension;
        }
        if (CodeDimension is not null)
        {
            this.CodeDimension = (int)CodeDimension;
        }
        if (TextSizeSmall is not null)
        {
            this.TextSizeSmall = (int)TextSizeSmall;
        }
        if (TextSizeMedium is not null)
        {
            this.TextSizeMedium = (int)TextSizeMedium;
        }
        if (TextSizeLarge is not null)
        {
            this.TextSizeLarge = (int)TextSizeLarge;
        }
        if (InternalPadding is not null)
        {
            this.InternalPadding = (int)InternalPadding;
        }
        this.HeadingHorizontalAnchor = HeadingHorizontalAnchor;
        this.HeadingVerticalAnchor = HeadingVerticalAnchor;
        this.SubHeadingHorizontalAnchor = SubHeadingHorizontalAnchor;
        this.SubHeadingVerticalAnchor = SubHeadingVerticalAnchor;
        this.BodyTitleHorizontalAnchor = BodyTitleHorizontalAnchor; 
        this.BodyTitleVerticalAnchor = BodyTitleVerticalAnchor;
        this.CodeHorizontalAnchor = CodeHorizontalAnchor; 
        this.CodeVerticalAnchor = CodeVerticalAnchor;
        this.BodyHorizontalAnchor = BodyHorizontalAnchor;
        this.BodyVerticalAnchor = BodyVerticalAnchor;
    }
}