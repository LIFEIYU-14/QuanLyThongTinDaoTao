using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.Models
{
    [Table("KhoaHoc_Attachments")]
    public class KhoaHocAttachment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("KhoaHoc")]

        public virtual KhoaHoc KhoaHoc { get; set; }
        public virtual Attachment Attachment { get; set; }
    }

}