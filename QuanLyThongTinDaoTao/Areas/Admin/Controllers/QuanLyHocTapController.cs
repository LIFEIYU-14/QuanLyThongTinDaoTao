using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuanLyHocTapController : Controller
    {
        public DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        public ActionResult QuanLyHocTapTheoKhoaHoc(Guid? khoaHocId)
        {
            ViewBag.KhoaHocList = new SelectList(db.KhoaHocs.ToList(), "KhoaHocId", "TenKhoaHoc");

            if (khoaHocId == null)
            {
                return View(new List<HocVienHocTapViewModel>());
            }

            // Lấy tất cả đăng ký của học viên trong các lớp thuộc khóa học
            var dangKyTrongKhoa = db.DangKyHocs
                                    .Include(dk => dk.HocVien)
                                    .Include(dk => dk.LopHoc)
                                    .Where(dk => dk.LopHoc.KhoaHocId == khoaHocId)
                                    .ToList();

            // Gom các đăng ký theo học viên
            var hocVienTheoKhoa = dangKyTrongKhoa
                .GroupBy(dk => dk.HocVienId)
                .Select(group => new
                {
                    HocVien = group.First().HocVien,
                    LopHocIds = group.Select(x => x.LopHocId).ToList()
                }).ToList();

            var result = new List<HocVienHocTapViewModel>();

            foreach (var item in hocVienTheoKhoa)
            {
                var hocVienId = item.HocVien.HocVienId;

                // Đếm tổng số buổi học trong các lớp học mà học viên đăng ký
                int tongBuoiHoc = db.BuoiHocs
                                    .Where(bh => item.LopHocIds.Contains(bh.LopHocId))
                                    .Count();

                // Đếm số buổi có mặt
                int soBuoiCoMat = db.DiemDanhs_HVs
                                    .Where(dd => dd.HocVienId == hocVienId
                                        && item.LopHocIds.Contains(dd.BuoiHoc.LopHocId)
                                        && dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.CoMat)
                                    .Count();

                // Đếm số buổi vắng (có phép hoặc không phép)
                int soBuoiVang = db.DiemDanhs_HVs
                                    .Where(dd => dd.HocVienId == hocVienId
                                        && item.LopHocIds.Contains(dd.BuoiHoc.LopHocId)
                                        && (dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.VangCoPhep
                                            || dd.TrangThai == DiemDanh_HV.TrangThaiDiemDanhHV.VangKhongPhep))
                                    .Count();

                // Kiểm tra chứng chỉ đã cấp chưa
                bool daCapChungChi = db.ChungChiHocTaps.Any(cc => cc.HocVienId == hocVienId && cc.KhoaHocId == khoaHocId);

                result.Add(new HocVienHocTapViewModel
                {
                    HocVienId = hocVienId,
                    TenHocVien = item.HocVien.HoVaTen,
                    NgaySinh = item.HocVien.NgaySinh,
                    Email = item.HocVien.Email,
                    TongBuoiHoc = tongBuoiHoc,
                    SoBuoiCoMat = soBuoiCoMat,
                    SoBuoiVang = soBuoiVang,
                    DaCapChungChi = daCapChungChi
                });
            }

            return View(result);
        }

        [HttpPost]
        public JsonResult CapChungChi(string hocVienId, string khoaHocId, string ngayHetHan, string ngayCap = null)
        {
            try
            {
                if (string.IsNullOrEmpty(hocVienId))
                    return Json(new { success = false, message = "Học viên không hợp lệ" });

                if (!Guid.TryParse(khoaHocId, out Guid khoaHocGuid))
                    return Json(new { success = false, message = "Khóa học không hợp lệ" });

                if (string.IsNullOrEmpty(ngayHetHan))
                    return Json(new { success = false, message = "Vui lòng chọn ngày hết hạn" });

                if (!DateTime.TryParseExact(ngayHetHan, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime parsedNgayHetHan))
                {
                    return Json(new { success = false, message = "Ngày hết hạn không hợp lệ" });
                }

                DateTime parsedNgayCap = DateTime.Now;
                if (!string.IsNullOrEmpty(ngayCap))
                {
                    if (!DateTime.TryParseExact(ngayCap, "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out parsedNgayCap))
                    {
                        return Json(new { success = false, message = "Ngày cấp không hợp lệ" });
                    }
                }

                if (parsedNgayHetHan <= parsedNgayCap)
                {
                    return Json(new { success = false, message = "Ngày hết hạn phải sau ngày cấp." });
                }

                bool daCap = db.ChungChiHocTaps.Any(cc => cc.HocVienId == hocVienId && cc.KhoaHocId == khoaHocGuid);
                if (daCap)
                    return Json(new { success = false, message = "Học viên đã được cấp chứng chỉ" });

                var chungChi = new ChungChiHocTap
                {
                    ChungChiId = Guid.NewGuid(),
                    HocVienId = hocVienId,
                    KhoaHocId = khoaHocGuid,
                    NgayCap = parsedNgayCap,
                    NgayHetHan = parsedNgayHetHan
                };

                db.ChungChiHocTaps.Add(chungChi);
                db.SaveChanges();

                return Json(new { success = true, message = "Cấp chứng chỉ thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }



        [HttpPost]
        public JsonResult HuyCapChungChi(string hocVienId, Guid khoaHocId)
        {
            var chungChi = db.ChungChiHocTaps.FirstOrDefault(cc => cc.HocVienId == hocVienId && cc.KhoaHocId == khoaHocId);

            if (chungChi == null)
            {
                return Json(new { success = false, message = "Không tìm thấy chứng chỉ để hủy." });
            }

            db.ChungChiHocTaps.Remove(chungChi);
            db.SaveChanges();

            return Json(new { success = true, message = "Đã hủy cấp chứng chỉ." });
        }
    }
}
