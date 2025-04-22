using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using QuanLyThongTinDaoTao.Identity;
using QuanLyThongTinDaoTao.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

public class AppUser : IdentityUser
{
    public DateTime NgayTao { get; set; } = DateTime.Now;
    public DateTime NgayCapNhat { get; set; } = DateTime.Now;

    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(AppUserManager userManager)
    {
        var userIdentity = await userManager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

        return userIdentity;
    }
}
