namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2204 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        AttachmentId = c.Guid(nullable: false),
                        FileName = c.String(nullable: false, maxLength: 255),
                        FileType = c.String(nullable: false, maxLength: 100),
                        FilePath = c.String(nullable: false, maxLength: 500),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AttachmentId);
            
            CreateTable(
                "dbo.BuoiHoc_Attachments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BuoiHocId = c.Guid(nullable: false),
                        AttachmentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId, cascadeDelete: true)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHocId, cascadeDelete: true)
                .Index(t => t.BuoiHocId)
                .Index(t => t.AttachmentId);
            
            CreateTable(
                "dbo.BuoiHocs",
                c => new
                    {
                        BuoiHocId = c.Guid(nullable: false),
                        NgayHoc = c.DateTime(nullable: false),
                        GioBatDau = c.Time(nullable: false, precision: 7),
                        GioKetThuc = c.Time(nullable: false, precision: 7),
                        TrangThai = c.Int(nullable: false),
                        GhiChu = c.String(),
                        LopHocId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.BuoiHocId)
                .ForeignKey("dbo.LopHocs", t => t.LopHocId, cascadeDelete: true)
                .Index(t => t.LopHocId);
            
            CreateTable(
                "dbo.GiangVien_BuoiHoc",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BuoiHocId = c.Guid(nullable: false),
                        AppUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHocId, cascadeDelete: true)
                .ForeignKey("dbo.GiangViens", t => t.AppUserId)
                .Index(t => t.BuoiHocId)
                .Index(t => t.AppUserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        GiangVien_Id = c.String(maxLength: 128),
                        HocVien_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GiangViens", t => t.GiangVien_Id)
                .ForeignKey("dbo.HocViens", t => t.HocVien_Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.GiangVien_Id)
                .Index(t => t.HocVien_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DiemDanhs_GV",
                c => new
                    {
                        DiemDanhId = c.Guid(nullable: false),
                        BuoiHocId = c.Guid(nullable: false),
                        AppUserId = c.String(nullable: false, maxLength: 128),
                        NgayDiemDanh = c.DateTime(nullable: false),
                        TrangThai = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DiemDanhId)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHocId, cascadeDelete: true)
                .ForeignKey("dbo.GiangViens", t => t.AppUserId)
                .Index(t => t.BuoiHocId)
                .Index(t => t.AppUserId);
            
            CreateTable(
                "dbo.DangKyHocs",
                c => new
                    {
                        DangKyId = c.Guid(nullable: false),
                        AppUserId = c.String(nullable: false, maxLength: 128),
                        LopHocId = c.Guid(nullable: false),
                        NgayDangKy = c.DateTime(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        ConfirmationToken = c.String(),
                    })
                .PrimaryKey(t => t.DangKyId)
                .ForeignKey("dbo.HocViens", t => t.AppUserId)
                .ForeignKey("dbo.LopHocs", t => t.LopHocId, cascadeDelete: true)
                .Index(t => t.AppUserId)
                .Index(t => t.LopHocId);
            
            CreateTable(
                "dbo.LopHocs",
                c => new
                    {
                        LopHocId = c.Guid(nullable: false),
                        MaLopHoc = c.String(nullable: false),
                        TenLopHoc = c.String(nullable: false),
                        SoTinChi = c.Int(nullable: false),
                        NgayBatDau = c.DateTime(nullable: false),
                        NgayKetThuc = c.DateTime(nullable: false),
                        TrangThai = c.Int(nullable: false),
                        SoLuongToiDa = c.Int(nullable: false),
                        MoTa = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                        KhoaHocId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.LopHocId)
                .ForeignKey("dbo.KhoaHocs", t => t.KhoaHocId, cascadeDelete: true)
                .Index(t => t.KhoaHocId);
            
            CreateTable(
                "dbo.KhoaHocs",
                c => new
                    {
                        KhoaHocId = c.Guid(nullable: false),
                        HinhDaiDienKhoaHoc = c.String(maxLength: 500),
                        MaKhoaHoc = c.String(nullable: false),
                        TenKhoaHoc = c.String(nullable: false, maxLength: 255),
                        MoTa = c.String(),
                        ThoiLuong = c.Int(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.KhoaHocId);
            
            CreateTable(
                "dbo.KhoaHoc_Attachments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        KhoaHocId = c.Guid(nullable: false),
                        AttachmentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId, cascadeDelete: true)
                .ForeignKey("dbo.KhoaHocs", t => t.KhoaHocId, cascadeDelete: true)
                .Index(t => t.KhoaHocId)
                .Index(t => t.AttachmentId);
            
            CreateTable(
                "dbo.LopHoc_Attachments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LopHocId = c.Guid(nullable: false),
                        AttachmentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId, cascadeDelete: true)
                .ForeignKey("dbo.LopHocs", t => t.LopHocId, cascadeDelete: true)
                .Index(t => t.LopHocId)
                .Index(t => t.AttachmentId);
            
            CreateTable(
                "dbo.DiemDanhs_HV",
                c => new
                    {
                        DiemDanhId = c.Guid(nullable: false),
                        BuoiHocId = c.Guid(nullable: false),
                        AppUserId = c.String(nullable: false, maxLength: 128),
                        NgayDiemDanh = c.DateTime(nullable: false),
                        TrangThai = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DiemDanhId)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHocId, cascadeDelete: true)
                .ForeignKey("dbo.HocViens", t => t.AppUserId)
                .Index(t => t.BuoiHocId)
                .Index(t => t.AppUserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ThongBaos",
                c => new
                    {
                        ThongBaoId = c.Guid(nullable: false),
                        TieuDe = c.String(nullable: false),
                        NoiDung = c.String(nullable: false),
                        NgayGui = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ThongBaoId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ThongBaoGiangViens",
                c => new
                    {
                        ThongBao_ThongBaoId = c.Guid(nullable: false),
                        GiangVien_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ThongBao_ThongBaoId, t.GiangVien_Id })
                .ForeignKey("dbo.ThongBaos", t => t.ThongBao_ThongBaoId, cascadeDelete: true)
                .ForeignKey("dbo.GiangViens", t => t.GiangVien_Id, cascadeDelete: true)
                .Index(t => t.ThongBao_ThongBaoId)
                .Index(t => t.GiangVien_Id);
            
            CreateTable(
                "dbo.GiangViens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        MaGiangVien = c.String(),
                        HoVaTen = c.String(nullable: false),
                        NgaySinh = c.DateTime(nullable: false),
                        SoDienThoai = c.String(nullable: false, maxLength: 15),
                        ChuyenMon = c.String(nullable: false, maxLength: 255),
                        HocHam = c.String(nullable: false, maxLength: 255),
                        QR_Code_GV = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.HocViens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        MaHocVien = c.String(),
                        HoVaTen = c.String(nullable: false),
                        NgaySinh = c.DateTime(nullable: false),
                        SoDienThoai = c.String(nullable: false, maxLength: 15),
                        CoQuanLamViec = c.String(nullable: false, maxLength: 255),
                        QR_Code_HV = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                        XacNhanToken = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HocViens", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.GiangViens", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "HocVien_Id", "dbo.HocViens");
            DropForeignKey("dbo.AspNetUsers", "GiangVien_Id", "dbo.GiangViens");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ThongBaoGiangViens", "GiangVien_Id", "dbo.GiangViens");
            DropForeignKey("dbo.ThongBaoGiangViens", "ThongBao_ThongBaoId", "dbo.ThongBaos");
            DropForeignKey("dbo.DiemDanhs_HV", "AppUserId", "dbo.HocViens");
            DropForeignKey("dbo.DiemDanhs_HV", "BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.DangKyHocs", "LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.LopHoc_Attachments", "LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.LopHoc_Attachments", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.LopHocs", "KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.KhoaHoc_Attachments", "KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.KhoaHoc_Attachments", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.BuoiHocs", "LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.DangKyHocs", "AppUserId", "dbo.HocViens");
            DropForeignKey("dbo.GiangVien_BuoiHoc", "AppUserId", "dbo.GiangViens");
            DropForeignKey("dbo.DiemDanhs_GV", "AppUserId", "dbo.GiangViens");
            DropForeignKey("dbo.DiemDanhs_GV", "BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.GiangVien_BuoiHoc", "BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.BuoiHoc_Attachments", "BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.BuoiHoc_Attachments", "AttachmentId", "dbo.Attachments");
            DropIndex("dbo.HocViens", new[] { "Id" });
            DropIndex("dbo.GiangViens", new[] { "Id" });
            DropIndex("dbo.ThongBaoGiangViens", new[] { "GiangVien_Id" });
            DropIndex("dbo.ThongBaoGiangViens", new[] { "ThongBao_ThongBaoId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.DiemDanhs_HV", new[] { "AppUserId" });
            DropIndex("dbo.DiemDanhs_HV", new[] { "BuoiHocId" });
            DropIndex("dbo.LopHoc_Attachments", new[] { "AttachmentId" });
            DropIndex("dbo.LopHoc_Attachments", new[] { "LopHocId" });
            DropIndex("dbo.KhoaHoc_Attachments", new[] { "AttachmentId" });
            DropIndex("dbo.KhoaHoc_Attachments", new[] { "KhoaHocId" });
            DropIndex("dbo.LopHocs", new[] { "KhoaHocId" });
            DropIndex("dbo.DangKyHocs", new[] { "LopHocId" });
            DropIndex("dbo.DangKyHocs", new[] { "AppUserId" });
            DropIndex("dbo.DiemDanhs_GV", new[] { "AppUserId" });
            DropIndex("dbo.DiemDanhs_GV", new[] { "BuoiHocId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "HocVien_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "GiangVien_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.GiangVien_BuoiHoc", new[] { "AppUserId" });
            DropIndex("dbo.GiangVien_BuoiHoc", new[] { "BuoiHocId" });
            DropIndex("dbo.BuoiHocs", new[] { "LopHocId" });
            DropIndex("dbo.BuoiHoc_Attachments", new[] { "AttachmentId" });
            DropIndex("dbo.BuoiHoc_Attachments", new[] { "BuoiHocId" });
            DropTable("dbo.HocViens");
            DropTable("dbo.GiangViens");
            DropTable("dbo.ThongBaoGiangViens");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ThongBaos");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.DiemDanhs_HV");
            DropTable("dbo.LopHoc_Attachments");
            DropTable("dbo.KhoaHoc_Attachments");
            DropTable("dbo.KhoaHocs");
            DropTable("dbo.LopHocs");
            DropTable("dbo.DangKyHocs");
            DropTable("dbo.DiemDanhs_GV");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.GiangVien_BuoiHoc");
            DropTable("dbo.BuoiHocs");
            DropTable("dbo.BuoiHoc_Attachments");
            DropTable("dbo.Attachments");
        }
    }
}
