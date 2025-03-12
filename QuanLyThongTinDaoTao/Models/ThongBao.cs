using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("ThongBaos")]
    public class ThongBao
    {
        [Key]
        public Guid ThongBaoId { get; set; } = Guid.NewGuid();

        [Required]
        public string TieuDe { get; set; }

        [Required]
        public string NoiDung { get; set; }

        public DateTime NgayGui { get; set; } = DateTime.Now;
        public ICollection<HocVien> HocViens { get; set; }
        public ICollection<GiangVien> GiangViens { get; set; }

    }

}