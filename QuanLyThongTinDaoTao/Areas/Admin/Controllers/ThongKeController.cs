// Controller: ThongKeController.cs
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        public DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        public ActionResult BaoCaoLopHoc()
        {
            ViewBag.KhoaHocList = new SelectList(db.KhoaHocs.ToList(), "KhoaHocId", "TenKhoaHoc");
            return View();
        }

        public JsonResult GetLopHocByKhoaHoc(Guid khoaHocId)
        {
            var lops = db.LopHocs
                .Where(l => l.KhoaHocId == khoaHocId)
                .Select(l => new { l.LopHocId, l.TenLopHoc })
                .ToList();
            return Json(lops, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetThongKeBuoiHocTheoLop(Guid lopHocId)
        {
            var buoiHocs = db.BuoiHocs
                .Where(b => b.LopHocId == lopHocId)
                .OrderBy(b => b.NgayHoc)
                .ThenBy(b => b.GioBatDau)
                .ToList();

            int tongDangKy = db.DangKyHocs.Count(d => d.LopHocId == lopHocId && d.IsConfirmed);

            var result = buoiHocs.Select(b =>
            {
                int soCoMat = db.DiemDanhs_HVs.Count(dd => dd.BuoiHocId == b.BuoiHocId && dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.CoMat);
                int tongGv = db.GiangVien_BuoiHoc.Count(gv => gv.BuoiHocId == b.BuoiHocId);
                int soGvCoMat = db.DiemDanhs_GVs.Count(ddgv => ddgv.BuoiHocId == b.BuoiHocId && ddgv.TrangThai == DiemDanh_GV.TrangThaiDiemDanhGV.CoMat);

                // Giới hạn không vượt tổng
                soCoMat = Math.Min(soCoMat, tongDangKy);
                soGvCoMat = Math.Min(soGvCoMat, tongGv);

                double tiLeHV = tongDangKy > 0 ? (double)soCoMat / tongDangKy : 0;
                double tiLeGV = tongGv > 0 ? (double)soGvCoMat / tongGv : 0;

                return new
                {
                    BuoiHocId = b.BuoiHocId,
                    NgayHoc = b.NgayHoc.ToString("dd/MM/yyyy"),
                    GioBatDau = b.GioBatDau.ToString(@"hh\:mm"),
                    GioKetThuc = b.GioKetThuc.ToString(@"hh\:mm"),
                    TongDangKy = tongDangKy,
                    SoCoMat = soCoMat,
                    TiLe = Math.Round(tiLeHV * 100, 2),   // phần trăm HV
                    TiLeGV = Math.Round(tiLeGV * 100, 2)  // phần trăm GV
                };
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
