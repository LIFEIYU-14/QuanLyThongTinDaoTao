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
        private readonly HocVienService hocVienService = new HocVienService(new DbContextThongTinDaoTao());

        public ActionResult Index()
        {
            var khoaHocs = db.KhoaHocs
                .Include(k => k.KhoaHocAttachments)
                .Take(3) // Chỉ lấy 3 khóa học đầu tiên
                .ToList();

            return View(khoaHocs);
        }
        public JsonResult LoadMoreCourses(int skip, int take)
        {
            var courses = db.KhoaHocs
                .OrderBy(k => k.KhoaHocId)
                .Skip(skip)
                .Take(take)
                .Include(k => k.KhoaHocAttachments)
                .ToList()  // Chuyển thành danh sách C# trước
                .Select(k => new
                {
                    KhoaHocId = k.KhoaHocId,
                    TenKhoaHoc = k.TenKhoaHoc,
                    ThoiLuong = k.ThoiLuong,
                    HinhDaiDienKhoaHoc = !string.IsNullOrEmpty(k.HinhDaiDienKhoaHoc)
                                         ? Url.Content(k.HinhDaiDienKhoaHoc)
                                         : (k.KhoaHocAttachments?
                                            .FirstOrDefault(a => a.Attachment != null &&
                                                 (a.Attachment.FileType.ToLower().Contains("jpg") ||
                                                  a.Attachment.FileType.ToLower().Contains("png") ||
                                                  a.Attachment.FileType.ToLower().Contains("jpeg")))?
                                            .Attachment?.FilePath ?? Url.Content("~/Upload/KhoaHoc/default.png"))
                })
                .ToList();

            return Json(courses, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DanhSachLopHoc(Guid? id)
        {

            var khoaHoc = db.KhoaHocs.Include(k => k.LopHocs)
                                     .Include(k => k.KhoaHocAttachments)
                                     .FirstOrDefault(k => k.KhoaHocId == id.Value);

            if (khoaHoc == null)
                return HttpNotFound("Không tìm thấy khóa học.");

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
                    LopHocId = lopHocId
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
            // **Tìm học viên theo email**
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.Email == email);

            if (hocVien == null)
            {
                // **Nếu chưa có học viên, tạo mới**
                hocVien = new HocVien
                {
                    NguoiDungId = Guid.NewGuid(),
                    MaHocVien = DateTime.Now.Year + hocVienData.NgaySinh.Year.ToString() + new Random().Next(1000, 9999),
                    TaiKhoan = hocVien.MaHocVien,
                    MatKhau = PasswordHelper.HashPassword(hocVien.MaHocVien + "123456"),
                    HoVaTen = hocVienData.HoVaTen,
                    Email = hocVienData.Email,
                    NgaySinh = hocVienData.NgaySinh,
                    SoDienThoai = hocVienData.SoDienThoai,
                    CoQuanLamViec = hocVienData.CoQuanLamViec,
                    QR_Code_HV = Guid.NewGuid().ToString(),
                    IsConfirmed = true,
                };

                db.HocViens.Add(hocVien);
                db.SaveChanges();
            }

            var lopHoc = db.LopHocs.Include(l => l.KhoaHoc).FirstOrDefault(l => l.LopHocId == hocVienData.LopHocId);
            if (lopHoc == null)
            {
                TempData["Error"] = "Lớp học không tồn tại.";
                return RedirectToAction("Index", "Home");
            }
            // **Kiểm tra xem học viên đã đăng ký lớp học chưa**
            var existingDangKy = db.DangKyHocs.FirstOrDefault(dk => dk.NguoiDungId == hocVien.NguoiDungId && dk.LopHocId == hocVienData.LopHocId);
            if (existingDangKy != null)
            {
                TempData["Error"] = "Bạn đã đăng ký lớp học này trước đó.";
                return RedirectToAction("Index", "Home");
            }

            // **Tạo đăng ký lớp học mới**
            var dangKyHoc = new DangKyHoc
            {
                DangKyId = Guid.NewGuid(),
                NguoiDungId = hocVien.NguoiDungId,
                LopHocId = hocVienData.LopHocId,
                IsConfirmed = true,
                NgayDangKy = DateTime.Now
            };

            db.DangKyHocs.Add(dangKyHoc);

            try
            {
                db.SaveChanges();
                // Tạo mã QR cho học viên sau khi đăng ký thành công
                string qrCode = hocVienService.GenerateQRCodeForStudent(hocVien.NguoiDungId);

                // Gửi QR Code qua email
                await emailService.SendQrCodeEmail(hocVien.Email, qrCode);

                TempData["Success"] = "Xác nhận thành công";
                return RedirectToAction("Index", "Courses");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError("", $"{validationError.PropertyName}: {validationError.ErrorMessage}");
                    }
                }
                return View("Register");
            }
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
