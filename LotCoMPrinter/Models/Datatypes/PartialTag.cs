using LotComPrinter.Models.Datasources;

namespace LotComPrinter.Models.Datatypes;

/// <summary>
/// A Label class used to track in-progress or partial Production data.
/// </summary>
public partial class PartialTag : Label
{   
    /// <summary>
    /// Creates a new Image of a Partial Production Data Set Label that can be sent to Print Spooling.
    /// </summary>
    public PartialTag() : base
    (
        new LabelDimensions
        (
            Dimension: LabelDimensions.LabelSizes.Default,
            CodeDimension: LabelDimensions.CodeSizes.Default,
            TextSizeSmall: LabelDimensions.TextSizes.Tiny,
            TextSizeMedium: LabelDimensions.TextSizes.Small,
            TextSizeLarge: LabelDimensions.TextSizes.Large,
            InternalPadding: LabelDimensions.PaddingLevels.Thin,
            HeadingHorizontalAnchor: LabelDimensions.PaddingLevels.Thin - 56,
            HeadingVerticalAnchor: LabelDimensions.PaddingLevels.Thin - 56,
            SubHeadingHorizontalAnchor: 0, // no sub-heading
            SubHeadingVerticalAnchor: 0, // no sub-heading
            BodyTitleHorizontalAnchor: LabelDimensions.PaddingLevels.Thin * 2,
            BodyTitleVerticalAnchor: (LabelDimensions.PaddingLevels.Thin * 8) + LabelDimensions.TextSizes.Large,
            CodeHorizontalAnchor: 0, // no QR Codes
            CodeVerticalAnchor: 0, // no QR Codes
            BodyHorizontalAnchor: LabelDimensions.PaddingLevels.Thin * 2,
            BodyVerticalAnchor: (LabelDimensions.PaddingLevels.Thin * 16) + LabelDimensions.TextSizes.Large + LabelDimensions.TextSizes.Small
        )
    )
    {
        
    }
}