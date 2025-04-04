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

    public string GenerateQRCodeForStudent(Guid hocVienId)
    {
        // Lấy thông tin học viên
        var hocVien = _context.HocViens.FirstOrDefault(hv => hv.NguoiDungId == hocVienId);
        if (hocVien == null) return null;

        // Tạo đối tượng chỉ chứa thông tin cần thiết cho điểm danh
        var qrInfo = new
        {
            HocVienId = hocVienId
        };

        // Chuyển đối tượng trên thành chuỗi JSON
        string qrData = JsonConvert.SerializeObject(qrInfo);

        // Tạo mã QR code từ chuỗi JSON
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);

            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                using (Bitmap qrCodeImage = qrCode.GetGraphic(30, Color.Black, Color.White, true))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] qrCodeBytes = ms.ToArray();
                        string qrBase64 = Convert.ToBase64String(qrCodeBytes);

                        // Lưu mã QR vào DB cho học viên (nếu cần)
                        hocVien.QR_Code_HV = qrBase64;
                        _context.SaveChanges();

                        return qrBase64; // Trả về mã QR dạng Base64
                    }
                }
            }
        }
    }
}
