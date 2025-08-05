using LotComPrinter.Models.Datasources;

namespace LotComPrinter.Models.Datatypes;

/// <summary>
/// A Label class used to produce full Basket Labels for the LotCom system.
/// </summary>
public partial class BasketLabel : Label 
{   
    /// <summary>
    /// Creates a new Image of a Production Lot Tracing Label that can be sent to Print Spooling.
    /// </summary>
    public BasketLabel() : base
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
            HeadingVerticalAnchor: LabelDimensions.PaddingLevels.Thin - 28,
            SubHeadingHorizontalAnchor: LabelDimensions.PaddingLevels.Thin - 56,
            SubHeadingVerticalAnchor: (LabelDimensions.PaddingLevels.Wide * 2) + LabelDimensions.TextSizes.Large,
            BodyTitleHorizontalAnchor: LabelDimensions.PaddingLevels.Thin,
            BodyTitleVerticalAnchor: LabelDimensions.CodeSizes.Default,
            CodeHorizontalAnchor: LabelDimensions.LabelSizes.Default - Convert.ToInt32(LabelDimensions.CodeSizes.Default * 1.5) - LabelDimensions.PaddingLevels.Thin,
            CodeVerticalAnchor: LabelDimensions.PaddingLevels.Thin,
            BodyHorizontalAnchor: LabelDimensions.PaddingLevels.Thin * 2,
            BodyVerticalAnchor: LabelDimensions.CodeSizes.Default + LabelDimensions.TextSizes.Small + (LabelDimensions.PaddingLevels.Thin * 4)
        )
    )
    {

    }
}