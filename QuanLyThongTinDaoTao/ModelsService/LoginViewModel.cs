using System.ComponentModel.DataAnnotations;

namespace QuanLyThongTinDaoTao.ModelsService
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mã xác nhận không được để trống")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Mã xác nhận phải có đúng 6 chữ số")]
        [Display(Name = "Mã xác nhận")]
        public string MaXacNhan { get; set; }
    }
}
