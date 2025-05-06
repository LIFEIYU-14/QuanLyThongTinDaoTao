using QuanLyThongTinDaoTao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.ModelsHelper
{
    public class LopHocDetailsViewModel
    {
        public LopHoc LopHoc { get; set; }
        public List<DangKyHoc> DangKyHocs { get; set; }
    }

}