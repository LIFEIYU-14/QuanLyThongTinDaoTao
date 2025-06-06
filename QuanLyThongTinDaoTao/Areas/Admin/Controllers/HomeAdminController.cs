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

            var dangKyQuery = db.DangKyHocs
                .Where(dk => dk.NgayDangKy.Year == year)
                .AsQueryable();

            if (lopHocId.HasValue)
            {
                dangKyQuery = dangKyQuery.Where(dk => dk.LopHocId == lopHocId);
            }
            else if (khoaHocId.HasValue)
            {
                var lopIds = db.LopHocs
                    .Where(l => l.KhoaHocId == khoaHocId)
                    .Select(l => l.LopHocId)
                    .ToList();
                dangKyQuery = dangKyQuery.Where(dk => lopIds.Contains(dk.LopHocId));
            }

            var months = Enumerable.Range(1, 12).ToList();
            var labels = months.Select(m => "Tháng " + m).ToList();
            var data = months.Select(m => dangKyQuery.Count(dk => dk.NgayDangKy.Month == m)).ToList();

            return Json(new
            {
                labels = labels,
                data = data,
                soLuongDangKyTheoThang = data // Giữ lại nếu cần dùng cho mục đích khác
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAttendanceStats(Guid? khoaHocId, Guid? lopHocId)
        {
            var year = DateTime.Today.Year;

            var buoiHocQuery = db.BuoiHocs
                .Where(b => b.NgayHoc.Year == year)
                .AsQueryable();

            if (lopHocId.HasValue)
            {
                buoiHocQuery = buoiHocQuery.Where(b => b.LopHocId == lopHocId);
            }
            else if (khoaHocId.HasValue)
            {
                var lopIds = db.LopHocs
                    .Where(l => l.KhoaHocId == khoaHocId)
                    .Select(l => l.LopHocId)
                    .ToList();
                buoiHocQuery = buoiHocQuery.Where(b => lopIds.Contains(b.LopHocId));
            }

            var buoiHocTheoThang = buoiHocQuery
                .GroupBy(b => b.NgayHoc.Month)
                .ToList();

            var dangKyQuery = db.DangKyHocs.AsQueryable();
            if (lopHocId.HasValue)
            {
                dangKyQuery = dangKyQuery.Where(d => d.LopHocId == lopHocId);
            }
            else if (khoaHocId.HasValue)
            {
                var lopIds = db.LopHocs
                    .Where(l => l.KhoaHocId == khoaHocId)
                    .Select(l => l.LopHocId)
                    .ToList();
                dangKyQuery = dangKyQuery.Where(d => lopIds.Contains(d.LopHocId));
            }

            var hocVienIds = dangKyQuery.Any()
                ? dangKyQuery.Select(d => d.HocVienId).Distinct().ToList()
                : db.DangKyHocs.Select(d => d.HocVienId).Distinct().ToList();

            var soLuongHV = hocVienIds.Count;

            var soLuongDiemDanhHV = new List<int>();
            var soLuongDiemDanhGV = new List<int>();

            var tiLeDiemDanhHV = new List<double>();
            var tiLeDiemDanhGV = new List<double>();

            foreach (int thang in Enumerable.Range(1, 12))
            {
                var buoiIds = buoiHocTheoThang.FirstOrDefault(g => g.Key == thang)?
                    .Select(b => b.BuoiHocId)
                    .ToList() ?? new List<Guid>();

                int totalHV = soLuongHV * buoiIds.Count;
                int diemDanhHV = db.DiemDanhs_HVs
                    .Count(dd => buoiIds.Contains(dd.BuoiHocId) && dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.CoMat);

                soLuongDiemDanhHV.Add(diemDanhHV);

                int totalGV = db.GiangVien_BuoiHoc.Count(gb => buoiIds.Contains(gb.BuoiHocId));
                int diemDanhGV = (from dd in db.DiemDanhs_GVs
                                  join gb in db.GiangVien_BuoiHoc on new { dd.BuoiHocId, dd.GiangVienId } equals new { gb.BuoiHocId, gb.GiangVienId }
                                  where buoiIds.Contains(dd.BuoiHocId)
                                        && dd.TrangThai == DiemDanh_GV.TrangThaiDiemDanhGV.CoMat
                                  select dd).Count();

                soLuongDiemDanhGV.Add(diemDanhGV);

                double tileHV = totalHV > 0 ? (double)diemDanhHV / totalHV * 100 : 0;
                double tileGV = totalGV > 0 ? (double)diemDanhGV / totalGV * 100 : 0;

                tiLeDiemDanhHV.Add(Math.Round(tileHV, 2));
                tiLeDiemDanhGV.Add(Math.Round(tileGV, 2));
            }

            // Tính % đăng ký học trên tổng học viên đăng ký trong năm (cho pie chart)
            var totalDangKyTrongNam = dangKyQuery.Count();

            return Json(new
            {
                soLuongDiemDanhHV = soLuongDiemDanhHV,
                soLuongDiemDanhGV = soLuongDiemDanhGV,
                tiLeDiemDanhHV = tiLeDiemDanhHV,
                tiLeDiemDanhGV = tiLeDiemDanhGV,
                soLuongDangKy = totalDangKyTrongNam
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
