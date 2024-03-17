namespace QRCodeGenerator.Creator.Interfaces
{
    public interface IQrCodeService
    {
        byte[] CreateQrCode(string data);
    }
}
