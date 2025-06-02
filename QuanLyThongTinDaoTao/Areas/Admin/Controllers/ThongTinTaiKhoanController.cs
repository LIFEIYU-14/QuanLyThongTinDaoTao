using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuanLyThongTinDaoTao.Identity;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    [Authorize]
    public class ThongTinTaiKhoanController : Controller
    {
        private DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        private AppUserManager _userManager;
        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set { _userManager = value; }
        }

        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();
            var giangVien = db.GiangViens.FirstOrDefault(g => g.AppUserId == currentUserId);

            var vm = new ThongTinTaiKhoanViewModel();

            if (giangVien != null)
            {
                vm.Role = "GiangVien";
                vm.Username = giangVien.AppUser?.UserName;
                vm.Email = giangVien.Email;
                vm.NgayTao = giangVien.AppUser.NgayTao;
                vm.NgayCapNhat = giangVien.AppUser.NgayCapNhat;

                vm.MaGiangVien = giangVien.MaGiangVien;
                vm.HoVaTen = giangVien.HoVaTen;
                vm.NgaySinh = giangVien.NgaySinh;
                vm.SoDienThoai = giangVien.SoDienThoai;
                vm.ChuyenMon = giangVien.ChuyenMon;
                vm.HocHam = giangVien.HocHam;
            }
            else
            {
                var user = db.Users.FirstOrDefault(u => u.Id == currentUserId);
                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin tài khoản.";
                    return RedirectToAction("Index", "Home");
                }

                vm.Role = "Admin";
                vm.Username = user.UserName;
                vm.Email = user.Email;
                vm.NgayTao = user.NgayTao;
                vm.NgayCapNhat = user.NgayCapNhat;
            }

            return View(vm);
        }
        public ActionResult Edit()
        {
            var userId = User.Identity.GetUserId();
            var appUser = db.Users.Find(userId);
            if (appUser == null) return HttpNotFound();

            var viewModel = new ThongTinTaiKhoanViewModel
            {
                AppUserId = appUser.Id,
                Username = appUser.UserName,
                Email = appUser.Email,
                NgayTao = appUser.NgayTao,
                NgayCapNhat = appUser.NgayCapNhat,
                Role = User.IsInRole("GiangVien") ? "GiangVien" : "Admin"
            };

            if (viewModel.Role == "GiangVien")
            {
                var gv = db.GiangViens.FirstOrDefault(g => g.AppUserId == userId);
                if (gv != null)
                {
                    viewModel.GiangVienId = gv.GiangVienId;
                    viewModel.MaGiangVien = gv.MaGiangVien;
                    viewModel.HoVaTen = gv.HoVaTen;
                    viewModel.NgaySinh = gv.NgaySinh;
                    viewModel.SoDienThoai = gv.SoDienThoai;
                    viewModel.ChuyenMon = gv.ChuyenMon;
                    viewModel.HocHam = gv.HocHam;
                }

            }

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ThongTinTaiKhoanViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var appUser = db.Users.Find(model.AppUserId);
            if (appUser == null) return HttpNotFound();

            appUser.Email = model.Email;
            appUser.UserName = model.Username;
            appUser.NgayCapNhat = DateTime.Now;

            if (User.IsInRole("GiangVien") && !string.IsNullOrEmpty(model.GiangVienId))
            {
                var gv = db.GiangViens.Find(model.GiangVienId);
                if (gv != null)
                {
                    gv.HoVaTen = model.HoVaTen;
                    gv.NgaySinh = model.NgaySinh;
                    gv.SoDienThoai = model.SoDienThoai;
                    gv.ChuyenMon = model.ChuyenMon;
                    gv.HocHam = model.HocHam;
                    gv.Email = model.Email;
                }
            }

            db.SaveChanges();
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }
        // GET: Hiển thị form đổi mật khẩu
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Xử lý đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.Identity.GetUserId();

            var result = await UserManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Đổi mật khẩu thành công.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(model);
            }
        }
    }
}
