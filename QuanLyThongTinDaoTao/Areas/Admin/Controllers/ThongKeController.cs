// Controller: ThongKeController.cs
using OfficeOpenXml;
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
        public JsonResult GetThongKeBuoiHocTheoKhoaHoc(Guid khoaHocId, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = db.BuoiHocs
            .Where(b => b.LopHoc.KhoaHocId == khoaHocId);

            query = LocTheoNgay(query, tuNgay, denNgay);

            var buoiHocs = query
                .OrderBy(b => b.NgayHoc)
                .ThenBy(b => b.GioBatDau)
                .ToList();

            var result = buoiHocs.Select(b =>
            {
                int tongDangKy = db.DangKyHocs.Count(d => d.LopHocId == b.LopHocId && d.IsConfirmed);
                int soCoMat = db.DiemDanhs_HVs.Count(dd => dd.BuoiHocId == b.BuoiHocId && dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.CoMat);
                int tongGv = db.GiangVien_BuoiHoc.Count(gv => gv.BuoiHocId == b.BuoiHocId);
                int soGvCoMat = db.DiemDanhs_GVs.Count(ddgv => ddgv.BuoiHocId == b.BuoiHocId && ddgv.TrangThai == DiemDanh_GV.TrangThaiDiemDanhGV.CoMat);

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
                    TiLe = Math.Round(tiLeHV * 100, 2),
                    TiLeGV = Math.Round(tiLeGV * 100, 2),
                    TenLopHoc = b.LopHoc.TenLopHoc,
                    TenKhoaHoc = b.LopHoc.KhoaHoc.TenKhoaHoc

                };
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetThongKeTatCaBuoiHoc(DateTime? tuNgay, DateTime? denNgay)
        {
            var query = db.BuoiHocs.AsQueryable();
            query = LocTheoNgay(query, tuNgay, denNgay);

            var buoiHocs = query
                .OrderBy(b => b.NgayHoc)
                .ThenBy(b => b.GioBatDau)
                .ToList();

            var result = buoiHocs.Select(b =>
            {
                int tongDangKy = db.DangKyHocs.Count(d => d.LopHocId == b.LopHocId && d.IsConfirmed);
                int soCoMat = db.DiemDanhs_HVs.Count(dd => dd.BuoiHocId == b.BuoiHocId && dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.CoMat);
                int tongGv = db.GiangVien_BuoiHoc.Count(gv => gv.BuoiHocId == b.BuoiHocId);
                int soGvCoMat = db.DiemDanhs_GVs.Count(ddgv => ddgv.BuoiHocId == b.BuoiHocId && ddgv.TrangThai == DiemDanh_GV.TrangThaiDiemDanhGV.CoMat);

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
                    TiLe = Math.Round(tiLeHV * 100, 2),
                    TiLeGV = Math.Round(tiLeGV * 100, 2),
                    TenLopHoc = b.LopHoc.TenLopHoc,
                    TenKhoaHoc = b.LopHoc.KhoaHoc.TenKhoaHoc
                };
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetThongKeBuoiHocTheoLop(Guid lopHocId, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = db.BuoiHocs.Where(b => b.LopHocId == lopHocId);
            query = LocTheoNgay(query, tuNgay, denNgay);

            var buoiHocs = query
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
                    TiLeGV = Math.Round(tiLeGV * 100, 2), // phần trăm GV
                    TenLopHoc = b.LopHoc.TenLopHoc,
                    TenKhoaHoc = b.LopHoc.KhoaHoc.TenKhoaHoc
                };
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private IQueryable<BuoiHoc> LocTheoNgay(IQueryable<BuoiHoc> query, DateTime? tuNgay, DateTime? denNgay)
        {
            if (tuNgay.HasValue)
                query = query.Where(b => b.NgayHoc >= tuNgay.Value);
            if (denNgay.HasValue)
                query = query.Where(b => b.NgayHoc <= denNgay.Value);
            return query;
        }

        public ActionResult XuatExcel(Guid? khoaHocId, Guid? lopHocId, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = db.BuoiHocs.AsQueryable();

            if (lopHocId.HasValue)
                query = query.Where(b => b.LopHocId == lopHocId.Value);
            else if (khoaHocId.HasValue)
                query = query.Where(b => b.LopHoc.KhoaHocId == khoaHocId.Value);

            query = LocTheoNgay(query, tuNgay, denNgay);

            var data = query.OrderBy(b => b.NgayHoc).ThenBy(b => b.GioBatDau).ToList();

            // Lấy tên khóa học và lớp học để hiển thị trong tiêu đề Excel
            string tenKhoaHoc = khoaHocId.HasValue
                ? db.KhoaHocs.Where(k => k.KhoaHocId == khoaHocId.Value).Select(k => k.TenKhoaHoc).FirstOrDefault()
                : "Tất cả khóa học";

            string tenLopHoc = lopHocId.HasValue
                ? db.LopHocs.Where(l => l.LopHocId == lopHocId.Value).Select(l => l.TenLopHoc).FirstOrDefault()
                : "Tất cả lớp học";

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("ThongKe");

                int headerRow = 1;

                // Ghi thông tin bộ lọc vào đầu file
                sheet.Cells[headerRow, 1].Value = "Thông tin bộ lọc";
                sheet.Cells[headerRow, 1, headerRow, 2].Merge = true;
                sheet.Cells[headerRow++, 1].Style.Font.Bold = true;

                sheet.Cells[headerRow, 1].Value = "Khóa học:";
                sheet.Cells[headerRow++, 2].Value = tenKhoaHoc;

                sheet.Cells[headerRow, 1].Value = "Lớp học:";
                sheet.Cells[headerRow++, 2].Value = tenLopHoc;

                if (tuNgay.HasValue)
                {
                    sheet.Cells[headerRow, 1].Value = "Từ ngày:";
                    sheet.Cells[headerRow++, 2].Value = tuNgay.Value.ToString("dd/MM/yyyy");
                }

                if (denNgay.HasValue)
                {
                    sheet.Cells[headerRow, 1].Value = "Đến ngày:";
                    sheet.Cells[headerRow++, 2].Value = denNgay.Value.ToString("dd/MM/yyyy");
                }

                // Ghi tiêu đề bảng dữ liệu
                int dataHeaderRow = headerRow + 1;
                string[] headers = {
            "Khóa học", "Lớp học", "Ngày học", "Giờ bắt đầu", "Giờ kết thúc",
            "Tổng đăng ký", "Số có mặt (HV)", "Tỷ lệ điểm danh (HV)", "Tỷ lệ điểm danh (GV)"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    sheet.Cells[dataHeaderRow, i + 1].Value = headers[i];
                    sheet.Cells[dataHeaderRow, i + 1].Style.Font.Bold = true;
                    sheet.Cells[dataHeaderRow, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Ghi dữ liệu thống kê bắt đầu từ dòng tiếp theo
                int row = dataHeaderRow + 1;
                foreach (var b in data)
                {
                    int tongDangKy = db.DangKyHocs.Count(d => d.LopHocId == b.LopHocId && d.IsConfirmed);
                    int soCoMat = db.DiemDanhs_HVs.Count(dd => dd.BuoiHocId == b.BuoiHocId && dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.CoMat);
                    int tongGv = db.GiangVien_BuoiHoc.Count(gv => gv.BuoiHocId == b.BuoiHocId);
                    int soGvCoMat = db.DiemDanhs_GVs.Count(ddgv => ddgv.BuoiHocId == b.BuoiHocId && ddgv.TrangThai == DiemDanh_GV.TrangThaiDiemDanhGV.CoMat);

                    soCoMat = Math.Min(soCoMat, tongDangKy);
                    soGvCoMat = Math.Min(soGvCoMat, tongGv);

                    double tiLeHV = tongDangKy > 0 ? (double)soCoMat / tongDangKy : 0;
                    double tiLeGV = tongGv > 0 ? (double)soGvCoMat / tongGv : 0;

                    sheet.Cells[row, 1].Value = b.LopHoc.KhoaHoc.TenKhoaHoc;
                    sheet.Cells[row, 2].Value = b.LopHoc.TenLopHoc;
                    sheet.Cells[row, 3].Value = b.NgayHoc.ToString("dd/MM/yyyy");
                    sheet.Cells[row, 4].Value = b.GioBatDau.ToString(@"hh\:mm");
                    sheet.Cells[row, 5].Value = b.GioKetThuc.ToString(@"hh\:mm");
                    sheet.Cells[row, 6].Value = tongDangKy;
                    sheet.Cells[row, 7].Value = soCoMat;
                    sheet.Cells[row, 8].Value = Math.Round(tiLeHV * 100, 2);
                    sheet.Cells[row, 9].Value = Math.Round(tiLeGV * 100, 2);

                    row++;
                }

                // Auto-fit cột
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

                var stream = new System.IO.MemoryStream(package.GetAsByteArray());

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ThongKeDiemDanh.xlsx");
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
