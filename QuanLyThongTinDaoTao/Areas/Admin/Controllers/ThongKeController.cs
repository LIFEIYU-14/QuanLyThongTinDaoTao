using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        public  DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        // GET: Admin/ThongKe
        public ActionResult BaoCaoLopHoc(DateTime? from, DateTime? to, Guid? khoaHocId, LopHoc.TrangThaiLopHoc? trangThai)
        {
            ViewBag.KhoaHocs = db.KhoaHocs.ToList();

            var query = db.LopHocs.Include("BuoiHocs").Include("DangKyHocs").AsQueryable();

            if (from.HasValue)
                query = query.Where(l => l.NgayBatDau >= from);
            if (to.HasValue)
                query = query.Where(l => l.NgayKetThuc <= to);
            if (khoaHocId.HasValue)
                query = query.Where(l => l.KhoaHocId == khoaHocId);
            if (trangThai.HasValue)
                query = query.Where(l => l.TrangThai == trangThai);

            var lopHocs = query.ToList();  // Lấy ra danh sách, truy vấn xuống DB

            var model = lopHocs.Select(l =>
            {
                var tongDiemDanh = db.DiemDanhs_HVs.Count(d => d.BuoiHoc.LopHocId == l.LopHocId);
                var diemDanhCoMat = db.DiemDanhs_HVs.Count(d => d.BuoiHoc.LopHocId == l.LopHocId && d.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.CoMat);

                double tiLeDiemDanh = (double)diemDanhCoMat / Math.Max(1, tongDiemDanh) * 100;

                // Lấy thông tin điểm danh giảng viên cho lớp này
                var tongDiemDanhGV = db.DiemDanhs_GVs.Count(d => d.BuoiHoc.LopHocId == l.LopHocId);
                var diemDanhGVCoMat = db.DiemDanhs_GVs.Count(d => d.BuoiHoc.LopHocId == l.LopHocId && d.TrangThai == DiemDanh_GV.TrangThaiDiemDanhGV.CoMat);
                double tiLeDiemDanhGV = (double)diemDanhGVCoMat / Math.Max(1, tongDiemDanhGV) * 100;

                return new LopHocThongKeViewModel
                {
                    TenLopHoc = l.TenLopHoc,
                    SoHocVien = l.DangKyHocs.Where(dk => dk.IsConfirmed).Select(dk => dk.HocVienId).Distinct().Count(),
                    SoBuoiHoc = l.BuoiHocs.Count(),
                    TiLeDiemDanh = tiLeDiemDanh,
                    TrangThai = l.TrangThai.ToString(),
                    NgayBatDau = l.NgayBatDau,
                    NgayKetThuc = l.NgayKetThuc,
                    GiangVienPhuTrach = db.GiangVien_BuoiHoc
                                        .Where(gb => gb.BuoiHoc.LopHocId == l.LopHocId)
                                        .Select(gb => gb.GiangVien.HoVaTen)
                                        .FirstOrDefault(),

                    SoLanDiemDanhGiangVienCoMat = diemDanhGVCoMat,
                    TongSoLanDiemDanhGiangVien = tongDiemDanhGV,
                    TiLeDiemDanhGiangVien = tiLeDiemDanhGV
                };
            }).ToList();


            return View(model);

        }

    }
}