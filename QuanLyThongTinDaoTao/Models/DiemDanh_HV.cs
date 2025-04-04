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
        public Guid NguoiDungId { get; set; }
        [ForeignKey("NguoiDungId")]
        public virtual HocVien HocVien { get; set; }    
        public DateTime NgayDiemDanh { get; set; }

        public enum TrangThaiDiemDanhHV
        {
            CoMat, // Có mặt
            VangKhongPhep, // Vắng không phép
            VangCoPhep // Vắng có phép
        }

        [Required]
        public TrangThaiDiemDanhHV TrangThai { get; set; }
    }
}