using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("KhoaHoc_Attachments")]
    public class KhoaHocAttachment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Khóa ngoại đến KhoaHoc
        [Required]
        public Guid KhoaHocId { get; set; }
        [ForeignKey("KhoaHocId")]
        public virtual KhoaHoc KhoaHoc { get; set; }

        // Khóa ngoại đến Attachment
        [Required]
        public Guid AttachmentId { get; set; }
        [ForeignKey("AttachmentId")]
        public virtual Attachment Attachment { get; set; }
    }
}
