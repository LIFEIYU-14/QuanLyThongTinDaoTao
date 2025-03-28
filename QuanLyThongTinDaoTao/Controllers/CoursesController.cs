using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Controllers
{
    public class CoursesController : Controller
    {
        public DbContextThongTinDaoTao db = new DbContextThongTinDaoTao(); // Khai báo DbContext

        // GET: Courses
        public ActionResult Index(int? page)
        {
            int pageSize = 12; // Số lượng khóa học trên mỗi trang
            int pageNumber = (page ?? 1); // Trang mặc định là trang 1

            var khoaHocs = db.KhoaHocs.Include(k => k.KhoaHocAttachments)
                                       .OrderByDescending(k => k.NgayTao)
                                       .ToPagedList(pageNumber, pageSize);

            return View(khoaHocs);
        }

        // Danh sách lớp học của khóa học
        public ActionResult DanhSachLopHoc(Guid id)
        {
            var khoaHoc = db.KhoaHocs
                .Include(k => k.LopHocs)  // Load danh sách lớp học
                .Include(k => k.KhoaHocAttachments) // Load file đính kèm khóa học
                .FirstOrDefault(k => k.KhoaHocId == id);

            if (khoaHoc == null)
            {
                return HttpNotFound(); // Nếu khóa học không tồn tại
            }

            ViewBag.KhoaHoc = khoaHoc; // Truyền thông tin khóa học vào ViewBag

            var lopHocs = khoaHoc.LopHocs?.ToList() ?? new List<LopHoc>(); // Tránh null nếu không có lớp học
                                                                         
            if (User.Identity.IsAuthenticated)
            {
                string email = User.Identity.Name;
                var hocVien = db.HocViens.FirstOrDefault(hv => hv.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                ViewBag.HocVien = hocVien; // Pass hocVien to ViewBag
                                         
                foreach (var lopHoc in lopHocs)
                {
                    lopHoc.IsRegistered = db.DangKyHocs.Any(dk => dk.LopHocId == lopHoc.LopHocId && dk.NguoiDungId == hocVien.NguoiDungId);
                }
            }
            return View(lopHocs);
        }

        // Đăng ký lớp học

        public ActionResult Register(Guid lopHocId)
        {
            // Đảm bảo người dùng đã đăng nhập
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            // Lấy email của người dùng đã đăng nhập
            string email = User.Identity.Name;

            // Tìm lớp học theo lopHocId
            var lopHoc = db.LopHocs.FirstOrDefault(l => l.LopHocId == lopHocId);
            if (lopHoc == null)
            {
                return HttpNotFound("Lớp học không tồn tại.");
            }

            // Tìm học viên theo email
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (hocVien == null)
            {
                return HttpNotFound("Không tìm thấy thông tin học viên.");
            }

            // Kiểm tra xem học viên đã đăng ký lớp học này chưa
            var daDangKy = db.DangKyHocs.Any(dk => dk.LopHocId == lopHoc.LopHocId && dk.NguoiDungId == hocVien.NguoiDungId);
            if (daDangKy)
            {
                TempData["Message"] = "Bạn đã đăng ký lớp học này rồi.";
                return RedirectToAction("DanhSachLopHoc", new { id = lopHoc.KhoaHoc.KhoaHocId });
            }

            // Kiểm tra lớp học có đủ số lượng học viên không
            if (lopHoc.HocViens.Count >= lopHoc.SoLuongToiDa)
            {
                TempData["Message"] = "Lớp học đã đầy.";
                return RedirectToAction("DanhSachLopHoc", new { id = lopHoc.KhoaHoc.KhoaHocId });
            }

            // Thêm mới đăng ký lớp học
            var dangKy = new DangKyHoc
            {
                NguoiDungId = hocVien.NguoiDungId,
                LopHocId = lopHoc.LopHocId,
                KhoaHocId = lopHoc.KhoaHoc.KhoaHocId,
                NgayDangKy = DateTime.Now
            };

            db.DangKyHocs.Add(dangKy);
            db.SaveChanges();

            TempData["Message"] = "Đăng ký lớp học thành công!";
            return RedirectToAction("DanhSachLopHoc", new { id = lopHoc.KhoaHoc.KhoaHocId });
        }
        // Hủy đăng ký lớp học
        public ActionResult CancelRegister(Guid lopHocId)
        {
            // Đảm bảo người dùng đã đăng nhập
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            // Lấy email của người dùng đã đăng nhập
            string email = User.Identity.Name;

            // Tìm lớp học theo lopHocId
            var lopHoc = db.LopHocs.FirstOrDefault(l => l.LopHocId == lopHocId);
            if (lopHoc == null)
            {
                return HttpNotFound("Lớp học không tồn tại.");
            }

            // Tìm học viên theo email
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (hocVien == null)
            {
                return HttpNotFound("Không tìm thấy thông tin học viên.");
            }

            // Tìm đăng ký của học viên với lớp học
            var dangKy = db.DangKyHocs.FirstOrDefault(dk => dk.LopHocId == lopHoc.LopHocId && dk.NguoiDungId == hocVien.NguoiDungId);
            if (dangKy == null)
            {
                TempData["Message"] = "Bạn chưa đăng ký lớp học này.";
                return RedirectToAction("DanhSachLopHoc", new { id = lopHoc.KhoaHoc.KhoaHocId });
            }

            // Xóa đăng ký lớp học
            db.DangKyHocs.Remove(dangKy);
            db.SaveChanges();

            TempData["Message"] = "Hủy đăng ký lớp học thành công!";
            return RedirectToAction("DanhSachLopHoc", new { id = lopHoc.KhoaHoc.KhoaHocId });
        }
    }
}
