using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DiemDanhGiangVienController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        [HttpGet]
        public JsonResult LayGiangVienTheoLopHoc(Guid lopHocId)
        {
            var giangviens = db.GiangVien_BuoiHoc
                .Where(g => g.BuoiHoc.LopHocId == lopHocId)
                .Select(g => new {
                    g.GiangVien.GiangVienId,
                    g.GiangVien.HoVaTen
                })
                .Distinct()  // để không lặp Giảng viên nếu dạy nhiều buổi
                .ToList();

            return Json(giangviens, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Index(DateTime? fromDate, DateTime? toDate, Guid? lopHocId, string trangThaiBuoiHoc)
        {
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.SelectedLopHocId = lopHocId;

            var query = db.BuoiHocs
                .Include(b => b.LopHoc)
                .Include(b => b.GiangVien_BuoiHocs.Select(g => g.GiangVien))
                .AsQueryable();

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

            var diemDanhs = db.DiemDanhs_GVs.ToList();

            ViewBag.DiemDanhs = diemDanhs;
            ViewBag.LopHocs = db.LopHocs.ToList();
            ViewBag.KhoaHocList = db.KhoaHocs.ToList();
            // Lấy danh sách khóa học để dropdown
            return View(buoiHocs);
        }

        [HttpPost]
        public JsonResult LuuDiemDanh(Guid buoiHocId, string giangVienId, int trangThai)
        {
            try
            {
                var existing = db.DiemDanhs_GVs
                    .FirstOrDefault(d => d.BuoiHocId == buoiHocId && d.GiangVienId == giangVienId);

                if (existing == null)
                {
                    existing = new DiemDanh_GV
                    {
                        BuoiHocId = buoiHocId,
                        GiangVienId = giangVienId,
                        TrangThai = (DiemDanh_GV.TrangThaiDiemDanhGV)trangThai,
                        NgayDiemDanh = DateTime.Now
                    };
                    db.DiemDanhs_GVs.Add(existing);
                }
                else
                {
                    existing.TrangThai = (DiemDanh_GV.TrangThaiDiemDanhGV)trangThai;
                    existing.NgayDiemDanh = DateTime.Now;
                    db.Entry(existing).State = EntityState.Modified;
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult HuyDiemDanh(Guid buoiHocId, string giangVienId)
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

        // API mới để hủy tất cả điểm danh của 1 buổi học
        [HttpPost]
        public JsonResult HuyTatCaDiemDanh(Guid buoiHocId)
        {
            try
            {
                var danhSachDiemDanh = db.DiemDanhs_GVs.Where(d => d.BuoiHocId == buoiHocId).ToList();
                db.DiemDanhs_GVs.RemoveRange(danhSachDiemDanh);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
