﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using QRCoder;
using QuanLyThongTinDaoTao.Models;
using Newtonsoft.Json;

public class GiangVienService
{
    private readonly DbContextThongTinDaoTao _context;

    public GiangVienService(DbContextThongTinDaoTao context)
    {
        _context = context;
    }

    public string GenerateQRCodeForTeacher(Guid giangVienId)
    {
        var giangVien = _context.GiangViens.FirstOrDefault(gv => gv.NguoiDungId == giangVienId);
        if (giangVien == null) return null;

        var qrInfo = new
        {
            GiangVienId = giangVienId
        };

        string qrData = JsonConvert.SerializeObject(qrInfo);

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

                        // Lưu mã QR vào DB cho giảng viên
                        giangVien.QR_Code_GV = qrBase64;
                        _context.SaveChanges();

                        return qrBase64;
                    }
                }
            }
        }
    }
}