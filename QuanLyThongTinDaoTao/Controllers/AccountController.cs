

//using QuanLyThongTinDaoTao.Models;
//using System;
//using System.Configuration;
//using System.Linq;
//using System.Net;
//using System.Net.Mail;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Security;

//namespace QuanLyThongTinDaoTao.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly DbContextThongTinDaoTao _db = new DbContextThongTinDaoTao();

//        public ActionResult Login()
//        {
//            return View();
//        }

//        public ActionResult Logout()
//        {
//            FormsAuthentication.SignOut();
//            Session.Clear();
//            return RedirectToAction("Login");
//        }

//        public ActionResult ThongTinCaNhan()
//        {
//            if (!User.Identity.IsAuthenticated)
//            {
//                return RedirectToAction("Login");
//            }

//            string email = User.Identity.Name; // Lấy email từ FormsAuthentication

//            var hocVien = _db.HocViens.FirstOrDefault(h => h.Email == email);
//            if (hocVien == null)
//            {
//                return HttpNotFound("Không tìm thấy thông tin học viên.");
//            }

//            return View(hocVien);
//        }

//        public ActionResult Edit()
//        {
//            if (!User.Identity.IsAuthenticated)
//            {
//                return RedirectToAction("Login");
//            }

//            string email = User.Identity.Name;
//            var hocVien = _db.HocViens.FirstOrDefault(h => h.Email == email);
//            if (hocVien == null)
//            {
//                return HttpNotFound("Không tìm thấy thông tin học viên.");
//            }

//            return View(hocVien);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(HocVien model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            var hocVien = _db.HocViens.FirstOrDefault(h => h.Email == model.Email);
//            if (hocVien == null)
//            {
//                return HttpNotFound("Không tìm thấy học viên.");
//            }

//            hocVien.HoVaTen = model.HoVaTen;
//            hocVien.NgaySinh = model.NgaySinh;
//            hocVien.SoDienThoai = model.SoDienThoai;
//            hocVien.CoQuanLamViec = model.CoQuanLamViec;
//            hocVien.NgayCapNhat = DateTime.Now;

//            _db.SaveChanges();

//            return RedirectToAction("ThongTinCaNhan");
//        }

//        [HttpPost]
//        public JsonResult SendVerificationCode(string email)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(email))
//                {
//                    return Json(new { success = false, message = "Email không được để trống!" });
//                }

//                string verificationCode = new Random().Next(100000, 999999).ToString();
//                Session["VerificationCode"] = verificationCode;
//                Session["VerificationEmail"] = email.Trim();

//                string fromEmail = ConfigurationManager.AppSettings["EmailUsername"];
//                string fromPassword = ConfigurationManager.AppSettings["EmailPassword"];

//                var smtpClient = new SmtpClient("smtp.gmail.com")
//                {
//                    Port = 587,
//                    Credentials = new NetworkCredential(fromEmail, fromPassword),
//                    EnableSsl = true
//                };

//                var mailMessage = new MailMessage
//                {
//                    From = new MailAddress(fromEmail, "Hệ Thống Đào Tạo"),
//                    Subject = "Mã Xác Nhận Đăng Nhập",
//                    Body = $"Mã xác nhận của bạn là: <b>{verificationCode}</b>",
//                    IsBodyHtml = true
//                };

//                mailMessage.To.Add(email);
//                smtpClient.Send(mailMessage);

//                return Json(new { success = true, message = "Mã xác nhận đã được gửi!" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = "Lỗi khi gửi email: " + ex.Message });
//            }
//        }

//        [HttpPost]
//        public JsonResult VerifyCode(string email, string code)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
//                {
//                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ email và mã xác nhận!" });
//                }

//                if (Session["VerificationCode"] == null || Session["VerificationEmail"] == null)
//                {
//                    return Json(new { success = false, message = "Phiên xác nhận đã hết hạn. Vui lòng thử lại!" });
//                }

//                string savedCode = Session["VerificationCode"]?.ToString().Trim();
//                string savedEmail = Session["VerificationEmail"]?.ToString().Trim();

//                if (!email.Equals(savedEmail, StringComparison.OrdinalIgnoreCase))
//                {
//                    return Json(new { success = false, message = "Email không khớp với email đã đăng ký." });
//                }

//                if (!code.Equals(savedCode, StringComparison.OrdinalIgnoreCase))
//                {
//                    return Json(new { success = false, message = "Mã xác nhận không chính xác. Vui lòng thử lại!" });
//                }

//                // Xử lý đăng nhập thành công
//                var hocVien = _db.HocViens.SingleOrDefault(hv => hv.Email == email);
//                if (hocVien == null)
//                {
//                    hocVien = new HocVien
//                    {
//                        Email = email,
//                        HoVaTen = "Học viên mới",
//                        MaHocVien = GenerateMaHocVien(),
//                        CoQuanLamViec = "Chưa cập nhật",
//                        MatKhau = PasswordHelper.HashPassword("default123"),
//                        VaiTro = VaiTroNguoiDung.HocVien,
//                        NgaySinh = new DateTime(2000, 1, 1),
//                        SoDienThoai = "0000000000",
//                        NgayTao = DateTime.Now,
//                        NgayCapNhat = DateTime.Now
//                    };
//                    _db.HocViens.Add(hocVien);
//                    _db.SaveChanges();
//                }

//                // Thiết lập FormsAuthentication để duy trì trạng thái đăng nhập
//                FormsAuthentication.SetAuthCookie(hocVien.Email, false);

//                Session.Remove("VerificationCode");
//                Session.Remove("VerificationEmail");

//                return Json(new { success = true, message = "Đăng nhập thành công!" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
//            }
//        }

//        private string GenerateMaHocVien()
//        {
//            return $"{DateTime.Now.Year}0000{_db.HocViens.Count() + 1:D4}";
//        }
//    }
//}
