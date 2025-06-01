using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Controllers
{
    public class TraCuuLichHocController : Controller
    {
        // GET: TraCuuLichHoc
        public DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult IndexTheoId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ViewBag.Error = "Dữ liệu học viên không hợp lệ.";
                return View("Index");
            }

            var hocVien = db.HocViens
                            .Include("DangKyHocs.LopHoc.BuoiHocs")
                            .FirstOrDefault(h => h.HocVienId == id);

            if (hocVien == null)
            {
                ViewBag.Error = "Không tìm thấy học viên.";
                return View("Index");
            }

            var buoiHocs = hocVien.DangKyHocs
                                  .Where(dk => dk.IsConfirmed)
                                  .SelectMany(dk => dk.LopHoc.BuoiHocs)
                                  .OrderBy(b => b.NgayHoc)
                                  .ThenBy(b => b.GioBatDau)
                                  .ToList();

            ViewBag.HocVien = hocVien;
            ViewBag.BuoiHocs = buoiHocs;

            // 👉 Quan trọng: return View("Inde

            return View("Index");
        }

        [HttpPost]
        public ActionResult Index(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Vui lòng nhập email học viên.";
                return View();
            }
            var hocVien = db.HocViens
                            .Include("DangKyHocs.LopHoc.BuoiHocs")
                            .FirstOrDefault(h => h.Email == email);
            if (hocVien == null)
            {
                ViewBag.Error = "Không tìm thấy học viên với email này.";
                return View();
            }

            // Lấy tất cả buổi học từ các lớp học đã đăng ký
            var buoiHocs = hocVien.DangKyHocs
                                  .Where(dk => dk.IsConfirmed)
                                  .SelectMany(dk => dk.LopHoc.BuoiHocs)
                                  .OrderBy(b => b.NgayHoc)
                                  .ThenBy(b => b.GioBatDau)
                                  .ToList();

            ViewBag.HocVien = hocVien;
            ViewBag.BuoiHocs = buoiHocs;

            return View();
        }
        [HttpPost]
        public JsonResult QuetQR(string qrData)
        {
            try
            {
                // qrData là chuỗi JSON chứa HocVienId
                var parsed = JsonConvert.DeserializeObject<dynamic>(qrData);
                string hocVienId = parsed.HocVienId;

                if (string.IsNullOrEmpty(hocVienId))
                {
                    return Json(new { success = false, message = "Dữ liệu QR không hợp lệ." });
                }

                var hocVien = db.HocViens
                                .Include("DangKyHocs.LopHoc.BuoiHocs")
                                .FirstOrDefault(h => h.HocVienId == hocVienId);

                if (hocVien == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy học viên." });
                }

                return Json(new
                {
                    success = true,
                    message = "Quét mã thành công!",
                    hocVienId = hocVien.HocVienId
                });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi xử lý dữ liệu QR.", error = ex.Message });
            }
        }
    }
}