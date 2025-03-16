using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("HinhAnhs")]
    public class HinhAnh
    {
        [Key]
        public Guid HinhAnhId { get; set; } = Guid.NewGuid();

        [Required, StringLength(500)]
        public string DuongDan { get; set; } // Đường dẫn ảnh

        [Required]
        public Guid DoiTuongId { get; set; } // ID của đối tượng liên quan (KhoaHoc, LopHoc, v.v.)

        [Required, StringLength(50)]
        public string LoaiDoiTuong { get; set; } // "KhoaHoc", "LopHoc", "BuoiHoc"

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public virtual KhoaHoc KhoaHoc { get; set; }
        public virtual LopHoc LopHoc { get; set; }
        public virtual BuoiHoc BuoiHoc { get; set; }
    }

}
