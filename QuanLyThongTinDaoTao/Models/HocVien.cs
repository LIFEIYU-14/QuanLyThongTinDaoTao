using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("HocViens")]
    public class HocVien : NguoiDung
    {
        public string MaHocVien { get; set; }
        [Required(ErrorMessage = "Họ tên bắt buộc nhập")]
        public string HoVaTen { get; set; }
        [Required]
        public DateTime NgaySinh { get; set; }
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
        public string CoQuanLamViec { get; set; }
        public string QR_Code_HV { get; set; } = Guid.NewGuid().ToString();
        public bool IsConfirmed { get; set; } = false; // Chỉ lưu học viên khi xác nhận
        public string XacNhanToken { get; set; } // Token để xác nhận đăng ký
        //public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
        //public virtual ICollection<BuoiHoc> BuoiHocs { get; set; } = new List<BuoiHoc>();
        public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
        public virtual ICollection<DiemDanh_HV> DiemDanh_HVs { get; set; } = new List<DiemDanh_HV>();
    }
}
