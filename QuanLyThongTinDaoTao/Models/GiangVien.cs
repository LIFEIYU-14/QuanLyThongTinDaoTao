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

        [Required(ErrorMessage = "Họ tên bắt buộc nhập")]
        public string HoVaTen { get; set; }
        [Required]
        public DateTime? NgaySinh { get; set; }
        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email sai định dạng")]
        [Index(IsUnique = true)]
        public string Email { get; set; }
        [Required]
        [StringLength(255)]
        public string ChuyenMon { get; set; }

        [Required]
        [StringLength(255)]
        public string HocHam { get; set; }
        public string QR_Code_GV { get; set; } = Guid.NewGuid().ToString();
        public virtual ICollection<BuoiHoc> BuoiHocs { get; set; }
        public virtual ICollection<LopHoc> LopHocs { get; set; }
        public virtual ICollection<ThongBao> ThongBaos { get; set; }
    }
    
}
