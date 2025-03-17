using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("Attachments")]
    public class Attachment
    {
        [Key]
        public Guid AttachmentId { get; set; } = Guid.NewGuid();

        [Required, StringLength(255)]
        public string FileName { get; set; }

        [Required, StringLength(100)]
        public string FileType { get; set; } // PDF, DOCX, PNG, JPG

        [Required, StringLength(500)]
        public string FilePath { get; set; } // Lưu đường dẫn file

        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}
