using System;
using System.Drawing;
using System.IO;
using System.Linq;
using QRCoder;
using QuanLyThongTinDaoTao.Models;
using Newtonsoft.Json;

public class HocVienService
{
    private readonly DbContextThongTinDaoTao _context;

    public HocVienService(DbContextThongTinDaoTao context)
    {
        _context = context;
    }

    // Thay Guid hocVienId thành string hocVienId để phù hợp với AppUser.Id
    public string GenerateQRCodeForStudent(string hocVienId)
    {
        // Lấy thông tin học viên theo Id (string)
        var hocVien = _context.HocViens.FirstOrDefault(hv => hv.Id == hocVienId);
        if (hocVien == null) return null;

        var qrInfo = new
        {
            HocVienId = hocVienId
        };

        string qrData = JsonConvert.SerializeObject(qrInfo);

        using (var qrGenerator = new QRCodeGenerator())
        {
            using (var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q))
            {
                using (var qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(30, Color.Black, Color.White, true))
                    {
                        using (var ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] qrCodeBytes = ms.ToArray();
                            string qrBase64 = Convert.ToBase64String(qrCodeBytes);

                            // Lưu QR Code Base64 vào DB
                            hocVien.QR_Code_HV = qrBase64;
                            _context.SaveChanges();

                            return qrBase64;
                        }
                    }
                }
            }
        }
    }
}
