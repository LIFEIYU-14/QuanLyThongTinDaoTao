using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("GiangVien_BuoiHoc")]
    public class GiangVien_BuoiHoc
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid BuoiHocId { get; set; }

        [ForeignKey("BuoiHocId")]
        public virtual BuoiHoc BuoiHoc { get; set; }

        [Required]
        public string GiangVienId { get; set; }

        [ForeignKey("GiangVienId")]
        public virtual GiangVien GiangVien { get; set; }
    }

}