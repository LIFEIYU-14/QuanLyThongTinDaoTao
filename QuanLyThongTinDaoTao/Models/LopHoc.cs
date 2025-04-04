﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Models
{
    public enum TrangThaiLopHoc
    {
        SapMo = 0,    // Giá trị số nguyên
        DangHoc = 1,
        DaKetThuc = 2
    }
    [Table("LopHocs")]
    public class LopHoc
    {
        [Key]
        public Guid LopHocId { get; set; } = Guid.NewGuid();
        [Required]
        public string MaLopHoc { get; set; }

        [Required]
        public string TenLopHoc { get; set; }

        [Required]
        public int SoTinChi { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(LopHoc), "ValidateNgayKetThuc")]
        public DateTime NgayKetThuc { get; set; }

        [Required]
        public TrangThaiLopHoc TrangThai { get; set; }
       
        [Required]
        public int SoLuongToiDa { get; set; }


        // Nội dung mô tả lớp học (hỗ trợ nhập bằng CKEditor)
        [AllowHtml]
        public string MoTa { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
        public virtual KhoaHoc KhoaHoc { get; set; }
        //public virtual ICollection<GiangVien> GiangViens { get; set; } = new List<GiangVien>();
        //public virtual ICollection<HocVien> HocViens { get; set; } = new List<HocVien>();
        public virtual ICollection<BuoiHoc> BuoiHocs { get; set; } = new List<BuoiHoc>();
        public virtual ICollection<LopHocAttachment> LopHocAttachments { get; set; } = new List<LopHocAttachment>();

        [NotMapped]
        public bool IsRegistered { get; set; }
        // Phương thức kiểm tra ngày kết thúc phải lớn hơn ngày bắt đầu
        public static ValidationResult ValidateNgayKetThuc(object value, ValidationContext context)
        {
            var lopHoc = (LopHoc)context.ObjectInstance;
            if (lopHoc.NgayBatDau >= lopHoc.NgayKetThuc)
            {
                return new ValidationResult("Ngày kết thúc phải lớn hơn ngày bắt đầu.");
            }
            return ValidationResult.Success;
        }
    }
}
