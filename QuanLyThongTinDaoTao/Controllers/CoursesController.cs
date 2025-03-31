using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.Services;
using PagedList;

namespace QuanLyThongTinDaoTao.Controllers
{
    public class CoursesController : Controller
    {
        private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        private readonly EmailService emailService = new EmailService();

        public ActionResult Index(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            var khoaHocs = db.KhoaHocs.Include(k => k.KhoaHocAttachments)
                                      .OrderByDescending(k => k.NgayTao)
                                      .ToPagedList(pageNumber, pageSize);
            return View(khoaHocs);
        }

        public ActionResult DanhSachLopHoc(Guid id)
        {
            var khoaHoc = db.KhoaHocs.Include(k => k.LopHocs)
                                     .Include(k => k.KhoaHocAttachments)
                                     .FirstOrDefault(k => k.KhoaHocId == id);
            if (khoaHoc == null)
                return HttpNotFound();

            ViewBag.KhoaHoc = khoaHoc;
            return View(khoaHoc.LopHocs.ToList());
        }

        public ActionResult Register(Guid? lopHocId)
        {
            if (!lopHocId.HasValue)
            {
                TempData["Error"] = "Lớp học không hợp lệ.";
                return RedirectToAction("Index");
            }

            var lopHoc = db.LopHocs.Include(l => l.KhoaHoc).FirstOrDefault(l => l.LopHocId == lopHocId.Value);
            if (lopHoc == null)
            {
                TempData["Error"] = "Lớp học không tồn tại.";
                return RedirectToAction("Index");
            }

            return View(lopHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(FormCollection form)
        {
            try
            {
                string hoVaTen = form["HoVaTen"];
                string email = form["Email"];
                string soDienThoai = form["SoDienThoai"];
                string coQuanLamViec = form["CoQuanLamViec"] ?? "Không có";

                if (!DateTime.TryParse(form["NgaySinh"], out DateTime ngaySinh))
                {
                    ModelState.AddModelError("", "Ngày sinh không hợp lệ.");
                    return View();
                }

                if (!Guid.TryParse(form["LopHocId"], out Guid lopHocId))
                {
                    ModelState.AddModelError("", "Lớp học không hợp lệ.");
                    return View();
                }

                var lopHoc = db.LopHocs.Include(l => l.KhoaHoc).FirstOrDefault(l => l.LopHocId == lopHocId);
                if (lopHoc == null) return HttpNotFound();

                // Tạo mã OTP
                string otp = new Random().Next(100000, 999999).ToString();
                Session["OTP_" + email] = otp;
                Session["HocVienData_" + email] = new HocVienTempData
                {
                    HoVaTen = hoVaTen,
                    Email = email,
                    NgaySinh = ngaySinh,
                    SoDienThoai = soDienThoai,
                    CoQuanLamViec = coQuanLamViec,
                    LopHocId = lopHocId,
                    KhoaHocId = lopHoc?.KhoaHoc?.KhoaHocId ?? Guid.Empty
                };

                await emailService.SendOtpEmail(email, otp);
                ViewBag.Email = email;
                return View(lopHoc);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi đăng ký: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyOTP(string email, string otp)
        {
            string storedOtp = Session["OTP_" + email]?.ToString();
            if (storedOtp == null || storedOtp != otp)
            {
                TempData["Error"] = "Mã OTP không chính xác hoặc đã hết hạn.";
                return RedirectToAction("Register", new { email });
            }

            var hocVienData = Session["HocVienData_" + email] as HocVienTempData;
            if (hocVienData == null) return RedirectToAction("Index", "Home");

            Session.Remove("OTP_" + email);
            Session.Remove("HocVienData_" + email);

            // **Kiểm tra xem học viên đã tồn tại chưa**
            var existingHocVien = db.HocViens.FirstOrDefault(hv => hv.Email == email);
            if (existingHocVien != null)
            {
                TempData["Error"] = "Email đã được đăng ký trước đó. Vui lòng sử dụng email khác hoặc kiểm tra lại.";
                return RedirectToAction("Index", "Home");
            }
            var hocVien = new HocVien
            {
                NguoiDungId = Guid.NewGuid(),
                MaHocVien = DateTime.Now.Year + hocVienData.NgaySinh.Year.ToString() + new Random().Next(1000, 9999),
                HoVaTen = hocVienData.HoVaTen,
                Email = hocVienData.Email,
                NgaySinh = hocVienData.NgaySinh,
                SoDienThoai = hocVienData.SoDienThoai,
                CoQuanLamViec = hocVienData.CoQuanLamViec,
                QR_Code_HV = Guid.NewGuid().ToString(),
                IsConfirmed = true,
                VaiTro = VaiTroNguoiDung.HocVien
            };

            db.HocViens.Add(hocVien);
            db.SaveChanges();

            var lopHoc = db.LopHocs.Include(l => l.KhoaHoc).FirstOrDefault(l => l.LopHocId == hocVienData.LopHocId);
            if (lopHoc == null)
            {
                TempData["Error"] = "Lớp học không tồn tại.";
                return RedirectToAction("Index", "Home");
            }

            var dangKyHoc = new DangKyHoc
            {
                DangKyId = Guid.NewGuid(),
                NguoiDungId = hocVien.NguoiDungId,
                LopHocId = hocVienData.LopHocId,
                KhoaHocId = lopHoc.KhoaHoc.KhoaHocId,
                IsConfirmed = true
            };

            db.DangKyHocs.Add(dangKyHoc);
            db.SaveChanges();

            await emailService.SendQrCodeEmail(hocVien.Email, hocVien.QR_Code_HV);
            return RedirectToAction("Success");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
