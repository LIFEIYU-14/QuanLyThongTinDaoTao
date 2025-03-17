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
        public virtual LopHoc LopHoc { get; set; }
        public virtual Attachment Attachment { get; set; }
    }
}
