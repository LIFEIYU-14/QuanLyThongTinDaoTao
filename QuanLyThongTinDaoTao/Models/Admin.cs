using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("Admins")]
    public class Admin : NguoiDung
    {
        public string MaNhanVien { get; set; }

    }
}