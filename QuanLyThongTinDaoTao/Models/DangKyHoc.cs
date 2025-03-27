using QuanLyThongTinDaoTao.Models;
using System.ComponentModel.DataAnnotations;
using System;

public class DangKyHoc
{
    [Key]
    public Guid DangKyId { get; set; } = Guid.NewGuid();

    public Guid NguoiDungId { get; set; }
    public virtual HocVien HocVien { get; set; }

    // Khóa ngoài đến LopHoc
    public Guid LopHocId { get; set; }
    public virtual LopHoc LopHoc { get; set; }

    // Khóa ngoài đến KhoaHoc
    public Guid KhoaHocId { get; set; }
    public virtual KhoaHoc KhoaHoc { get; set; }

    public DateTime NgayDangKy { get; set; } = DateTime.Now;
}
