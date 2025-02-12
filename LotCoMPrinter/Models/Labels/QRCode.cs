using QRCoder;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace LotCoMPrinter.Models.Labels;

public class QRCode {
    // code size constant (square)
    private const int CodeDimension = 150;  // square size of QR code (375)
    private byte[] _codeBytes;
    public byte[] CodeBytes {
        get {return _codeBytes;}
        set {_codeBytes = value;}
    }

    /// <summary>
    /// Constructs a QR Code with a bitmap image property.
    /// </summary>
    /// <param name="ProcessTitle">The name of the Process producing the Label.</param>
    /// <param name="LabelFields">The Fields of information to encode in the QR Code.</param>
    public QRCode(IEnumerable<string> LabelFields) {
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
        PngByteQRCode QRCodePNG = new(NewQRCode);
        // save the QRCode as a stream of bytes
        _codeBytes = QRCodePNG.GetGraphic(20);
    }

    /// <summary>
    /// Returns the Code's bitmap converted to an Image.
    /// </summary>
    /// <returns></returns>
    public SixLabors.ImageSharp.Image AsImage() {
        // convert the byte array to an Image
        SixLabors.ImageSharp.Image CodeImage = SixLabors.ImageSharp.Image.Load<Rgba32>(_codeBytes);
        // size the image to the proper dimensions
        CodeImage.Mutate(x => x.Resize(new ResizeOptions {
            Mode = SixLabors.ImageSharp.Processing.ResizeMode.Stretch,
            Size = new SixLabors.ImageSharp.Size(CodeDimension, CodeDimension)
        }));
        return CodeImage;
    }
}