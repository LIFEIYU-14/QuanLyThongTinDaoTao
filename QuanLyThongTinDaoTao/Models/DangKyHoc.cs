using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("DangKyHocs")]
    public class DangKyHoc
    {
        [Key]
        public Guid DangKyId { get; set; } = Guid.NewGuid();

        public virtual HocVien HocVien { get; set; }

        public virtual LopHoc LopHoc { get; set; }

        public DateTime NgayDangKy { get; set; } = DateTime.Now;
    }
}