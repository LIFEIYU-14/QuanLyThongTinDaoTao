using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("LopHoc_Attachments")]
    public class LopHocAttachment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Khóa ngoại đến LopHoc
        [Required]
        public Guid LopHocId { get; set; }
        [ForeignKey("LopHocId")]
        public virtual LopHoc LopHoc { get; set; }

        [Required]
        public Guid AttachmentId { get; set; }
        [ForeignKey("AttachmentId")]
        public virtual Attachment Attachment { get; set; }
    }
}
