using OfficeOpenXml;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class ThongKeGVController : Controller
    {
        public DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        // GET: Admin/ThongKeGV
        public ActionResult BaoCaoGiangVien(DateTime? startDate, DateTime? endDate)
        {
            var query = db.GiangVien_BuoiHoc.Include("BuoiHoc").Include("GiangVien").AsQueryable();

            if (startDate.HasValue)
                query = query.Where(g => g.BuoiHoc.NgayHoc >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(g => g.BuoiHoc.NgayHoc <= endDate.Value);

            var dsGVBuoiHoc = query.ToList();

            var thongKe = dsGVBuoiHoc
                .GroupBy(g => g.GiangVienId)
                .Select(group => {
                    var giangVienId = group.Key;
                    var tenGV = group.First().GiangVien.HoVaTen;
                    var buoiHocIds = group.Select(g => g.BuoiHocId).ToList();

                    var soBuoiCoMat = db.DiemDanhs_GVs
                        .Where(d => d.GiangVienId == giangVienId
                                 && d.TrangThai == DiemDanh_GV.TrangThaiDiemDanhGV.CoMat
                                 && buoiHocIds.Contains(d.BuoiHocId))
                        .Count();

                    return new ThongKeGiangVienViewModel
                    {
                        GiangVienId = giangVienId,
                        TenGiangVien = tenGV,
                        SoBuoiDay = group.Count(),
                        SoBuoiCoMat = soBuoiCoMat,
                        SoBuoiVang = group.Count() - soBuoiCoMat,
                        TyLeChuyenCan = group.Count() > 0
                            ? Math.Round((double)soBuoiCoMat * 100 / group.Count(), 2)
                            : 0,
                        LopHocThamGia = group.Select(g => g.BuoiHoc.LopHoc.TenLopHoc).Distinct().ToList()
                    };
                }).ToList();

            return View(thongKe);
        }

        public ActionResult XuatExcel(DateTime? startDate, DateTime? endDate)
        {
            var query = db.GiangVien_BuoiHoc.Include("BuoiHoc").Include("GiangVien").AsQueryable();

            if (startDate.HasValue)
                query = query.Where(g => g.BuoiHoc.NgayHoc >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(g => g.BuoiHoc.NgayHoc <= endDate.Value);

            var dsGVBuoiHoc = query.ToList();

            var thongKe = dsGVBuoiHoc
                .GroupBy(g => g.GiangVienId)
                .Select(group => {
                    var giangVienId = group.Key;
                    var tenGV = group.First().GiangVien.HoVaTen;
                    var buoiHocIds = group.Select(g => g.BuoiHocId).ToList();

                    var soBuoiCoMat = db.DiemDanhs_GVs
                        .Where(d => d.GiangVienId == giangVienId
                                 && d.TrangThai == DiemDanh_GV.TrangThaiDiemDanhGV.CoMat
                                 && buoiHocIds.Contains(d.BuoiHocId))
                        .Count();

                    return new
                    {
                        TenGiangVien = tenGV,
                        SoBuoiDay = group.Count(),
                        SoBuoiCoMat = soBuoiCoMat,
                        SoBuoiVang = group.Count() - soBuoiCoMat,
                        TyLeChuyenCan = group.Count() > 0 ? Math.Round((double)soBuoiCoMat * 100 / group.Count(), 2) : 0,
                        LopHocThamGia = group.Select(g => g.BuoiHoc.LopHoc.TenLopHoc).Distinct().ToList()
                    };
                }).ToList();

            // Tạo mô tả thời gian lọc
            string moTaThoiGian = "Thống kê tổng hợp";
            if (startDate.HasValue || endDate.HasValue)
            {
                string tuNgay = startDate.HasValue ? startDate.Value.ToString("dd/MM/yyyy") : "...";
                string denNgay = endDate.HasValue ? endDate.Value.ToString("dd/MM/yyyy") : "...";
                moTaThoiGian = $"Thống kê từ ngày {tuNgay} đến ngày {denNgay}";
            }

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("ThongKeGiangVien");

                // Ghi chú thời gian lọc ở dòng đầu tiên
                sheet.Cells[1, 1].Value = moTaThoiGian;
                sheet.Cells[1, 1, 1, 5].Merge = true;
                sheet.Cells[1, 1].Style.Font.Bold = true;

                // Ghi tiêu đề bảng vào hàng 3
                string[] headers = { "Giảng viên", "Số buổi có mặt", "Số buổi vắng", "Tỷ lệ chuyên cần (%)", "Lớp giảng dạy" };
                for (int i = 0; i < headers.Length; i++)
                {
                    sheet.Cells[3, i + 1].Value = headers[i];
                    sheet.Cells[3, i + 1].Style.Font.Bold = true;
                }

                // Ghi dữ liệu từ hàng 4
                int row = 4;
                foreach (var gv in thongKe)
                {
                    sheet.Cells[row, 1].Value = gv.TenGiangVien;
                    sheet.Cells[row, 2].Value = gv.SoBuoiCoMat;
                    sheet.Cells[row, 3].Value = gv.SoBuoiVang;
                    sheet.Cells[row, 4].Value = gv.TyLeChuyenCan;
                    sheet.Cells[row, 5].Value = string.Join(", ", gv.LopHocThamGia);
                    row++;
                }

                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ThongKeGiangVien.xlsx");
            }
        }
    }
}
