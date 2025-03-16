using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("Tailieus")]
    public class TaiLieu
    {
        [Key]
        public Guid TaiLieuId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(255)]
        public string TenTaiLieu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
        public virtual KhoaHoc KhoaHoc { get; set; }
    }
}