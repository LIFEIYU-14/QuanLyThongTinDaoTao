﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Helpers;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("NguoiDungs")]
    public class NguoiDung
    {
        [Key]
        public Guid NguoiDungId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Họ tên bắt buộc nhập")]
        public string HoVaTen { get; set; }

        [Required(ErrorMessage = "Mật khẩu bắt buộc nhập")]
        [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
        public string MatKhau { get; set; }

        [Required]
        public VaiTroNguoiDung VaiTro { get; set; }

        [Required]
        public DateTime? NgaySinh { get; set; }

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email sai định dạng")]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }
    public enum VaiTroNguoiDung
    {
        Admin,
        GiangVien,
        HocVien
    }
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
