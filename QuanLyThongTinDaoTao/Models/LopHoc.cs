using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("LopHocs")]
    public class LopHoc
    {
        [Key]
        public Guid LopHocId { get; set; } = Guid.NewGuid();

        [Required]
        public string MaLopHoc { get; set; }

        [Required]
        public string TenLopHoc { get; set; }

        [Required]
        public int SoTinChi { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        [Required]
        public string TrangThai { get; set; } // SapMo, DangHoc, DaKetThuc

        [Required]
        public int SoLuongToiDa { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
        public virtual KhoaHoc KhoaHoc { get; set; }
        public virtual ICollection<GiangVien> GiangViens { get; set; }
        public virtual ICollection<HinhAnh> HinhAnhs { get; set; }

    }
}
