using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.LoginModelView
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tài khoản hoặc email")]
        [Display(Name = "Tài khoản hoặc Email")]
        public string TaiKhoanOrEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }
    }
}
