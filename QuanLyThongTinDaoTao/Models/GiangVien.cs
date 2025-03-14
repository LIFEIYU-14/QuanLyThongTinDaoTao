﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("GiangViens")]
    public class GiangVien : NguoiDung
    {
        [Required]
        [StringLength(20)]
        public string MaGiangVien { get; set; }
        [Required]
        [StringLength(255)]
        public string ChuyenMon { get; set; }

        public virtual ICollection<LopHoc> LopHocs { get; set; }
        public virtual ICollection<ThongBao> ThongBaos { get; set; }
    }
}
