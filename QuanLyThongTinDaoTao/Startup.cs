using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using QuanLyThongTinDaoTao.Identity;
using QuanLyThongTinDaoTao.Models;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(QuanLyThongTinDaoTao.Startup))]

namespace QuanLyThongTinDaoTao
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(DbContextThongTinDaoTao.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);
            app.CreatePerOwinContext<AppSignInManager>(AppSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Admin/Login/DangNhap")
            });
            this.CreateDefaultRolesAndAdminUser(app);

        }
        private void CreateDefaultRolesAndAdminUser(IAppBuilder app)
        {
            using (var context = new DbContextThongTinDaoTao())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<AppUser>(context);
                var userManager = new AppUserManager(userStore);

                // Tạo các role nếu chưa có
                string[] roleNames = { "Admin", "GiangVien", "HocVien" };
                foreach (var role in roleNames)
                {
                    if (!roleManager.RoleExists(role))
                        roleManager.Create(new IdentityRole(role));
                }

                // Tạo tài khoản admin mặc định
                string adminEmail = "admin@qldt.com";
                string adminPassword = "Admin@123"; // Nên đổi sau
                var adminUser = userManager.FindByEmail(adminEmail);

                if (adminUser == null)
                {
                    var newAdmin = new AppUser
                    {
                        UserName = "admin",
                        Email = adminEmail,
                        PasswordHash = userManager.PasswordHasher.HashPassword(adminPassword),
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    };

                    var result = userManager.Create(newAdmin, adminPassword);
                    if (result.Succeeded)
                    {
                        userManager.AddToRole(newAdmin.Id, "Admin");
                    }
                }
            }
        }


    }
}
