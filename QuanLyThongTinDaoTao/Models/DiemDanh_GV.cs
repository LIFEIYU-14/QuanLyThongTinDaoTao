using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Windows.Data;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("DiemDanhs_GV")]
    public class DiemDanh_GV
	{
        [Key]
        public Guid DiemDanhId { get; set; } = Guid.NewGuid();
        [Required]
        public Guid BuoiHocId { get; set; }
        [ForeignKey("BuoiHocId")]
        public virtual BuoiHoc BuoiHoc { get; set; }
        public virtual GiangVien GiangVien { get; set; }
        [Required]
        public DateTime NgayDiemDanh { get; set; }

        public enum TrangThaiDiemDanhGV
        {
            CoMat, // Có mặt
            VangKhongPhep, // Vắng không phép
            VangCoPhep // Vắng có phép
        }

        [Required]
        public TrangThaiDiemDanhGV TrangThai { get; set; }
    }
}