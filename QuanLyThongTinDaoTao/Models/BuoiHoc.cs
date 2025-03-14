﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("BuoiHocs")]

    public class BuoiHoc
    {
        [Key]
        public Guid BuoiHocId { get; set; } = Guid.NewGuid(); // Khóa chính


        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayHoc { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan GioKetThuc { get; set; }

        [Required]
        [StringLength(20)]
        public string TrangThai { get; set; } = "SapDienRa"; // Trạng thái buổi học

        public string GhiChu { get; set; }

        public string TaiLieuDinhKem { get; set; }

        public virtual LopHoc LopHoc { get; set; }
    }

}