using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    [RoleAuthorize("Admin")]
    public class DiemDanhGiangVienController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        public ActionResult Index(DateTime? fromDate, DateTime? toDate, Guid? lopHocId, string trangThaiBuoiHoc)
        {
            // Lưu giá trị filter vào ViewBag để giữ lại khi reload trang
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.SelectedLopHocId = lopHocId;
            // Lấy danh sách buổi học với giảng viên được phân công
            var query = db.BuoiHocs
                .Include(b => b.LopHoc)
                .Include(b => b.GiangVien_BuoiHocs.Select(g => g.GiangVien))
                .AsQueryable();

            // Áp dụng bộ lọc
            if (fromDate.HasValue)
                query = query.Where(b => b.NgayHoc >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(b => b.NgayHoc <= toDate.Value);

            if (lopHocId.HasValue)
                query = query.Where(b => b.LopHoc.LopHocId == lopHocId.Value);

            if (!string.IsNullOrEmpty(trangThaiBuoiHoc))
            {
                var status = (TrangThaiBuoiHoc)Enum.Parse(typeof(TrangThaiBuoiHoc), trangThaiBuoiHoc);
                query = query.Where(b => b.TrangThai == status);
            }

            var buoiHocs = query.OrderBy(b => b.NgayHoc)
                               .ThenBy(b => b.GioBatDau)
                               .ToList();

            // Lấy tất cả điểm danh để hiển thị
            var diemDanhs = db.DiemDanhs_GVs.ToList();

            ViewBag.DiemDanhs = diemDanhs;
            ViewBag.LopHocs = db.LopHocs.ToList();
            return View(buoiHocs);
        }


        [HttpPost]
        public JsonResult LuuDiemDanh(List<DiemDanh_GV> danhSachDiemDanh)
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
                    var existingRecord = db.DiemDanhs_GVs
                        .FirstOrDefault(d => d.NguoiDungId == diemDanh.NguoiDungId && d.BuoiHocId == diemDanh.BuoiHocId);

                    if (existingRecord == null)
                    {
                        diemDanh.NgayDiemDanh = DateTime.Now;
                        db.DiemDanhs_GVs.Add(diemDanh);
                    }
                    else
                    {
                        existingRecord.TrangThai = diemDanh.TrangThai;
                        existingRecord.NgayDiemDanh = DateTime.Now;
                        db.Entry(existingRecord).State = EntityState.Modified;
                    }

                    updatedAttendance.Add(new
                    {
                        diemDanh.NguoiDungId,
                        diemDanh.BuoiHocId,
                        diemDanh.TrangThai
                    });
                }

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Điểm danh đã được lưu thành công.",
                    updatedAttendance
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lưu điểm danh: " + ex.Message
                });
            }
        }

    }
}