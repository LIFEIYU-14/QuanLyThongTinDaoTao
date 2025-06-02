using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using QuanLyThongTinDaoTao.Identity;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.ModelsHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class AdminAccountController : Controller
    {
        private AppUserManager userManager;
        private AppRoleManager roleManager;

        public AdminAccountController()
        {
            var context = new DbContextThongTinDaoTao();
            userManager = new AppUserManager(new UserStore<AppUser>(context));
            roleManager = new AppRoleManager(new RoleStore<IdentityRole>(context));
        }

        public ActionResult Index()
        {
            var adminRole = roleManager.FindByNameAsync("Admin").Result;
            var admins = new List<AppUser>();

            if (adminRole != null)
            {
                string adminRoleId = adminRole.Id;
                admins = userManager.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == adminRoleId))
                    .ToList();
            }

            var model = new AdminAccountViewModel
            {
                ExistingAdmins = admins
            };

            return View(model);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(AdminAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var adminRole = roleManager.FindByName("Admin");

            if (adminRole == null)
            {
                roleManager.Create(new IdentityRole("Admin"));
                adminRole = roleManager.FindByNameAsync("Admin").Result;
            }

            var existing = userManager.FindByEmail(model.Email);
            if (existing != null)
            {
                ViewBag.Message = "Email đã tồn tại!";
                model.ExistingAdmins = GetAdminUsers();
                return View("Index", model);
            }

            var newAdmin = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            var result = userManager.Create(newAdmin, model.Password);
            if (result.Succeeded)
            {
                userManager.AddToRole(newAdmin.Id, "Admin");
                ViewBag.Message = "Tạo tài khoản Admin thành công!";
            }
            else
            {
                ViewBag.Message = string.Join(", ", result.Errors);
            }

            model.ExistingAdmins = GetAdminUsers();
            return View("Index", model);
        }

        private List<AppUser> GetAdminUsers()
        {
            var adminRole = roleManager.FindByNameAsync("Admin").Result;
            if (adminRole == null) return new List<AppUser>();

            string adminRoleId = adminRole.Id;

            return userManager.Users
                .Where(u => u.Roles.Any(r => r.RoleId == adminRoleId))
                .ToList();
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var currentUserId = User.Identity.GetUserId();

            if (id == currentUserId)
            {
                TempData["Error"] = "Bạn không thể xóa tài khoản của chính mình.";
                return RedirectToAction("Index");
            }

            var user = userManager.FindById(id);

            if (user == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return RedirectToAction("Index");
            }

            // Kiểm tra nếu xóa xong sẽ không còn admin nào
            var adminRoleId = roleManager.FindByName("Admin")?.Id;
            int totalAdmins = userManager.Users
                .Count(u => u.Roles.Any(r => r.RoleId == adminRoleId));

            if (totalAdmins <= 1)
            {
                TempData["Error"] = "Không thể xóa. Hệ thống cần ít nhất một tài khoản Admin.";
                return RedirectToAction("Index");
            }

            // Thực hiện xóa
            var result = userManager.Delete(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Xóa tài khoản thành công.";
            }
            else
            {
                TempData["Error"] = "Xóa thất bại: " + string.Join(", ", result.Errors);
            }

            return RedirectToAction("Index");
        }

    }
}
