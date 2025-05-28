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
            var khoaHoc = db.KhoaHocs
                .Include(k => k.LopHocs.Select(l => l.BuoiHocs))
                .Include(k => k.KhoaHocAttachments)
                .FirstOrDefault(k => k.KhoaHocId == id.Value);

            if (khoaHoc == null)
                return HttpNotFound("Không tìm thấy khóa học.");

            var lopHocList = khoaHoc.LopHocs.ToList();

            // Tạo dictionary: LopHocId => Danh sách LopHocId trùng giờ
            var lopTrungGioMap = new Dictionary<Guid, List<Guid>>();

            for (int i = 0; i < lopHocList.Count; i++)
            {
                var lopI = lopHocList[i];
                var buoiI = lopI.BuoiHocs;

                for (int j = i + 1; j < lopHocList.Count; j++)
                {
                    var lopJ = lopHocList[j];
                    var buoiJ = lopJ.BuoiHocs;

                    bool trungGio = buoiI.Any(b1 => buoiJ.Any(b2 =>
                        b1.NgayHoc == b2.NgayHoc &&
                        b1.GioBatDau < b2.GioKetThuc &&
                        b2.GioBatDau < b1.GioKetThuc));

                    if (trungGio)
                    {
                        if (!lopTrungGioMap.ContainsKey(lopI.LopHocId))
                            lopTrungGioMap[lopI.LopHocId] = new List<Guid>();
                        if (!lopTrungGioMap.ContainsKey(lopJ.LopHocId))
                            lopTrungGioMap[lopJ.LopHocId] = new List<Guid>();

                        lopTrungGioMap[lopI.LopHocId].Add(lopJ.LopHocId);
                        lopTrungGioMap[lopJ.LopHocId].Add(lopI.LopHocId);
                    }
                }
            }

            ViewBag.KhoaHoc = khoaHoc;
            ViewBag.LopTrungGioMap = lopTrungGioMap;

            return View(lopHocList);
        }




        [HttpPost]
        public ActionResult Register(List<Guid> selectedLopHocIds)
        {
            if (selectedLopHocIds == null || !selectedLopHocIds.Any())
            {
                return new HttpStatusCodeResult(400, "Vui lòng chọn ít nhất một lớp học.");
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
