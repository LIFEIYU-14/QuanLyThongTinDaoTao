using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
	[Table("PhanQuyens")]
    public class PhanQuyen
	{
		[Key]
        public Guid PhanQuyenId { get; set; } = Guid.NewGuid();
		[Required]
		public string TenQuyen { get; set; }
        [Required]
        public Guid NguoiDungId { get; set; }  // Thêm khóa ngoại

        [ForeignKey("NguoiDungId")]
        public virtual NguoiDung NguoiDung { get; set; }

    }
}