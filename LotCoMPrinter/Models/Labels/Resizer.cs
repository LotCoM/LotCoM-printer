using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace LotCoMPrinter.Models.Labels;

# pragma warning disable CA1416 // Validate platform compatibility

public static class Resizer {
    
    /// <summary>
    /// Resize the image to print.
    /// Adapted from method by mpen at:
    /// https://stackoverflow.com/a/24199315
    /// </summary>
    /// <param name="image">The image to resize.</param>
    /// <returns>The resized image.</returns>
    public static Bitmap ResizeImage(Bitmap Image, int width, int height) {
        // create bounding rectangle for the destination size
        Rectangle DestinationBounding = new Rectangle(0, 0, width, height);
        Bitmap ResizedImage = new Bitmap(width, height);
        // create an image to place the resulting resize on and set its resolution
        ResizedImage.SetResolution(Image.HorizontalResolution, Image.VerticalResolution);
        // create a Surface to Resize the image on
        using (Graphics Surface = Graphics.FromImage(ResizedImage)) {
            // set the Surface's resizing properties
            Surface.CompositingMode = CompositingMode.SourceCopy;
            Surface.CompositingQuality = CompositingQuality.HighQuality;
            Surface.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Surface.SmoothingMode = SmoothingMode.HighQuality;
            Surface.PixelOffsetMode = PixelOffsetMode.HighQuality;
            // configure the Wrap mode of the Surface (to handle how the Image is drawn from larger to smaller size)
            using (ImageAttributes wrapMode = new ImageAttributes()) {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                // draw the resized image
                Surface.DrawImage(Image, DestinationBounding, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }
        return ResizedImage;
    }
}
# pragma warning restore CA1416 // Validate platform compatibility