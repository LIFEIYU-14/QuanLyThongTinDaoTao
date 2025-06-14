﻿using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using QuanLyThongTinDaoTao.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class DiemDanhHocVienController : Controller
    {
        private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        // GET: Admin/DiemDanhHocVien
        [Authorize]
        public ActionResult DanhSachBuoiDay()
        {
            var userId = User.Identity.GetUserId();

            if (User.IsInRole("GiangVien"))
            {
                var buoiHoc = db.GiangVien_BuoiHoc
                                .Where(gb => gb.GiangVien.AppUserId == userId)
                                .Select(gb => gb.BuoiHoc)
                                .Include(bh => bh.LopHoc.KhoaHoc)
                                .ToList();

                return View("DanhSachBuoiDayGV", buoiHoc);
            }

            return View("DanhSachTatCaBuoiHoc", db.BuoiHocs.ToList());
        }
        public ActionResult Index()
        {
            // Lấy danh sách lớp học
            var lopHocs = db.LopHocs.ToList();
            ViewBag.LopHocList = lopHocs;
            ViewBag.KhoaHocList = db.KhoaHocs.ToList();

            var danhSachHocVien = db.DangKyHocs
                   .Include(dk => dk.LopHoc)
                   .Include(dk => dk.HocVien)
                   .Include(dk => dk.LopHoc.BuoiHocs.Select(bh => bh.GiangVien_BuoiHocs.Select(gvbh => gvbh.GiangVien)))
                   .ToList();
            ViewBag.KhoaHocList = new SelectList(db.KhoaHocs.ToList(), "KhoaHocId", "TenKhoaHoc");
            return View(danhSachHocVien);
        }
        // Lấy danh sách lớp theo khóa học
        public ActionResult GetLopHocTheoKhoaHoc(Guid? khoaHocId)
        {
            var lopHocs = db.LopHocs
                .Where(lh => lh.KhoaHocId == khoaHocId)
                .Select(lh => new { lh.LopHocId, lh.TenLopHoc })
                .ToList();
            return Json(lopHocs, JsonRequestBehavior.AllowGet);
        }

        // Lấy danh sách buổi học theo lớp học
        public ActionResult FilterByLopHoc(Guid? lopHocId, string startDate)
        {
            if (!lopHocId.HasValue)
                return PartialView("_BuoiHocEmpty");
            DateTime parsedStartDate;
            if (!DateTime.TryParse(startDate, out parsedStartDate))
            {
                parsedStartDate = DateTime.Today;
                int dow = (int)parsedStartDate.DayOfWeek;
                dow = dow == 0 ? 7 : dow;
                parsedStartDate = parsedStartDate.AddDays(1 - dow);
            }

            var buoiHocs = db.BuoiHocs
                    .Include(b => b.LopHoc.KhoaHoc)
                    .Where(b => b.LopHoc.LopHocId == lopHocId.Value)
                    .OrderBy(b => b.NgayHoc)
                    .ToList();

            ViewBag.LopHocId = lopHocId.Value;
            return PartialView("_BuoiHocListPartial", buoiHocs);
        }
        // Trang điểm danh chi tiết: hiển thị thông tin buổi học và danh sách học viên, giảng viên
        public ActionResult ThongTinDiemDanh(Guid buoiHocId)
        {
            var buoiHoc = db.BuoiHocs
                .Include(b => b.LopHoc)
                .FirstOrDefault(b => b.BuoiHocId == buoiHocId);
            if (buoiHoc == null)
                return HttpNotFound();

            // Danh sách học viên đã đăng ký lớp của buổi học
            var danhSachHocVien = db.DangKyHocs
                .Include(dk => dk.HocVien)
                .Where(dk => dk.LopHocId == buoiHoc.LopHoc.LopHocId)
                .ToList();

            // Danh sách học viên đã được điểm danh
            var diemDanhs = db.DiemDanhs_HVs
                .Where(dd => dd.BuoiHocId == buoiHocId)
                .ToList();

            // Danh sách giảng viên đã điểm danh
            var diemDanhsGV = db.DiemDanhs_GVs
                .Include(d => d.GiangVien)
                .Where(d => d.BuoiHocId == buoiHocId)
                .ToList();

            // Danh sách tất cả giảng viên tham gia buổi học
            var tatCaGiangVien = db.GiangVien_BuoiHoc
                .Include(g => g.GiangVien)
                .Where(g => g.BuoiHocId == buoiHocId)
                .Select(g => g.GiangVien)
                .Distinct()
                .ToList();

            // Danh sách ID giảng viên đã điểm danh (để kiểm tra trạng thái trong view)
            var danhSachIdDaDiemDanh = diemDanhsGV.Select(gv => gv.GiangVienId).ToList();

            // Gán ViewBag để truyền dữ liệu sang View
            ViewBag.BuoiHoc = buoiHoc;
            ViewBag.DiemDanhs = diemDanhs;
            ViewBag.GiangVienDiemDanh = diemDanhsGV;
            ViewBag.TatCaGiangVien = tatCaGiangVien;
            ViewBag.DanhSachIdDaDiemDanh = danhSachIdDaDiemDanh;

            return View(danhSachHocVien);
        }



        // POST: Lưu điểm danh
        [HttpPost]
        public JsonResult DiemDanhHocVien(string hocVienId, Guid buoiHocId)
        {
            try
            {
                var existing = db.DiemDanhs_HVs.FirstOrDefault(d => d.HocVienId == hocVienId && d.BuoiHocId == buoiHocId);
                if (existing != null)
                {
                    return Json(new { success = false, message = "Học viên đã được điểm danh." });
                }

                var record = new DiemDanh_HV
                {
                    DiemDanhId = Guid.NewGuid(),
                    HocVienId = hocVienId,
                    BuoiHocId = buoiHocId,
                    TrangThai = DiemDanh_HV.TrangThaiDiemDanhHV.CoMat,
                    NgayDiemDanh = DateTime.Now
                };

                db.DiemDanhs_HVs.Add(record);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        // POST: Lưu điểm danh
        [HttpPost]
        public JsonResult DiemDanhGiangVien(string giangVienId, Guid buoiHocId)
        {
            try
            {
                var existing = db.DiemDanhs_GVs.FirstOrDefault(d => d.GiangVienId == giangVienId && d.BuoiHocId == buoiHocId);
                if (existing != null)
                {
                    return Json(new { success = false, message = "Giảng viên đã được điểm danh." });
                }

                var record = new DiemDanh_GV
                {
                    DiemDanhId = Guid.NewGuid(),
                    GiangVienId = giangVienId,
                    BuoiHocId = buoiHocId,
                    TrangThai = DiemDanh_GV.TrangThaiDiemDanhGV.CoMat,
                    NgayDiemDanh = DateTime.Now
                };

                db.DiemDanhs_GVs.Add(record);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult HuyDiemDanhHocVien(string hocVienId, Guid buoiHocId)
        {
            try
            {
                var record = db.DiemDanhs_HVs.FirstOrDefault(d => d.HocVienId == hocVienId && d.BuoiHocId == buoiHocId);
                if (record == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi điểm danh." });
                }

                db.DiemDanhs_HVs.Remove(record);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult HuyDiemDanhGiangVien(string giangVienId, Guid buoiHocId)
        {
            try
            {
                var record = db.DiemDanhs_GVs.FirstOrDefault(d => d.GiangVienId == giangVienId && d.BuoiHocId == buoiHocId);
                if (record == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi điểm danh." });
                }

                db.DiemDanhs_GVs.Remove(record);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult DiemDanhBangQr(string qrData)
        {
            try
            {
                var parsed = JsonConvert.DeserializeObject<dynamic>(qrData);
                Guid buoiHocId = Guid.Parse((string)parsed.buoiHocId ?? "");

                // Nếu là học viên
                if (parsed.HocVienId != null)
                {
                    string hocVienId = parsed.HocVienId;
                    bool daDiemDanh = db.DiemDanhs_HVs.Any(dd => dd.HocVienId == hocVienId && dd.BuoiHocId == buoiHocId);

                    if (daDiemDanh)
                        return Json(new { success = false, message = "Học viên đã được điểm danh!" });

                    var diemDanh = new DiemDanh_HV
                    {
                        DiemDanhId = Guid.NewGuid(),
                        HocVienId = hocVienId,
                        BuoiHocId = buoiHocId,
                        NgayDiemDanh = DateTime.Now,
                        TrangThai = DiemDanh_HV.TrangThaiDiemDanhHV.CoMat
                    };
                    db.DiemDanhs_HVs.Add(diemDanh);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Điểm danh học viên thành công!" });
                }
                // Nếu là giảng viên
                else if (parsed.GiangVienId != null)
                {
                    string giangVienId = parsed.GiangVienId;
                    bool daDiemDanh = db.DiemDanhs_GVs.Any(dd => dd.GiangVienId == giangVienId && dd.BuoiHocId == buoiHocId);

                    if (daDiemDanh)
                        return Json(new { success = false, message = "Giảng viên đã được điểm danh!" });

                    var diemDanhGV = new DiemDanh_GV
                    {
                        DiemDanhId = Guid.NewGuid(),
                        GiangVienId = giangVienId,
                        BuoiHocId = buoiHocId,
                        NgayDiemDanh = DateTime.Now,
                        TrangThai = DiemDanh_GV.TrangThaiDiemDanhGV.CoMat
                    };
                    db.DiemDanhs_GVs.Add(diemDanhGV);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Điểm danh giảng viên thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "QR không chứa mã hợp lệ!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi xử lý QR!", error = ex.Message });
            }
        }

    }
}
