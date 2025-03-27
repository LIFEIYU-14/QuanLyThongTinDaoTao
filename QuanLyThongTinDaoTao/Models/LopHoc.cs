using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("LopHocs")]
    public class LopHoc
    {
        [Key]
        public Guid LopHocId { get; set; } = Guid.NewGuid();

        [Required]
        public string TenLopHoc { get; set; }

        [Required]
        public int SoTinChi { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        [Required]
        public string TrangThai { get; set; } // SapMo, DangHoc, DaKetThuc

        [Required]
        public int SoLuongToiDa { get; set; }


        // Nội dung mô tả lớp học (hỗ trợ nhập bằng CKEditor)
        [AllowHtml]
        public string MoTa { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
        public virtual KhoaHoc KhoaHoc { get; set; }

        public virtual ICollection<GiangVien> GiangViens { get; set; }
        public virtual ICollection<HocVien> HocViens { get; set; }
        public virtual ICollection<BuoiHoc> BuoiHocs { get; set; }
        public virtual ICollection<LopHocAttachment> LopHocAttachments { get; set; }
        [NotMapped]
        public bool IsRegistered { get; set; }
    }
}
