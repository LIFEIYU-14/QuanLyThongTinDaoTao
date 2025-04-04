using QuanLyThongTinDaoTao.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace QuanLyThongTinDaoTao.Areas.Admin.Controllers
{
    public class DiemDanhHocVienController : Controller
    {
        private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

        // GET: Admin/DiemDanhHocVien
        public ActionResult Index()
        {
            // Lấy danh sách lớp học
            var lopHocs = db.LopHocs.ToList();
            ViewBag.LopHocList = lopHocs;

            return View();
        }
        [HttpPost]
        public ActionResult Index(Guid? LopHocId)
        {
            // Lấy danh sách lớp học
            var lopHocs = db.LopHocs.ToList();
            ViewBag.LopHocList = lopHocs;

            // Nếu lớp học được chọn
            if (LopHocId.HasValue)
            {
                // Lấy danh sách học viên đã đăng ký và xác nhận lớp học đã chọn
                var students = db.DangKyHocs
                    .Where(d => d.LopHocId == LopHocId.Value && d.IsConfirmed)
                    .Include(d => d.HocVien) // Đảm bảo lấy thông tin học viên
                    .ToList();

                // Lấy các buổi học (sessions) của lớp học đã chọn
                var buoiHocs = db.BuoiHocs
                    .Where(b => b.LopHoc.LopHocId == LopHocId.Value) // Sửa từ LopHoc.LopHocId thành LopHocId
                    .OrderBy(b => b.NgayHoc)
                    .ToList();

                // Truyền dữ liệu vào ViewBag để sử dụng trong View
                var model = new
                {
                    Students = students,
                    GroupedSessions = buoiHocs // Truyền trực tiếp danh sách buổi học
                };

                // Truyền lại giá trị LopHocId đã chọn vào ViewBag để hiển thị trên dropdown
                ViewBag.SelectedLopHocId = LopHocId;

                return PartialView("_DanhSachHocVienPartial", model); // Trả lại partial view
            }

            return View();
        }


    }

}
