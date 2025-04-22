using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuanLyThongTinDaoTao.Identity;
using QuanLyThongTinDaoTao.LoginModelView;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private AppUserManager _userManager;
        private AppSignInManager _signInManager;

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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DangNhap(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await UserManager.FindByNameAsync(model.TaiKhoanOrEmail)
                       ?? await UserManager.FindByEmailAsync(model.TaiKhoanOrEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại.");
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(
                user.UserName, model.MatKhau, isPersistent: false, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    var roles = await UserManager.GetRolesAsync(user.Id);
                    if (roles.Contains("Admin"))
                        return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
                    if (roles.Contains("GiangVien"))
                        return RedirectToAction("Index", "HomeGiangVien", new { area = "GiangVien" });

                    return RedirectToAction("Index", "Home");

                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "Tài khoản đã bị khóa.");
                    break;

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                    break;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangXuat()
        {
            SignInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("DangNhap");
        }
    }
}
