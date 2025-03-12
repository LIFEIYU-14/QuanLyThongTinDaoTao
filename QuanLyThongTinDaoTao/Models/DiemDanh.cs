using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("DiemDanhs")]

    public class DiemDanh
    {
        [Key]
        public Guid DiemDanhId { get; set; } = Guid.NewGuid();

        public virtual BuoiHoc BuoiHoc { get; set; }
        public virtual HocVien HocVien { get; set; }    
        [Required]
        public DateTime NgayDiemDanh { get; set; }

        public enum TrangThaiDiemDanh
        {
            CoMat, // Có mặt
            Vang,  // Vắng
            Muon   // Muộn
        }

        [Required]
        public TrangThaiDiemDanh TrangThai { get; set; }
    }
}