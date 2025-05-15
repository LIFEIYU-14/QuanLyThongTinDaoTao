using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    }
}