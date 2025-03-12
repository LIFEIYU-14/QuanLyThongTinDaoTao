using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
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

        public virtual KhoaHoc KhoaHoc { get; set; }

        public virtual GiangVien GiangVien { get; set; }

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
        public virtual ICollection<HocVien> HocVien { get; set; }

        public virtual ICollection<BuoiHoc> BuoiHocs  { get; set; }

    }

}