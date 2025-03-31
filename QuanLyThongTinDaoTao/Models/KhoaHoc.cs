using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("KhoaHocs")]
    public class KhoaHoc
    {
        [Key]
        public Guid KhoaHocId { get; set; } = Guid.NewGuid();
        // Hình đại diện cho khóa học
        [StringLength(500)]
        public string HinhDaiDienKhoaHoc { get; set; }
        [Required]
        public string MaKhoaHoc { get; set; }

        [Required, StringLength(255)]
        public string TenKhoaHoc { get; set; }

        // Nội dung mô tả lớp học (hỗ trợ nhập bằng CKEditor)
        [AllowHtml]
        public string MoTa { get; set; }

        [Required]
        public int ThoiLuong { get; set; } // Số tiết học
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public virtual ICollection<LopHoc> LopHocs { get; set; }
        public virtual ICollection<KhoaHocAttachment> KhoaHocAttachments { get; set; }

    }
}
