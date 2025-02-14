using System.Drawing;
using QRCoder;

namespace LotCoMPrinter.Models.Labels;

# pragma warning disable CA1416 // Validate platform compatibility

public class QRCode {
    // code Bitmap Image property
    private Bitmap? _codeImage;
    public Bitmap? CodeImage {
        get {return _codeImage;}
        set {_codeImage = value;}
    }

    /// <summary>
    /// Constructs a QR Code with a bitmap image property.
    /// </summary>
    /// <param name="ProcessTitle">The name of the Process producing the Label.</param>
    /// <param name="LabelFields">The Fields of information to encode in the QR Code.</param>
    /// <exception cref="ArgumentException"></exception>
    public QRCode(IEnumerable<string> LabelFields) {
        // ensure there was some data passed
        if (!LabelFields.Any()) {
            throw new ArgumentException("LabelFields must contain at least one field of data to pass into the QR Code encoded data.");
        }
        // create a new QR Code generator
        QRCodeGenerator Coder = new();
        // format the QR Code data
        string CodeData = "";
        foreach (string _field in LabelFields) {
            CodeData += $"{_field}|";
        }
        // remove the trailing | symbol
        CodeData = CodeData.Remove(CodeData.Length - 1);
        // generate new Data to be encoded
        QRCodeData NewQRCode = Coder.CreateQrCode(CodeData, QRCodeGenerator.ECCLevel.H);
        // generate the QR Code as a new PNG image 
        BitmapByteQRCode QRCodeBitmap = new (NewQRCode);
        // save the QRCode Image
        Stream ImageData = new MemoryStream(QRCodeBitmap.GetGraphic(20));
        CodeImage = new Bitmap(ImageData);
        ImageData.Dispose();
    }
}
# pragma warning restore CA1416 // Validate platform compatibility