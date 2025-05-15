using Microsoft.AspNet.Identity;
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
                   .Include(lh => lh.LopHoc)
                   .Include(hv => hv.HocVien)
                   .ToList();
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
        public JsonResult HuyDiemDanhTatCa(Guid buoiHocId)
        {
            try
            {
                var records = db.DiemDanhs_HVs.Where(d => d.BuoiHocId == buoiHocId).ToList();
                if (!records.Any())
                    return Json(new { success = false, message = "Không có học viên nào đã điểm danh." });

                db.DiemDanhs_HVs.RemoveRange(records);
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
