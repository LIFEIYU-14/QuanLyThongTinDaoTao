using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.LoginModelView
{
    public class LoginViewModel
    {
        [Required]
        public string TaiKhoanOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
    }

}