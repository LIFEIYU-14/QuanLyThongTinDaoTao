using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.Helpers
{
    public static class QuyenConstants
    {
        public const string HOCVIEN = "HocVien";
        public const string GIANGVIEN = "GiangVien";
        public const string ADMIN = "Admin";

        public static List<string> DanhSachQuyen = new List<string>
        {
            HOCVIEN,
            GIANGVIEN,
            ADMIN
        };
        public static Dictionary<string, string> TenQuyenTiengViet = new Dictionary<string, string>
        {
            { HOCVIEN, "Học viên" },
            { GIANGVIEN, "Giảng viên" },
            { ADMIN, "Quản trị" }
        };
    }
}
