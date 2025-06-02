using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuanLyThongTinDaoTao.Models;

namespace QuanLyThongTinDaoTao.ModelsHelper
{
    public class AdminAccountViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<AppUser> ExistingAdmins { get; set; } = new List<AppUser>();
    }
}