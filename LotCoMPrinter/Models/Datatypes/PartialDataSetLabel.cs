namespace LotCoMPrinter.Models.Datasources;

/// <summary>
/// A Label class used to track in-progress or partial Production data.
/// </summary>
public partial class PartialDataSetLabel : Label
{   
    /// <summary>
    /// Creates a new Image of a Partial Production Data Set Label that can be sent to Print Spooling.
    /// </summary>
    public PartialDataSetLabel() : base
    (
        new LabelDimensions
        (
            Dimension: LabelDimensions.LabelSizes.Default,
            CodeDimension: LabelDimensions.CodeSizes.Default,
            TextSizeSmall: LabelDimensions.TextSizes.Tiny,
            TextSizeMedium: LabelDimensions.TextSizes.Small,
            TextSizeLarge: LabelDimensions.TextSizes.Medium,
            InternalPadding: LabelDimensions.PaddingLevels.Thin,
            HeadingHorizontalAnchor: LabelDimensions.PaddingLevels.Thin - 56,
            HeadingVerticalAnchor: LabelDimensions.PaddingLevels.Thin - 56,
            SubHeadingHorizontalAnchor: LabelDimensions.PaddingLevels.Thin - 56,
            SubHeadingVerticalAnchor: LabelDimensions.PaddingLevels.Thin + LabelDimensions.TextSizes.Large,
            BodyTitleHorizontalAnchor: LabelDimensions.PaddingLevels.Thin,
            BodyTitleVerticalAnchor: LabelDimensions.PaddingLevels.Thin - 56 + (LabelDimensions.TextSizes.Medium * 2) + LabelDimensions.PaddingLevels.Thin,
            CodeHorizontalAnchor: 0, // no QR Codes
            CodeVerticalAnchor: 0, // no QR Codes
            BodyHorizontalAnchor: LabelDimensions.PaddingLevels.Thin * 2,
            BodyVerticalAnchor: LabelDimensions.TextSizes.Medium + LabelDimensions.TextSizes.Small + (LabelDimensions.PaddingLevels.Thin * 4)
        )
    )
    {
        
    }
}