using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("GiangViens")]
    public class GiangVien : NguoiDung
    {
        [Required]
        [StringLength(20)]
        public string MaGiangVien { get; set; }
        [Required]
        [StringLength(255)]
        public string ChuyenMon { get; set; }

        [Required]
        [StringLength(255)]
        public string HocHam { get; set; }

        [Required(ErrorMessage = "Mật khẩu bắt buộc nhập")]
        [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
        public string MatKhau { get; set; }
        public string QR_Code_GV { get; set; } = Guid.NewGuid().ToString();
        public virtual ICollection<BuoiHoc> BuoiHocs { get; set; }
        public virtual ICollection<LopHoc> LopHocs { get; set; }
        public virtual ICollection<ThongBao> ThongBaos { get; set; }
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
