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
            LabelDimensions.LabelSizes.Default,
            LabelDimensions.CodeSizes.Default,
            LabelDimensions.TextSizes.Tiny,
            LabelDimensions.TextSizes.Small,
            LabelDimensions.TextSizes.Medium,
            LabelDimensions.PaddingLevels.Thin,
            LabelDimensions.PaddingLevels.Thin - 56,
            LabelDimensions.PaddingLevels.Thin - 56,
            LabelDimensions.PaddingLevels.Thin,
            LabelDimensions.PaddingLevels.Thin - 56 + (LabelDimensions.TextSizes.Medium * 2) + LabelDimensions.PaddingLevels.Thin,
            0, // no QR Codes
            0, // no QR Codes
            LabelDimensions.PaddingLevels.Thin * 2,
            LabelDimensions.TextSizes.Medium + LabelDimensions.TextSizes.Small + (LabelDimensions.PaddingLevels.Thin * 4)
        )
    )
    {
        
    }
}