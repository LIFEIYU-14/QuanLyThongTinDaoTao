using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.Models
{
    public class ChungChiHocTap
    {
        [Key]
        public Guid ChungChiId { get; set; } = Guid.NewGuid();

        [Required]
        public string HocVienId { get; set; }

        [ForeignKey("HocVienId")]
        public virtual HocVien HocVien { get; set; }

        [Required]
        public Guid KhoaHocId { get; set; }

        [ForeignKey("KhoaHocId")]
        public virtual KhoaHoc KhoaHoc { get; set; }

        public DateTime NgayCap { get; set; } = DateTime.Now;
        public DateTime NgayHetHan { get; set; }
    }

}