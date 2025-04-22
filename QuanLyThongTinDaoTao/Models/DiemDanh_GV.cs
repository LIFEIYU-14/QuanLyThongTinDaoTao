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

        [Required]
        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual GiangVien GiangVien { get; set; }

        [Required]
        public DateTime NgayDiemDanh { get; set; }

        public enum TrangThaiDiemDanhGV
        {
            CoMat,
            VangKhongPhep,
            VangCoPhep
        }

        [Required]
        public TrangThaiDiemDanhGV TrangThai { get; set; }
    }

}