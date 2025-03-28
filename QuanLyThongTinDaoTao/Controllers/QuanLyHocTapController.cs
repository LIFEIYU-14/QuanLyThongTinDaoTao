using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Controllers
{
    public class QuanLyHocTapController : Controller
    {
        public DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        // GET: QuanLyHocTap
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LopHocDaDangKy()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy email của người dùng đang đăng nhập
            string email = User.Identity.Name;

            // Tìm học viên theo email
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (hocVien == null)
            {
                return HttpNotFound("Không tìm thấy thông tin học viên.");
            }

            // Lấy danh sách lớp học mà học viên đã đăng ký
            var lopHocsDaDangKy = db.DangKyHocs
                .Where(dk => dk.NguoiDungId == hocVien.NguoiDungId)
                .Select(dk => dk.LopHoc)
                .ToList();

            return View(lopHocsDaDangKy);
        }

        public ActionResult LichHoc(string startDate)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy email của học viên đang đăng nhập
            string email = User.Identity.Name;

            // Tìm học viên theo email
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (hocVien == null)
            {
                return HttpNotFound("Không tìm thấy thông tin học viên.");
            }

            // Lấy danh sách lớp học mà học viên đã đăng ký (lấy cả ID của lớp)
            var lopHocsDaDangKy = db.DangKyHocs
                .Where(dk => dk.NguoiDungId == hocVien.NguoiDungId)
                .Select(dk => dk.LopHoc.LopHocId)
                .ToList();

            // Lấy danh sách buổi học của các lớp học đã đăng ký
            var lichHoc = db.BuoiHocs
                .Where(bh => lopHocsDaDangKy.Contains(bh.LopHoc.LopHocId))
                .OrderBy(bh => bh.NgayHoc)
                .ThenBy(bh => bh.GioBatDau)
                .ToList();

            // Lọc theo tuần dựa vào startDate (nếu có)
            DateTime weekStart;
            if (!DateTime.TryParse(startDate, out weekStart))
            {
                weekStart = DateTime.Today;
            }
            // Chuyển về thứ Hai (nếu Sunday thì xem như cuối tuần của tuần trước)
            int dow = (int)weekStart.DayOfWeek;
            dow = (dow == 0) ? 7 : dow;
            weekStart = weekStart.AddDays(1 - dow);
            DateTime weekEnd = weekStart.AddDays(7);
            lichHoc = lichHoc.Where(bh => bh.NgayHoc >= weekStart && bh.NgayHoc < weekEnd).ToList();

            // Nếu request là AJAX, trả về PartialView
            if (Request.IsAjaxRequest())
            {
                return PartialView("_LichHocPartial", lichHoc);
            }
            return View(lichHoc);
        }
    }
}
