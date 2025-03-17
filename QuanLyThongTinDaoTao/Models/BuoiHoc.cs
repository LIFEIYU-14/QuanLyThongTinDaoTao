using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    public enum TrangThaiBuoiHoc
    {
        SapDienRa,
        DangDienRa,
        DaKetThuc
    }
    [Table("BuoiHocs")]

    public class BuoiHoc
    {
        [Key]
        public Guid BuoiHocId { get; set; } = Guid.NewGuid();

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayHoc { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [CustomValidation(typeof(BuoiHoc), "ValidateGioKetThuc")]
        public TimeSpan GioKetThuc { get; set; }

        [Required]
        public TrangThaiBuoiHoc TrangThai { get; set; } = TrangThaiBuoiHoc.SapDienRa;

        public string GhiChu { get; set; }

        public virtual LopHoc LopHoc { get; set; }
        public virtual GiangVien GiangVien { get; set; }
        public virtual ICollection<BuoiHocAttachment> BuoiHocAttachments { get; set; }
        public static ValidationResult ValidateGioKetThuc(TimeSpan gioKetThuc, ValidationContext context)
        {
            var instance = (BuoiHoc)context.ObjectInstance;
            if (gioKetThuc <= instance.GioBatDau)
            {
                return new ValidationResult("Giờ kết thúc phải lớn hơn giờ bắt đầu.");
            }
            return ValidationResult.Success;
        }
    }
}
