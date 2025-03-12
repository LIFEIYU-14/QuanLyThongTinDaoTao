using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.Models
{
    public class KhoaHoc
    {
        [Key]
        public Guid KhoaHocId { get; set; } = Guid.NewGuid();

        [Required, StringLength(255)]
        public string TenKhoaHoc { get; set; }

        [Required, StringLength(255)]
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }

        [Required]
        public int ThoiLuong { get; set; } // Tính theo tiết // 45p = 1 tiết
        public virtual ICollection<LopHoc> LopHocs { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }

}