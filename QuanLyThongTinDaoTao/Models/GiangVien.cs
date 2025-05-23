﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("GiangViens")]
    public class GiangVien
    {
        [Key]
        public string GiangVienId { get; set; }

        public string MaGiangVien { get; set; }

        [Required]
        public string HoVaTen { get; set; }

        [Required]
        public DateTime NgaySinh { get; set; }

        [Required, StringLength(15)]
        public string SoDienThoai { get; set; }
        [Required]
        public string Email { get; set; }

        [Required, StringLength(255)]
        public string ChuyenMon { get; set; }

        [Required, StringLength(255)]
        public string HocHam { get; set; }
        public string QR_Code_GV { get; set; } = Guid.NewGuid().ToString();
        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; }
        public virtual ICollection<DiemDanh_GV> DiemDanh_GVs { get; set; }
        public virtual ICollection<ThongBao> ThongBaos { get; set; }
        public ICollection<GiangVien_BuoiHoc> GiangVien_BuoiHocs { get; internal set; }
    }
}
