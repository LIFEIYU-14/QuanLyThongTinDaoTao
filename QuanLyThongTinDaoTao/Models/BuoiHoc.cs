using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Models
{
    public enum TrangThaiBuoiHoc
    {
        SapDienRa,
        DangDienRa,
        DaKetThuc
    }
    [Table("BuoiHocs")]

    public class BuoiHoc :IValidatableObject
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
        // Custom Validation cho giờ kết thúc đã có sẵn
        public static ValidationResult ValidateGioKetThuc(TimeSpan gioKetThuc, ValidationContext context)
        {
            var instance = (BuoiHoc)context.ObjectInstance;
            if (gioKetThuc <= instance.GioBatDau)
            {
                return new ValidationResult("Giờ kết thúc phải lớn hơn giờ bắt đầu.");
            }
            return ValidationResult.Success;
        }

        // Thực hiện validation để kiểm tra NgàyHoc nằm trong khoảng thời gian của lớp học
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (LopHoc != null)
            {
                if (NgayHoc < LopHoc.NgayBatDau || NgayHoc > LopHoc.NgayKetThuc)
                {
                    yield return new ValidationResult(
                        "Ngày học phải nằm trong khoảng thời gian của lớp học.",
                        new[] { "NgayHoc" }
                    );
                }
            }
        }
    }
}
