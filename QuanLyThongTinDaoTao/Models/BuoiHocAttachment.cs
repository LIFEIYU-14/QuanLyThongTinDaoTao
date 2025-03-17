using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("BuoiHoc_Attachments")]
    public class BuoiHocAttachment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public virtual BuoiHoc BuoiHoc { get; set; } 
        public virtual Attachment Attachment { get; set; }
    }

}