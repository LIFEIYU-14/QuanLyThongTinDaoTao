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
        // Khóa ngoại đến BuoiHoc
        [Required]
        public Guid BuoiHocId { get; set; }
        [ForeignKey("BuoiHocId")]
        public virtual BuoiHoc BuoiHoc { get; set; }
        // Khóa ngoại đến Attachment
        [Required]
        public Guid AttachmentId { get; set; }
        [ForeignKey("AttachmentId")]
        public virtual Attachment Attachment { get; set; }
    }

}