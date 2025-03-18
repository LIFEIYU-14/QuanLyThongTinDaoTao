using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("HocViens")]
    public class HocVien : NguoiDung
    {
        [Required]
        [StringLength(20)]
        public string MaHocVien { get; set; }
        [Required]
        [StringLength(255)]
        public string CoQuanLamViec { get; set; }
        [Required]
        public string QR_Code_HV { get; set; } = Guid.NewGuid().ToString();
        public virtual ICollection<LopHoc> LopHocs { get; set; }
        public virtual ICollection<ThongBao> ThongBaos { get; set; }
    }
}
