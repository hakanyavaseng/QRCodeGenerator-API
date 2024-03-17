using QRCodeGenerator.Creator.Interfaces;
using QRCoder;

namespace QRCodeGenerator.Creator.Concretes
{
    public class QrCodeService : IQrCodeService
    {
        public byte[] CreateQrCode(string data)
        {
            using QRCoder.QRCodeGenerator generator = new();
            QRCodeData qRCodeData = generator.CreateQrCode(data, QRCoder.QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new(qRCodeData);
            return qrCode.GetGraphic(10, new byte[] {84,99,71 }, new byte[] { 240,240,240});
        }
    }
}
