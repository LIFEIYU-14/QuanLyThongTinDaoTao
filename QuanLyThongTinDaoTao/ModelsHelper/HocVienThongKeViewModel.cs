using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.ModelsHelper
{
    public class HocVienThongKeViewModel
    {
        public string HocVienId { get; set; }
        public string MaHocVien { get; set; }
        public string HoVaTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string CoQuanLamViec { get; set; }
        public int SoLopDaDangKy { get; set; }
        public int SoBuoiDiHoc { get; set; }
        public int SoBuoiVang { get; set; }
        public int TongSoBuoi { get; set; }
        public double TiLeDiemDanh { get; set; }
        public bool IsConfirmed { get; set; }

        // Thông tin khóa học và lớp học
        public Guid LopHocId { get; set; }
        public string TenLopHoc { get; set; }
        public string TenKhoaHoc { get; set; }

    }

}