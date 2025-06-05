using OfficeOpenXml;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class ThongKeHVController : Controller
    {
        public DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        public ActionResult BaoCaoHocVien(Guid? khoaHocId)
        {
            ViewBag.KhoaHocList = new SelectList(db.KhoaHocs.OrderBy(k => k.TenKhoaHoc), "KhoaHocId", "TenKhoaHoc");
            if (khoaHocId == null)
            {
                return View(new List<HocVienThongKeViewModel>());
            }

            // Khi lần đầu vào trang có thể chọn mặc định, hoặc load rỗng cũng được.
            return View(new List<HocVienThongKeViewModel>());
        }
        public ActionResult GetHocVienByKhoaHoc(Guid khoaHocId, DateTime? startDate, DateTime? endDate)
        {
            var lopHocIds = db.LopHocs.Where(l => l.KhoaHocId == khoaHocId).Select(l => l.LopHocId).ToList();
            var dsDangKy = db.DangKyHocs.Where(dk => lopHocIds.Contains(dk.LopHocId)).ToList();

            var model = dsDangKy
                .GroupBy(dk => dk.HocVienId)
                .Select(g =>
                {
                    var hocVienId = g.Key;
                    var hv = g.First().HocVien;
                    var lopHocIdsCuaHocVien = g.Select(dk => dk.LopHocId).ToList();

                    var buoiHocIds = db.BuoiHocs
                        .Where(b => lopHocIdsCuaHocVien.Contains(b.LopHocId)
                                 && (!startDate.HasValue || b.NgayHoc >= startDate.Value)
                                 && (!endDate.HasValue || b.NgayHoc <= endDate.Value))
                        .Select(b => b.BuoiHocId)
                        .ToList();

                    int tongBuoiHoc = buoiHocIds.Count;
                    int soBuoiCoMat = db.DiemDanhs_HVs.Count(dd => dd.HocVienId == hocVienId
                                                    && buoiHocIds.Contains(dd.BuoiHocId)
                                                    && dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.CoMat);
                    int soBuoiDaDiemDanh = db.DiemDanhs_HVs.Count(dd => dd.HocVienId == hocVienId
                                                    && buoiHocIds.Contains(dd.BuoiHocId));
                    int soBuoiVang = tongBuoiHoc - soBuoiDaDiemDanh;

                    double tiLeDiemDanh = tongBuoiHoc == 0 ? 0 : (double)soBuoiCoMat / tongBuoiHoc * 100;

                    return new HocVienThongKeViewModel
                    {
                        HocVienId = hv.HocVienId,
                        MaHocVien = hv.MaHocVien,
                        HoVaTen = hv.HoVaTen,
                        NgaySinh = hv.NgaySinh,
                        SoDienThoai = hv.SoDienThoai,
                        Email = hv.Email,
                        CoQuanLamViec = hv.CoQuanLamViec,
                        SoLopDaDangKy = g.Count(),
                        SoBuoiDiHoc = soBuoiCoMat,
                        SoBuoiVang = soBuoiVang,
                        TongSoBuoi = tongBuoiHoc,
                        TiLeDiemDanh = tiLeDiemDanh,
                        IsConfirmed = hv.IsConfirmed,
                        TenKhoaHoc = g.First().LopHoc.KhoaHoc.TenKhoaHoc
                    };
                }).ToList();

            return PartialView("_HocVienThongKePartial", model);
        }

        public ActionResult ExportHocVienByKhoaHoc(Guid khoaHocId, DateTime? startDate, DateTime? endDate)
        {
            var lopHocIds = db.LopHocs.Where(l => l.KhoaHocId == khoaHocId).Select(l => l.LopHocId).ToList();
            var dsDangKy = db.DangKyHocs.Where(dk => lopHocIds.Contains(dk.LopHocId)).ToList();

            var model = dsDangKy
                .GroupBy(dk => dk.HocVienId)
                .Select(g =>
                {
                    var hocVienId = g.Key;
                    var hv = g.First().HocVien;
                    var lopHocIdsCuaHocVien = g.Select(dk => dk.LopHocId).ToList();

                    var buoiHocIds = db.BuoiHocs
                        .Where(b => lopHocIdsCuaHocVien.Contains(b.LopHocId)
                                 && (!startDate.HasValue || b.NgayHoc >= startDate.Value)
                                 && (!endDate.HasValue || b.NgayHoc <= endDate.Value))
                        .Select(b => b.BuoiHocId)
                        .ToList();

                    int tongBuoiHoc = buoiHocIds.Count;
                    int soBuoiCoMat = db.DiemDanhs_HVs.Count(dd => dd.HocVienId == hocVienId
                                                    && buoiHocIds.Contains(dd.BuoiHocId)
                                                    && dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.CoMat);
                    int soBuoiDaDiemDanh = db.DiemDanhs_HVs.Count(dd => dd.HocVienId == hocVienId
                                                    && buoiHocIds.Contains(dd.BuoiHocId));
                    int soBuoiVang = tongBuoiHoc - soBuoiDaDiemDanh;

                    double tiLeDiemDanh = tongBuoiHoc == 0 ? 0 : (double)soBuoiCoMat / tongBuoiHoc * 100;

                    return new HocVienThongKeViewModel
                    {
                        HocVienId = hv.HocVienId,
                        MaHocVien = hv.MaHocVien,
                        HoVaTen = hv.HoVaTen,
                        NgaySinh = hv.NgaySinh,
                        SoDienThoai = hv.SoDienThoai,
                        Email = hv.Email,
                        CoQuanLamViec = hv.CoQuanLamViec,
                        SoLopDaDangKy = g.Count(),
                        SoBuoiDiHoc = soBuoiCoMat,
                        SoBuoiVang = soBuoiVang,
                        TongSoBuoi = tongBuoiHoc,
                        TiLeDiemDanh = tiLeDiemDanh,
                        IsConfirmed = hv.IsConfirmed,
                        TenKhoaHoc = g.First().LopHoc.KhoaHoc.TenKhoaHoc
                    };
                }).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("ThongKeHocVien");

                // Thông tin khoảng lọc ngày
                string dateRangeText = "Toàn bộ thời gian";
                if (startDate.HasValue && endDate.HasValue)
                    dateRangeText = $"Từ ngày {startDate.Value:dd/MM/yyyy} đến ngày {endDate.Value:dd/MM/yyyy}";
                else if (startDate.HasValue)
                    dateRangeText = $"Từ ngày {startDate.Value:dd/MM/yyyy}";
                else if (endDate.HasValue)
                    dateRangeText = $"Đến ngày {endDate.Value:dd/MM/yyyy}";

                worksheet.Cells["A1"].Value = "Thống kê học viên";
                worksheet.Cells["A1:M1"].Merge = true;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells["A2"].Value = dateRangeText;
                worksheet.Cells["A2:M2"].Merge = true;
                worksheet.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2"].Style.Font.Bold = true;


                worksheet.Cells["A3"].Value = "STT";
                worksheet.Cells["B3"].Value = "Mã HV";
                worksheet.Cells["C3"].Value = "Họ tên";
                worksheet.Cells["D3"].Value = "Ngày sinh";
                worksheet.Cells["E3"].Value = "SĐT";
                worksheet.Cells["F3"].Value = "Email";
                worksheet.Cells["G3"].Value = "Cơ quan";
                worksheet.Cells["H3"].Value = "Số lớp đã ĐK";
                worksheet.Cells["I3"].Value = "Số buổi đi học";
                worksheet.Cells["J3"].Value = "Số buổi vắng";
                worksheet.Cells["K3"].Value = "Tổng số buổi";
                worksheet.Cells["L3"].Value = "Tỉ lệ điểm danh (%)";
                worksheet.Cells["M3"].Value = "Khóa học";

                int row = 4;
                int stt = 1;
                foreach (var item in model)
                {
                    worksheet.Cells[row, 1].Value = stt++;
                    worksheet.Cells[row, 2].Value = item.MaHocVien;
                    worksheet.Cells[row, 3].Value = item.HoVaTen;
                    worksheet.Cells[row, 4].Value = item.NgaySinh.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 5].Value = item.SoDienThoai;
                    worksheet.Cells[row, 6].Value = item.Email;
                    worksheet.Cells[row, 7].Value = item.CoQuanLamViec;
                    worksheet.Cells[row, 8].Value = item.SoLopDaDangKy;
                    worksheet.Cells[row, 9].Value = item.SoBuoiDiHoc;
                    worksheet.Cells[row, 10].Value = item.SoBuoiVang;
                    worksheet.Cells[row, 11].Value = item.TongSoBuoi;
                    worksheet.Cells[row, 12].Value = Math.Round(item.TiLeDiemDanh, 2);
                    worksheet.Cells[row, 13].Value = item.TenKhoaHoc;
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = "ThongKeHocVien.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(stream, contentType, fileName);
            }
        }
    }
}