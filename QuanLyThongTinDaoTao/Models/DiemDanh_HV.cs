using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("DiemDanhs_HV")]

    public class DiemDanh_HV
    {
        [Key]
        public Guid DiemDanhId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid BuoiHocId { get; set; }

        [ForeignKey("BuoiHocId")]
        public virtual BuoiHoc BuoiHoc { get; set; }

        [Required]
        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual HocVien HocVien { get; set; }

        public DateTime NgayDiemDanh { get; set; }

        public enum TrangThaiDiemDanhHV
        {
            CoMat,
            VangKhongPhep,
            VangCoPhep
        }

        [Required]
        public TrangThaiDiemDanhHV TrangThai { get; set; }
    }

}