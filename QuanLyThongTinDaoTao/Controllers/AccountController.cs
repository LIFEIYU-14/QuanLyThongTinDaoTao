using QuanLyThongTinDaoTao.Helpers;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsService;
using QuanLyThongTinDaoTao.Services;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QuanLyThongTinDaoTao.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbContextThongTinDaoTao _db = new DbContextThongTinDaoTao();

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hocVien = _db.HocViens.SingleOrDefault(hv => hv.MaHocVien == model.MaHocVien);
                if (hocVien != null && PasswordHelper.VerifyPassword(model.MatKhau, hocVien.MatKhau))
                {
                    var authTicket = new FormsAuthenticationTicket(
                        1,
                        hocVien.MaHocVien,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(30),
                        model.RememberMe,
                        hocVien.HoVaTen
                    );

                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                    {
                        HttpOnly = true,
                        Expires = authTicket.Expiration
                    };
                    Response.Cookies.Add(authCookie);

                    TempData["LoginSuccess"] = "Đăng nhập thành công! Chào mừng bạn, " + hocVien.HoVaTen + "!";
                    return RedirectToAction("Index", "Home");
                }

                TempData["LoginError"] = "Mã học viên hoặc mật khẩu không đúng.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddDays(-1)
            });

            return RedirectToAction("Login", "Account");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_db.HocViens.Any(hv => hv.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                if (_db.HocViens.Any(hv => hv.SoDienThoai == model.SoDienThoai))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng.");
                    return View(model);
                }

                string hashedPassword = PasswordHelper.HashPassword(model.MatKhau);
                string maHocVien = GenerateMaHocVien(model.NgaySinh.Value, model.SoNgauNhien);

                var hocVien = new HocVien
                {
                    NguoiDungId = Guid.NewGuid(),
                    HoVaTen = model.HoVaTen,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    MatKhau = hashedPassword,
                    VaiTro = VaiTroNguoiDung.HocVien,
                    NgaySinh = model.NgaySinh,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    MaHocVien = maHocVien
                };

                _db.HocViens.Add(hocVien);
                _db.SaveChanges();

                // 🔹 Tạo mã QR Code từ Mã Học Viên
                byte[] qrCodeBytes = QRCodeHelper.GenerateQRCode(maHocVien);
                string subject = "Chúc mừng bạn đã đăng ký thành công!";
                string body = $@"
                    <h3>Xin chào {hocVien.HoVaTen},</h3>
                    <p>Chúc mừng bạn đã đăng ký thành công tài khoản trên hệ thống đào tạo.</p>
                    <p>Mã học viên của bạn: <strong>{hocVien.MaHocVien}</strong></p>
                    <p>Vui lòng đăng nhập vào hệ thống để bắt đầu học tập.</p>
                    <p>Trân trọng,</p>
                    <p><strong>Ban Quản Trị Hệ Thống Đào Tạo</strong></p>";

                try
                {
                    EmailService.SendEmailWithAttachment(hocVien.Email, subject, body, qrCodeBytes, "QRCode.png");
                    TempData["RegisterSuccess"] = "Đăng ký thành công! Vui lòng kiểm tra email để nhận mã QR điểm danh.";
                }
                catch (Exception ex)
                {
                    TempData["RegisterError"] = "Đăng ký thành công nhưng lỗi gửi email.";
                    System.Diagnostics.Debug.WriteLine("Lỗi gửi email: " + ex.Message);
                }

                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        /// <summary>
        /// Tạo mã học viên theo format: 200 + ngày sinh (dd) + số ngẫu nhiên
        /// </summary>
        private string GenerateMaHocVien(DateTime ngaySinh, string soNgauNhien)
        {
            string ngaySinhFormatted = ngaySinh.ToString("dd"); // Lấy 2 số ngày sinh (dd)
            return $"200{ngaySinhFormatted}{soNgauNhien}";
        }
    }
}
