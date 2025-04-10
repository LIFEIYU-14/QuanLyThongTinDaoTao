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

        [AllowHtml]
        public string GhiChu { get; set; }
        [Required]
        public Guid LopHocId { get; set; }
        [ForeignKey("LopHocId")]
        public virtual LopHoc LopHoc { get; set; }
        public virtual ICollection<GiangVien_BuoiHoc> GiangVien_BuoiHocs { get; set; } = new List<GiangVien_BuoiHoc>();
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

        // Thực hiện validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (GioKetThuc <= GioBatDau)
            {
                yield return new ValidationResult(
                    "Giờ kết thúc phải lớn hơn giờ bắt đầu.",
                    new[] { "GioKetThuc" }
                );
            }

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
        // Thuộc tính tính toán trạng thái buổi học theo thời gian thực
        [NotMapped]
        public TrangThaiBuoiHoc CurrentTrangThai
        {
            get
            {
                // Ghép ngày và giờ để tạo DateTime thực tế của buổi học
                DateTime startDateTime = NgayHoc.Date.Add(GioBatDau);
                DateTime endDateTime = NgayHoc.Date.Add(GioKetThuc);
                DateTime now = DateTime.Now;

                if (now < startDateTime)
                    return TrangThaiBuoiHoc.SapDienRa;
                else if (now >= startDateTime && now < endDateTime)
                    return TrangThaiBuoiHoc.DangDienRa;
                else
                    return TrangThaiBuoiHoc.DaKetThuc;
            }
        }
    }
}
