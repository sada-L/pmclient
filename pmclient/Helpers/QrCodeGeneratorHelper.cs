using System;
using System.IO;
using Avalonia.Media.Imaging;
using QRCoder;
using SkiaSharp;

namespace pmclient.Helpers;

public static class QrCodeGeneratorHelper
{
    public static string GenerateQrCodeUri(string secretKey, string userEmail)
    {
        var issuer = "PassManager";
        return $"otpauth://totp/{issuer}:{userEmail}?secret={secretKey}&issuer={issuer}";
    } 
    
    public static SKBitmap GenerateQrCodeImage(string uri)
    {
        using (var generator = new QRCodeGenerator())
        {
            var qrCodeData = generator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);
            
            return SKBitmap.Decode(qrCodeBytes);
        }
    }

    public static Bitmap ConvertToBitmap(SKBitmap bitmap)
    {
        using MemoryStream ms = new MemoryStream();
        bitmap.Encode(ms, SKEncodedImageFormat.Png, 100);
        ms.Seek(0, SeekOrigin.Begin);
        return new Bitmap(ms);
    }
}