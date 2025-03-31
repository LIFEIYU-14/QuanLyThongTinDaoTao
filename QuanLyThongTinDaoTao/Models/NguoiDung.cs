using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Helpers;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("NguoiDungs")]
    public class NguoiDung
    {
        [Key]
        public Guid NguoiDungId { get; set; } = Guid.NewGuid();
        [Required]
        public string TaiKhoan { get; set; }
        [Required]
        public string MatKhau { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
        public virtual ICollection<PhanQuyen> PhanQuyens { get; set; } = new List<PhanQuyen>();

    }
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }

}
