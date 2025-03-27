using QuanLyThongTinDaoTao.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QuanLyThongTinDaoTao.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbContextThongTinDaoTao _db = new DbContextThongTinDaoTao();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendVerificationCode(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return Json(new { success = false, message = "Email không được để trống!" });
                }

                string verificationCode = new Random().Next(100000, 999999).ToString();
                Session["VerificationCode"] = verificationCode;
                Session["VerificationEmail"] = email.Trim(); // Lưu email đã chuẩn hóa

                string fromEmail = ConfigurationManager.AppSettings["EmailUsername"];
                string fromPassword = ConfigurationManager.AppSettings["EmailPassword"];

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Hệ Thống Đào Tạo"),
                    Subject = "Mã Xác Nhận Đăng Nhập",
                    Body = $"Mã xác nhận của bạn là: <b>{verificationCode}</b>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                smtpClient.Send(mailMessage);

                return Json(new { success = true, message = "Mã xác nhận đã được gửi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi gửi email: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult VerifyCode(string email, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
                {
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ email và mã xác nhận!" });
                }

                if (Session["VerificationCode"] == null || Session["VerificationEmail"] == null)
                {
                    return Json(new { success = false, message = "Phiên xác nhận đã hết hạn. Vui lòng thử lại!" });
                }

                string savedCode = Session["VerificationCode"]?.ToString().Trim();
                string savedEmail = Session["VerificationEmail"]?.ToString().Trim();

                // Log kiểm tra giá trị email & mã xác nhận
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Email nhập vào: {email}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Email session: {savedEmail}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Mã xác nhận nhập vào: {code}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Mã xác nhận session: {savedCode}");

                if (!email.Equals(savedEmail, StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "Email không khớp với email đã đăng ký." });
                }

                if (!code.Equals(savedCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "Mã xác nhận không chính xác. Vui lòng thử lại!" });
                }

                // Xử lý đăng nhập thành công
                var hocVien = _db.HocViens.SingleOrDefault(hv => hv.Email == email);
                if (hocVien == null)
                {
                    hocVien = new HocVien
                    {
                        Email = email,
                        HoVaTen = "Học viên mới",
                        MaHocVien = GenerateMaHocVien(),
                        CoQuanLamViec = "Chưa cập nhật", // Thêm giá trị mặc định
                        MatKhau = PasswordHelper.HashPassword("default123"), // Mật khẩu mặc định, có thể yêu cầu đổi sau
                        VaiTro = VaiTroNguoiDung.HocVien,
                        NgaySinh = new DateTime(2000, 1, 1), // Giả định ngày sinh, sau có thể yêu cầu cập nhật
                        SoDienThoai = "0000000000", // Giá trị mặc định tránh lỗi ,sau có thể yêu cầu cập nhật
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    };
                    _db.HocViens.Add(hocVien);
                }

                try
                {
                    _db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} - Error: {validationError.ErrorMessage}");
                        }
                    }
                    return Json(new { success = false, message = "Lỗi dữ liệu: Vui lòng kiểm tra lại thông tin!" });
                }


                var authTicket = new FormsAuthenticationTicket(1, hocVien.Email, DateTime.Now, DateTime.Now.AddMinutes(30), false, hocVien.HoVaTen);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                {
                    HttpOnly = true,
                    Expires = authTicket.Expiration
                };
                Response.Cookies.Add(authCookie);

                Session.Remove("VerificationCode");
                Session.Remove("VerificationEmail");

                return Json(new { success = true, message = "Đăng nhập thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }




        private string GenerateMaHocVien()
        {
            return $"{DateTime.Now.Year}0000{_db.HocViens.Count() + 1:D4}";
        }
    }
}
