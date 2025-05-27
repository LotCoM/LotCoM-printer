using System.Drawing;
using QRCoder;

namespace LotCoMPrinter.Models.Datatypes;

# pragma warning disable CA1416 // Validate platform compatibility

public class QRCode
{
    private Bitmap? _code;
    /// <summary>
    /// QR Code Bitmap image.
    /// </summary>
    public Bitmap? Code
    {
        get { return _code; }
        set
        {
            _code = value;
        }
    }

    /// <summary>
    /// Constructs a QR Code with a bitmap image property.
    /// </summary>
    /// <param name="Data">The individual data fields to encode in the QR Code.</param>
    /// <exception cref="ArgumentException"></exception>
    public QRCode(List<string> Data)
    {
        // create a new QR Code generator and format the data
        QRCodeGenerator Generator = new QRCodeGenerator();
        string CodeData = "";
        foreach (string _field in Data)
        {
            CodeData += $"{_field}|";
        }
        // remove the trailing | symbol
        if (CodeData.Length > 0)
        {
            CodeData = CodeData[..^1];
        }
        // generate new Data to be encoded, encode that data, generate a Code image, and save the image
        QRCodeData NewQRCode = Generator.CreateQrCode(CodeData, QRCodeGenerator.ECCLevel.H);
        BitmapByteQRCode QRCodeBitmap = new(NewQRCode);
        Stream ImageData = new MemoryStream(QRCodeBitmap.GetGraphic(20));
        Code = new Bitmap(ImageData);
        ImageData.Dispose();
    }
}
# pragma warning restore CA1416 // Validate platform compatibility