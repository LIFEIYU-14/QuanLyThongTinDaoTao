using Newtonsoft.Json;
using QuanLyThongTinDaoTao.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        public ActionResult Index(Guid? khoaHocId, Guid? lopHocId)
        {
            ViewBag.TotalCourses = db.KhoaHocs.Count();
            ViewBag.TotalClasses = db.LopHocs.Count();
            ViewBag.TotalStudents = db.HocViens.Count();
            ViewBag.TotalTeachers = db.GiangViens.Count();

            var today = DateTime.Today;
            var year = today.Year;
            var month = today.Month;

            var dangKyQuery = db.DangKyHocs.AsQueryable();
            if (lopHocId.HasValue)
                dangKyQuery = dangKyQuery.Where(d => d.LopHocId == lopHocId);
            else if (khoaHocId.HasValue)
                dangKyQuery = dangKyQuery.Where(d => db.LopHocs.Any(l => l.KhoaHocId == khoaHocId && l.LopHocId == d.LopHocId));

            ViewBag.RegistrationsToday = dangKyQuery.Count(d => DbFunctions.TruncateTime(d.NgayDangKy) == today);
            ViewBag.RegistrationsThisMonth = dangKyQuery.Count(d => d.NgayDangKy.Month == month && d.NgayDangKy.Year == year);

            var registrationsByMonth = dangKyQuery
                .Where(d => d.NgayDangKy.Year == year)
                .GroupBy(d => d.NgayDangKy.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            var months = Enumerable.Range(1, 12).ToList();
            var monthLabels = months.Select(m => "Tháng " + m).ToList();
            var monthData = months.Select(m => registrationsByMonth.FirstOrDefault(x => x.Month == m)?.Count ?? 0).ToList();

            ViewBag.MonthLabels = JsonConvert.SerializeObject(monthLabels);
            ViewBag.MonthData = JsonConvert.SerializeObject(monthData);

            var lopHocQuery = db.LopHocs.AsQueryable();
            if (khoaHocId.HasValue)
                lopHocQuery = lopHocQuery.Where(l => l.KhoaHocId == khoaHocId);

            var classStatuses = lopHocQuery
                .GroupBy(l => l.TrangThai)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.StatusLabels = JsonConvert.SerializeObject(classStatuses.Select(s => s.Status));
            ViewBag.StatusData = JsonConvert.SerializeObject(classStatuses.Select(s => s.Count));

            ViewBag.KhoaHocList = new SelectList(db.KhoaHocs.ToList(), "KhoaHocId", "TenKhoaHoc", khoaHocId);
            ViewBag.LopHocList = new SelectList(db.LopHocs.Where(l => l.KhoaHocId == khoaHocId).ToList(), "LopHocId", "TenLopHoc", lopHocId);

            return View();
        }

        public JsonResult GetLopHocByKhoaHoc(Guid? khoaHocId)
        {
            if (!khoaHocId.HasValue)
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);

            var lops = db.LopHocs
                .Where(l => l.KhoaHocId == khoaHocId.Value)
                .Select(l => new { l.LopHocId, l.TenLopHoc })
                .ToList();

            return Json(lops, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetRegistrationStats(Guid? khoaHocId, Guid? lopHocId)
        {
            var year = DateTime.Today.Year;
            var dangKyQuery = db.DangKyHocs.AsQueryable();

            if (lopHocId.HasValue)
                dangKyQuery = dangKyQuery.Where(d => d.LopHocId == lopHocId);
            else if (khoaHocId.HasValue)
                dangKyQuery = dangKyQuery.Where(d => db.LopHocs.Any(l => l.KhoaHocId == khoaHocId && l.LopHocId == d.LopHocId));

            var registrationsByMonth = dangKyQuery
                .Where(d => d.NgayDangKy.Year == year)
                .GroupBy(d => d.NgayDangKy.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            var months = Enumerable.Range(1, 12).ToList();
            var counts = months.Select(m => registrationsByMonth.FirstOrDefault(x => x.Month == m)?.Count ?? 0).ToList();

            return Json(new
            {
                labels = months.Select(m => "Tháng " + m),
                data = counts
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAttendanceStats(Guid? khoaHocId, Guid? lopHocId)
        {
            var year = DateTime.Today.Year;

            var buoiHocQuery = db.BuoiHocs.AsQueryable();
            if (lopHocId.HasValue)
                buoiHocQuery = buoiHocQuery.Where(b => b.LopHocId == lopHocId);
            else if (khoaHocId.HasValue)
                buoiHocQuery = buoiHocQuery.Where(b => db.LopHocs.Any(l => l.KhoaHocId == khoaHocId && l.LopHocId == b.LopHocId));

            var buoiHocTheoThang = buoiHocQuery
                .Where(b => b.NgayHoc.Year == year)
                .GroupBy(b => b.NgayHoc.Month)
                .ToList();

            var hvDangKyQuery = db.DangKyHocs.AsQueryable();
            if (lopHocId.HasValue)
                hvDangKyQuery = hvDangKyQuery.Where(d => d.LopHocId == lopHocId);
            else if (khoaHocId.HasValue)
                hvDangKyQuery = hvDangKyQuery.Where(d => db.LopHocs.Any(l => l.KhoaHocId == khoaHocId && l.LopHocId == d.LopHocId));

            var hocVienIds = hvDangKyQuery.Select(d => d.HocVienId).Distinct().ToList();

            var tileHV = new List<double>();
            var tileGV = new List<double>();

            foreach (int thang in Enumerable.Range(1, 12))
            {
                var buoiIds = buoiHocTheoThang
                    .FirstOrDefault(g => g.Key == thang)?
                    .Select(b => b.BuoiHocId)
                    .ToList() ?? new List<Guid>();

                int totalHV = hocVienIds.Count * buoiIds.Count;
                int diemDanhHV = db.DiemDanhs_HVs.Count(dd => buoiIds.Contains(dd.BuoiHocId) && dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.CoMat);

                double tile1 = totalHV > 0 ? (double)diemDanhHV / totalHV * 100 : 0;
                tileHV.Add(Math.Round(tile1, 2));

                int totalGV = db.GiangVien_BuoiHoc.Count(gb => buoiIds.Contains(gb.BuoiHocId));
                int diemDanhGV = db.DiemDanhs_GVs.Count(dd => buoiIds.Contains(dd.BuoiHocId) && dd.TrangThai == DiemDanh_GV.TrangThaiDiemDanhGV.CoMat);

                double tile2 = totalGV > 0 ? (double)diemDanhGV / totalGV * 100 : 0;
                tileGV.Add(Math.Round(tile2, 2));
            }

            return Json(new
            {
                tiLeDiemDanhHV = tileHV,
                tiLeDiemDanhGV = tileGV
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
