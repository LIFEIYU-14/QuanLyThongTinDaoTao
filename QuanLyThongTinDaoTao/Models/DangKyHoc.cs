using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("DangKyHocs")]
    public class DangKyHoc
    {
        [Key]
        public Guid DangKyId { get; set; } = Guid.NewGuid();

        public Guid NguoiDungId { get; set; }

        [ForeignKey("NguoiDungId")]
        public virtual HocVien HocVien { get; set; } 

        public Guid LopHocId { get; set; }
        [ForeignKey("LopHocId")]
        public virtual LopHoc LopHoc { get; set; }

        public DateTime NgayDangKy { get; set; } = DateTime.Now;
        public bool IsConfirmed { get; set; } = false;
        public string ConfirmationToken { get; set; }
    }
}
