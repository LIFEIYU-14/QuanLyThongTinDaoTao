using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace QuanLyThongTinDaoTao.Helpers
{
    public class QRCodeHelper
    {
        public static byte[] GenerateQRCode(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
                {
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        using (Bitmap qrCodeImage = qrCode.GetGraphic(10))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                qrCodeImage.Save(ms, ImageFormat.Png);
                                return ms.ToArray();
                            }
                        }
                    }
                }
            }
        }
    }
}