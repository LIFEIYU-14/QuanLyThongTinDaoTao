using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace QuanLyThongTinDaoTao.ModelsService
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string HoVaTen { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email sai định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string XacNhanMatKhau { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số ngẫu nhiên")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Số ngẫu nhiên phải có đúng 4 chữ số")]
        public string SoNgauNhien { get; set; }

        // Hàm kiểm tra ngày sinh
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NgaySinh.HasValue)
            {
                if (NgaySinh.Value.Year < 1950)
                {
                    yield return new ValidationResult("Ngày sinh không hợp lệ. Chỉ chấp nhận từ năm 1950 trở đi.", new[] { "NgaySinh" });
                }
            }
        }
    }
}
