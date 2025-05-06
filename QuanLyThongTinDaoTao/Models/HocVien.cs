using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("HocViens")]
    public class HocVien
    {
        [Key]
        public string HocVienId { get; set; } = Guid.NewGuid().ToString();
        public string MaHocVien { get; set; }

        [Required]
        public string HoVaTen { get; set; }

        [Required]
        public DateTime NgaySinh { get; set; }

        [Required, StringLength(15)]
        public string SoDienThoai { get; set; }
        [Required]
        public string Email { get; set; }

        [Required, StringLength(255)]
        public string CoQuanLamViec { get; set; }

        public string QR_Code_HV { get; set; } = Guid.NewGuid().ToString();
        public bool IsConfirmed { get; set; } = false;
        public string XacNhanToken { get; set; }
        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; }
        public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
        public virtual ICollection<DiemDanh_HV> DiemDanh_HVs { get; set; } = new List<DiemDanh_HV>();
    }
}
