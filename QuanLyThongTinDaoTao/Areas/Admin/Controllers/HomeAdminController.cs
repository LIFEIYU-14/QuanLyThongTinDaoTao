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
        public DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        public ActionResult Index(Guid? khoaHocId, Guid? lopHocId)
        {
            // Thống kê tổng (không phụ thuộc bộ lọc)
            ViewBag.TotalCourses = db.KhoaHocs.Count();
            ViewBag.TotalClasses = db.LopHocs.Count();
            ViewBag.TotalRegistrations = db.DangKyHocs.Count();
            ViewBag.TotalStudents = db.HocViens.Count();
            ViewBag.TotalTeachers = db.GiangViens.Count();

            var today = DateTime.Today;
            var month = today.Month;
            var year = today.Year;

            var dangKyQuery = db.DangKyHocs.AsQueryable();
            if (lopHocId.HasValue)
            {
                dangKyQuery = dangKyQuery.Where(d => d.LopHocId == lopHocId);
            }
            else if (khoaHocId.HasValue)
            {
                var lopIds = db.LopHocs.Where(l => l.KhoaHocId == khoaHocId).Select(l => l.LopHocId).ToList();
                dangKyQuery = dangKyQuery.Where(d => lopIds.Contains(d.LopHocId));
            }

            var registrationsToday = dangKyQuery.Count(d => DbFunctions.TruncateTime(d.NgayDangKy) == today);
            var registrationsThisMonth = dangKyQuery.Count(d => d.NgayDangKy.Month == month && d.NgayDangKy.Year == year);
            var registrationsThisYear = dangKyQuery.Count(d => d.NgayDangKy.Year == year);

            var registrationsByMonth = dangKyQuery
                .Where(d => d.NgayDangKy.Year == year)
                .GroupBy(d => d.NgayDangKy.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            var months = Enumerable.Range(1, 12).ToList();
            var monthlyCounts = months.Select(m => registrationsByMonth.FirstOrDefault(x => x.Month == m)?.Count ?? 0).ToList();

            var lopHocQuery = db.LopHocs.AsQueryable();
            if (khoaHocId.HasValue)
                lopHocQuery = lopHocQuery.Where(l => l.KhoaHocId == khoaHocId);

            var classStatuses = lopHocQuery
                .GroupBy(l => l.TrangThai)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.RegistrationsToday = registrationsToday;
            ViewBag.RegistrationsThisMonth = registrationsThisMonth;
            ViewBag.RegistrationsThisYear = registrationsThisYear;

            ViewBag.MonthLabels = JsonConvert.SerializeObject(months.Select(m => "Tháng " + m));
            ViewBag.MonthData = JsonConvert.SerializeObject(monthlyCounts);

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
        [HttpGet]
        public JsonResult GetRegistrationStats(Guid? khoaHocId, Guid? lopHocId, string startDate, string endDate)
        {
            var today = DateTime.Today;
            var year = today.Year;

            DateTime? start = null;
            DateTime? end = null;

            // Parse ngày tháng truyền vào nếu có
            if (DateTime.TryParse(startDate, out DateTime sd))
                start = sd;

            if (DateTime.TryParse(endDate, out DateTime ed))
                end = ed;

            var dangKyQuery = db.DangKyHocs.AsQueryable();

            if (lopHocId.HasValue)
            {
                dangKyQuery = dangKyQuery.Where(d => d.LopHocId == lopHocId);
            }
            else if (khoaHocId.HasValue)
            {
                var lopIds = db.LopHocs.Where(l => l.KhoaHocId == khoaHocId).Select(l => l.LopHocId).ToList();
                dangKyQuery = dangKyQuery.Where(d => lopIds.Contains(d.LopHocId));
            }
            else
            {
                // Nếu không chọn gì, lấy toàn bộ dữ liệu
                dangKyQuery = db.DangKyHocs;
            }

            // Lọc theo năm hiện tại nếu không có ngày bắt đầu kết thúc
            if (!start.HasValue && !end.HasValue)
            {
                dangKyQuery = dangKyQuery.Where(d => d.NgayDangKy.Year == year);
            }
            else
            {
                if (start.HasValue)
                    dangKyQuery = dangKyQuery.Where(d => d.NgayDangKy >= start.Value);

                if (end.HasValue)
                    dangKyQuery = dangKyQuery.Where(d => d.NgayDangKy <= end.Value);
            }

            var registrationsByMonth = dangKyQuery
                .GroupBy(d => d.NgayDangKy.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            var months = Enumerable.Range(1, 12).ToList();
            var monthlyCounts = months.Select(m => registrationsByMonth.FirstOrDefault(x => x.Month == m)?.Count ?? 0).ToList();

            return Json(new
            {
                labels = months.Select(m => "Tháng " + m),
                data = monthlyCounts
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
