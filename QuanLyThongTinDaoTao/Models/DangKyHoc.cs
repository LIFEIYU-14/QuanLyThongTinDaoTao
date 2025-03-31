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
        public virtual HocVien HocVien { get; set; }

        // Khóa ngoài đến LopHoc
        public Guid LopHocId { get; set; }
        public virtual LopHoc LopHoc { get; set; }
        public DateTime NgayDangKy { get; set; } = DateTime.Now;
        public bool IsConfirmed { get; set; } = false;  // Mặc định là chưa xác nhận
        public string ConfirmationToken { get; set; }
    }
}
