using QuanLyThongTinDaoTao.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using System.Linq;
using System.Data.Entity;
using static QuanLyThongTinDaoTao.Models.DiemDanh_HV;

public class DiemDanhHocVienController : Controller
{
    private readonly DbContextThongTinDaoTao db = new DbContextThongTinDaoTao();

    // GET: Admin/DiemDanhHocVien/Index
    public ActionResult Index(Guid lopHocId, Guid buoiHocId)
    {
        // Get the class (LopHoc) by Id
        var lopHoc = db.LopHocs.FirstOrDefault(lh => lh.LopHocId == lopHocId);
        if (lopHoc == null)
        {
            return HttpNotFound("Không tìm thấy lớp học !");
        }

        // Get the list of students enrolled in the class
        var hocVien = db.DangKyHocs
                         .Where(dk => dk.LopHocId == lopHocId && dk.IsConfirmed)
                         .Select(dk => dk.HocVien)
                         .ToList();

        // Get the session (BuoiHoc) for attendance
        var buoiHoc = db.BuoiHocs.FirstOrDefault(bh => bh.BuoiHocId == buoiHocId);
        if (buoiHoc == null)
        {
            return HttpNotFound("Không tìm thấy buổi học.");
        }

        // Pass students and session to the view
        ViewBag.LopHoc = lopHoc;
        ViewBag.BuoiHoc = buoiHoc;
        return View(hocVien);
    }
    // POST: Admin/DiemDanhHocVien/Index
    [HttpPost]
    public ActionResult MarkAttendance(Guid lopHocId, Guid buoiHocId, Dictionary<Guid, bool> attendance)
    {
        var lopHoc = db.LopHocs.FirstOrDefault(lh => lh.LopHocId == lopHocId);
        if (lopHoc == null)
        {
            return HttpNotFound("Class not found.");
        }

        var buoiHoc = db.BuoiHocs.FirstOrDefault(bh => bh.BuoiHocId == buoiHocId);
        if (buoiHoc == null)
        {
            return HttpNotFound("Session not found.");
        }

        // Loop through each student's attendance status and save it
        foreach (var item in attendance)
        {
            var studentId = item.Key;
            var isPresent = item.Value;

            // Create or update attendance record
            var attendanceRecord = db.DiemDanhs_HVs
                                     .FirstOrDefault(dh => dh.NguoiDungId == studentId && dh.BuoiHocId == buoiHocId);

            if (attendanceRecord == null)
            {
                attendanceRecord = new DiemDanh_HV
                {
                    NguoiDungId = studentId,
                    BuoiHocId = buoiHocId,
                    TrangThai = isPresent ? TrangThaiDiemDanhHV.CoMat : TrangThaiDiemDanhHV.VangKhongPhep
                };
                db.DiemDanhs_HVs.Add(attendanceRecord);
            }
            else
            {
                attendanceRecord.TrangThai = isPresent ? TrangThaiDiemDanhHV.CoMat : TrangThaiDiemDanhHV.VangKhongPhep;
            }
        }

        db.SaveChanges();
        return RedirectToAction("Index", new { lopHocId = lopHocId, buoiHocId = buoiHocId });
    }
}
