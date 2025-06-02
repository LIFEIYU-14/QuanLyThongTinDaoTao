using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.ModelsHelper
{
    public class ThongTinTaiKhoanViewModel
    {
        public string AppUserId { get; set; }
        public string GiangVienId { get; set; }

        public string Role { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }

        public string MaGiangVien { get; set; }
        public string HoVaTen { get; set; }

        public DateTime NgaySinh { get; set; } // Cho phép null
        public string SoDienThoai { get; set; }
        public string ChuyenMon { get; set; }
        public string HocHam { get; set; }
    }


}