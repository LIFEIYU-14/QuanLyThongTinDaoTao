using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.Models
{
    public class HocVienTempData
    {
        public string HoVaTen { get; set; }
        public string Email { get; set; }
        public DateTime NgaySinh { get; set; }
        public string SoDienThoai { get; set; }
        public string CoQuanLamViec { get; set; }
        public Guid LopHocId { get; set; }
    }

}