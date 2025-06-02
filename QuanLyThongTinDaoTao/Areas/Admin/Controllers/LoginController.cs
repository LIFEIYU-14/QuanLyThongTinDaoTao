using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuanLyThongTinDaoTao.Identity;
using QuanLyThongTinDaoTao.LoginModelView;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using QuanLyThongTinDaoTao.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private AppUserManager _userManager;
        private AppSignInManager _signInManager;
        private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        private readonly EmailService emailService = new EmailService();
        public LoginController() { }

        public LoginController(AppUserManager userManager, AppSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public AppUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            private set => _userManager = value;
        }

        public AppSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<AppSignInManager>();
            private set => _signInManager = value;
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            ViewBag.SuccessMessage = TempData["Success"] as string;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DangNhap(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await UserManager.FindByNameAsync(model.TaiKhoanOrEmail)
                    ?? await UserManager.FindByEmailAsync(model.TaiKhoanOrEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại.");
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.MatKhau, false, false);

            if (result == SignInStatus.Success)
            {
                var roles = await UserManager.GetRolesAsync(user.Id);
                if (roles.Contains("Admin") || roles.Contains("GiangVien"))
                    return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", result == SignInStatus.LockedOut
                ? "Tài khoản đã bị khóa."
                : "Tài khoản hoặc mật khẩu không đúng.");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangXuat()
        {
            SignInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuenMatKhau(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Vui lòng nhập email.");
                return View("DangNhap");
            }

            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
                return View("DangNhap");
            }

            var roles = await UserManager.GetRolesAsync(user.Id);
            if (!roles.Contains("GiangVien"))
            {
                ModelState.AddModelError("", "Chỉ hỗ trợ gửi mật khẩu cho giảng viên.");
                return View("DangNhap");
            }

            // Lấy thông tin giảng viên theo AppUserId
            var giangVien = db.GiangViens.FirstOrDefault(gv => gv.AppUserId == user.Id);
            if (giangVien == null)
            {
                ModelState.AddModelError("", "Không tìm thấy thông tin giảng viên.");
                return View("DangNhap");
            }

            // Tạo mật khẩu mới ngẫu nhiên
            string newPassword = GenerateRandomPassword();

            var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var resetResult = await UserManager.ResetPasswordAsync(user.Id, resetToken, newPassword);
            if (!resetResult.Succeeded)
            {
                ModelState.AddModelError("", "Không thể đặt lại mật khẩu. Vui lòng thử lại.");
                return View("DangNhap");
            }

            // Lấy mã QR code base64 lưu sẵn trong DB
            string qrCodeBase64 = giangVien.QR_Code_GV;

            try
            {
                var emailService = new EmailService();
                await emailService.SendTeacherAccountWithQrEmail(email, user.UserName, newPassword, qrCodeBase64);

                TempData["Success"] = "Mật khẩu mới đã được gửi vào email của bạn.";
                return RedirectToAction("DangNhap");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Gửi email thất bại. Vui lòng thử lại.");
                return View("DangNhap");
            }
        }

        private string GenerateRandomPassword(int length = 8)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var res = new char[length];
            var rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                res[i] = valid[rnd.Next(valid.Length)];
            }
            return new string(res);
        }
    }
}
