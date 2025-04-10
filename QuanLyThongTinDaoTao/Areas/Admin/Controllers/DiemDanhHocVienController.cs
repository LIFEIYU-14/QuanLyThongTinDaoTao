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
        public ActionResult Index()
        {
            // Lấy danh sách lớp học
            var lopHocs = db.LopHocs.ToList();
            ViewBag.LopHocList = lopHocs;

            var danhSachHocVien = db.DangKyHocs
                   .Include(lh => lh.LopHoc)
                   .Include(hv => hv.HocVien)
                   .ToList();
            return View();
        }

        // Lấy danh sách buổi học theo lớp học
        public ActionResult FilterByLopHoc(Guid? lopHocId , string startDate)
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
                .Where(b => b.LopHoc.LopHocId == lopHocId.Value)
                .OrderBy(b => b.NgayHoc)
                .ToList();

            ViewBag.LopHocId = lopHocId.Value;
            return PartialView("_BuoiHocListPartial", buoiHocs);
        }
        // Trang điểm danh chi tiết: hiển thị thông tin buổi học và danh sách học viên
        public ActionResult ThongTinDiemDanh(Guid buoiHocId)
        {
            var buoiHoc = db.BuoiHocs
                .Include(b => b.LopHoc)
                .FirstOrDefault(b => b.BuoiHocId == buoiHocId);
            if (buoiHoc == null)
                return HttpNotFound();

            var danhSachHocVien = db.DangKyHocs
                .Include(dk => dk.HocVien)
                .Where(dk => dk.LopHocId == buoiHoc.LopHoc.LopHocId)
                .ToList();

            var diemDanhs = db.DiemDanhs_HVs
                .Where(dd => dd.BuoiHocId == buoiHocId)
                .ToList();

            ViewBag.BuoiHoc = buoiHoc;
            ViewBag.DiemDanhs = diemDanhs;
            return View(danhSachHocVien);
        }

        // POST: Lưu điểm danh
        [HttpPost]
        public JsonResult LuuDiemDanh(List<DiemDanh_HV> danhSachDiemDanh)
        {
            if (danhSachDiemDanh == null || !danhSachDiemDanh.Any())
            {
                return Json(new { success = false, message = "Không có dữ liệu điểm danh." });
            }

            try
            {
                List<object> updatedAttendance = new List<object>();

                foreach (var diemDanh in danhSachDiemDanh)
                {
                    // Check if attendance for this student and session already exists
                    var existingRecord = db.DiemDanhs_HVs
                        .FirstOrDefault(d => d.NguoiDungId == diemDanh.NguoiDungId && d.BuoiHocId == diemDanh.BuoiHocId);

                    if (existingRecord == null)
                    {
                        // Create a new record if none exists
                        diemDanh.NgayDiemDanh = DateTime.Now; // Set the current date as the attendance date
                        db.DiemDanhs_HVs.Add(diemDanh);
                    }
                    else
                    {
                        // Update existing attendance record
                        existingRecord.TrangThai = diemDanh.TrangThai;
                        db.Entry(existingRecord).State = EntityState.Modified;
                    }

                    updatedAttendance.Add(new
                    {
                        diemDanh.NguoiDungId,
                        diemDanh.BuoiHocId,
                        diemDanh.TrangThai
                    });
                }

                db.SaveChanges(); // Commit the transaction

                return Json(new { success = true, message = "Điểm danh đã được lưu thành công.", updatedAttendance });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi lưu điểm danh. " + ex.Message });
            }
        }
    }
}
