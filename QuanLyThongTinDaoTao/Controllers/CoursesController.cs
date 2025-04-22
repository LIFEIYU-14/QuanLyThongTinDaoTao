using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using QuanLyThongTinDaoTao.Models;
using QuanLyThongTinDaoTao.Services;
using PagedList;
using System.Collections.Generic;

namespace QuanLyThongTinDaoTao.Controllers
{
    public class CoursesController : Controller
    {
        private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();
        private readonly EmailService emailService = new EmailService();
        private readonly HocVienService hocVienService = new HocVienService(new DbContextThongTinDaoTao());

        public ActionResult Index()
        {
            var khoaHocs = db.KhoaHocs
                .Include(k => k.KhoaHocAttachments)
                .Take(3) // Chỉ lấy 3 khóa học đầu tiên
                .ToList();

            return View(khoaHocs);
        }
        public JsonResult LoadMoreCourses(int skip, int take)
        {
            var courses = db.KhoaHocs
                .OrderBy(k => k.KhoaHocId)
                .Skip(skip)
                .Take(take)
                .Include(k => k.KhoaHocAttachments)
                .ToList()  // Chuyển thành danh sách C# trước
                .Select(k => new
                {
                    KhoaHocId = k.KhoaHocId,
                    TenKhoaHoc = k.TenKhoaHoc,
                    ThoiLuong = k.ThoiLuong,
                    HinhDaiDienKhoaHoc = !string.IsNullOrEmpty(k.HinhDaiDienKhoaHoc)
                                         ? Url.Content(k.HinhDaiDienKhoaHoc)
                                         : (k.KhoaHocAttachments?
                                            .FirstOrDefault(a => a.Attachment != null &&
                                                 (a.Attachment.FileType.ToLower().Contains("jpg") ||
                                                  a.Attachment.FileType.ToLower().Contains("png") ||
                                                  a.Attachment.FileType.ToLower().Contains("jpeg")))?
                                            .Attachment?.FilePath ?? Url.Content("~/Upload/KhoaHoc/default.png"))
                })
                .ToList();

            return Json(courses, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DanhSachLopHoc(Guid? id)
        {

            var khoaHoc = db.KhoaHocs.Include(k => k.LopHocs)
                                     .Include(k => k.KhoaHocAttachments)
                                     .FirstOrDefault(k => k.KhoaHocId == id.Value);

            if (khoaHoc == null)
                return HttpNotFound("Không tìm thấy khóa học.");

            ViewBag.KhoaHoc = khoaHoc;
            return View(khoaHoc.LopHocs.ToList());
        }


        [HttpPost]
        public ActionResult Register(List<Guid> selectedLopHocIds)
        {
            if (selectedLopHocIds == null || !selectedLopHocIds.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một lớp học để đăng ký.";
                return RedirectToAction("Index");
            }

            // Lấy danh sách lớp học đã chọn và kèm thông tin khóa học
            var lopHocList = db.LopHocs
                .Include(l => l.KhoaHoc)
                .Where(l => selectedLopHocIds.Contains(l.LopHocId))
                .ToList();

            if (lopHocList.Count != selectedLopHocIds.Count)
            {
                TempData["Error"] = "Một số lớp học không tồn tại.";
                return RedirectToAction("Index");
            }

            return View("Register", lopHocList); // View mới
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
