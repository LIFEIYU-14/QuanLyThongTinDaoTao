using Newtonsoft.Json;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Controllers
{
    public class TraCuuChungChiController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TraCuuBangEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Vui lòng nhập email.";
                return View("Index");
            }

            var hocVien = db.HocViens.FirstOrDefault(h => h.Email == email);
            if (hocVien == null)
            {
                ViewBag.Error = "Không tìm thấy học viên với email này.";
                return View("Index");
            }

            var chungChis = db.ChungChiHocTaps
                              .Where(c => c.HocVienId == hocVien.HocVienId)
                              .ToList();

            if (!chungChis.Any())
            {
                ViewBag.Error = "Học viên chưa được cấp chứng chỉ.";
                return View("Index");
            }

            var model = new HocVienHocTapViewModel
            {
                HocVienId = hocVien.MaHocVien,
                TenHocVien = hocVien.HoVaTen,
                Email = hocVien.Email,
                NgaySinh = hocVien.NgaySinh,
                DaCapChungChi = true,
                ChungChis = chungChis.Select(c => new ChungChiViewModel
                {
                    TenKhoaHoc = c.KhoaHoc?.TenKhoaHoc,
                    NgayCap = c.NgayCap,
                    NgayHetHan = c.NgayHetHan
                }).ToList()
            };

            return View("Index", model);
        }

        [HttpPost]
        public JsonResult QuetQR(string qrData)
        {
            try
            {
                var parsed = JsonConvert.DeserializeObject<dynamic>(qrData);
                string hocVienId = parsed.HocVienId;

                var hocVien = db.HocViens.FirstOrDefault(h => h.HocVienId == hocVienId);
                if (hocVien == null)
                    return Json(new { success = false, message = "Không tìm thấy học viên." });

                var chungChis = db.ChungChiHocTaps.Where(c => c.HocVienId == hocVienId).ToList();
                if (!chungChis.Any())
                    return Json(new { success = false, message = "Học viên chưa được cấp chứng chỉ." });

                return Json(new { success = true, message = "✅ Tra cứu thành công!", hocVienId = hocVienId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi xử lý dữ liệu QR.", error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult IndexTheoId(string id)
        {
            var hocVien = db.HocViens.FirstOrDefault(h => h.HocVienId == id);
            if (hocVien == null)
            {
                ViewBag.Error = "Không tìm thấy học viên.";
                return View("Index");
            }

            var chungChis = db.ChungChiHocTaps.Where(c => c.HocVienId == hocVien.HocVienId).ToList();
            if (!chungChis.Any())
            {
                ViewBag.Error = "Học viên chưa được cấp chứng chỉ.";
                return View("Index");
            }

            var model = new HocVienHocTapViewModel
            {
                HocVienId = hocVien.MaHocVien,
                TenHocVien = hocVien.HoVaTen,
                Email = hocVien.Email,
                NgaySinh = hocVien.NgaySinh,
                DaCapChungChi = true,
                ChungChis = chungChis.Select(c => new ChungChiViewModel
                {
                    TenKhoaHoc = c.KhoaHoc?.TenKhoaHoc,
                    NgayCap = c.NgayCap,
                    NgayHetHan = c.NgayHetHan
                }).ToList()
            };

            return View("Index", model);
        }
    }
}
