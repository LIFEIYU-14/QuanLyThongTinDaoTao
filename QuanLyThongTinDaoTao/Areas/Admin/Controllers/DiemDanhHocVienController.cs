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

        // This action filters students based on the selected class (LopHoc)
        public ActionResult FilterByLopHoc(Guid? lopHocId)
        {
            if (!lopHocId.HasValue)
            {
                // Không chọn lớp học => trả về view rỗng hoặc thông báo
                return PartialView("_HocVienTableEmpty");
            }

            var danhSachHocVien = db.DangKyHocs
                .Include(lh => lh.LopHoc)
                .Include(hv => hv.HocVien)
                .Where(lh => lh.LopHoc.LopHocId == lopHocId.Value)
                .ToList();

            var buoiHocs = db.BuoiHocs
                .Where(b => b.LopHoc.LopHocId == lopHocId.Value)
                .OrderBy(b => b.NgayHoc)
                .ToList();
            // Get the attendance data from DiemDanh_HV
            var diemDanhs = db.DiemDanhs_HVs
                .Where(dd => dd.BuoiHoc.LopHoc.LopHocId == lopHocId.Value)
                .ToList();

            ViewBag.BuoiHocs = buoiHocs;
            ViewBag.DiemDanhs = diemDanhs;

            return PartialView("_HocVienTablePartial", danhSachHocVien);
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
