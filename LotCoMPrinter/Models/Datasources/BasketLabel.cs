namespace LotCoMPrinter.Models.Datasources;

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
            LabelDimensions.LabelSizes.Default,
            LabelDimensions.CodeSizes.Default,
            LabelDimensions.TextSizes.Tiny,
            LabelDimensions.TextSizes.Small,
            LabelDimensions.TextSizes.Large,
            LabelDimensions.PaddingLevels.Thin,
            LabelDimensions.PaddingLevels.Thin - 56,
            LabelDimensions.PaddingLevels.Thin - 28,
            LabelDimensions.PaddingLevels.Thin,
            LabelDimensions.CodeSizes.Default,
            LabelDimensions.LabelSizes.Default - LabelDimensions.CodeSizes.Default - LabelDimensions.PaddingLevels.Thin,
            LabelDimensions.PaddingLevels.Thin,
            LabelDimensions.PaddingLevels.Thin * 2,
            LabelDimensions.CodeSizes.Default + LabelDimensions.TextSizes.Small + (LabelDimensions.PaddingLevels.Thin * 4)
        )
    )
    {

    }
}