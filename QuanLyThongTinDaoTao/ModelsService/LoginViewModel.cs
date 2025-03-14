using System.ComponentModel.DataAnnotations;

namespace QuanLyThongTinDaoTao.ModelsService
{
	public class LoginViewModel
	{
        [Required(ErrorMessage = "Mã học viên không được để trống")]
        [StringLength(20, ErrorMessage = "Mã học viên không hợp lệ")]
        public string MaHocVien { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        public bool RememberMe { get; set; }
    }
}
