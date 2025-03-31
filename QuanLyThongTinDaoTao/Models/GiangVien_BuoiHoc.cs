using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("GiangVien_BuoiHoc")]
    public class GiangVien_BuoiHoc
	{
		[Key]
        public Guid GiangVien_BuoiHocId { get; set; } = Guid.NewGuid();
		[Required]
        public virtual GiangVien GiangVien { get; set; }
        public virtual BuoiHoc BuoiHoc { get; set; }
    }
}