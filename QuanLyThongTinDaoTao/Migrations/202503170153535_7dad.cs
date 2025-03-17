namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7dad : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.LopHocGiangViens", newName: "GiangVienLopHocs");
            DropForeignKey("dbo.HinhAnhs", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.HinhAnhs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.HinhAnhs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.HinhAnhs", new[] { "BuoiHoc_BuoiHocId" });
            DropIndex("dbo.HinhAnhs", new[] { "KhoaHoc_KhoaHocId" });
            DropIndex("dbo.HinhAnhs", new[] { "LopHoc_LopHocId" });
            DropPrimaryKey("dbo.GiangVienLopHocs");
            CreateTable(
                "dbo.BuoiHoc_Attachments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Attachment_AttachmentId = c.Guid(),
                        BuoiHoc_BuoiHocId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.Attachment_AttachmentId)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHoc_BuoiHocId)
                .Index(t => t.Attachment_AttachmentId)
                .Index(t => t.BuoiHoc_BuoiHocId);
            
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
                "dbo.KhoaHoc_Attachments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Attachment_AttachmentId = c.Guid(),
                        KhoaHoc_KhoaHocId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.Attachment_AttachmentId)
                .ForeignKey("dbo.KhoaHocs", t => t.KhoaHoc_KhoaHocId)
                .Index(t => t.Attachment_AttachmentId)
                .Index(t => t.KhoaHoc_KhoaHocId);
            
            CreateTable(
                "dbo.LopHoc_Attachments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Attachment_AttachmentId = c.Guid(),
                        LopHoc_LopHocId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.Attachment_AttachmentId)
                .ForeignKey("dbo.LopHocs", t => t.LopHoc_LopHocId)
                .Index(t => t.Attachment_AttachmentId)
                .Index(t => t.LopHoc_LopHocId);
            
            AddColumn("dbo.LopHocs", "MoTa", c => c.String());
            AlterColumn("dbo.BuoiHocs", "TrangThai", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.GiangVienLopHocs", new[] { "GiangVien_NguoiDungId", "LopHoc_LopHocId" });
            CreateIndex("dbo.NguoiDungs", "Email", unique: true);
            DropColumn("dbo.KhoaHocs", "TaiLieuDinhKem");
            DropTable("dbo.HinhAnhs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.HinhAnhs",
                c => new
                    {
                        HinhAnhId = c.Guid(nullable: false),
                        DuongDan = c.String(nullable: false, maxLength: 500),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                        BuoiHoc_BuoiHocId = c.Guid(),
                        KhoaHoc_KhoaHocId = c.Guid(),
                        LopHoc_LopHocId = c.Guid(),
                    })
                .PrimaryKey(t => t.HinhAnhId);
            
            AddColumn("dbo.KhoaHocs", "TaiLieuDinhKem", c => c.String(nullable: false));
            DropForeignKey("dbo.BuoiHoc_Attachments", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.LopHoc_Attachments", "Attachment_AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.KhoaHoc_Attachments", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.KhoaHoc_Attachments", "Attachment_AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.BuoiHoc_Attachments", "Attachment_AttachmentId", "dbo.Attachments");
            DropIndex("dbo.LopHoc_Attachments", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.LopHoc_Attachments", new[] { "Attachment_AttachmentId" });
            DropIndex("dbo.KhoaHoc_Attachments", new[] { "KhoaHoc_KhoaHocId" });
            DropIndex("dbo.KhoaHoc_Attachments", new[] { "Attachment_AttachmentId" });
            DropIndex("dbo.BuoiHoc_Attachments", new[] { "BuoiHoc_BuoiHocId" });
            DropIndex("dbo.BuoiHoc_Attachments", new[] { "Attachment_AttachmentId" });
            DropIndex("dbo.NguoiDungs", new[] { "Email" });
            DropPrimaryKey("dbo.GiangVienLopHocs");
            AlterColumn("dbo.BuoiHocs", "TrangThai", c => c.String(nullable: false, maxLength: 20));
            DropColumn("dbo.LopHocs", "MoTa");
            DropTable("dbo.LopHoc_Attachments");
            DropTable("dbo.KhoaHoc_Attachments");
            DropTable("dbo.Attachments");
            DropTable("dbo.BuoiHoc_Attachments");
            AddPrimaryKey("dbo.GiangVienLopHocs", new[] { "LopHoc_LopHocId", "GiangVien_NguoiDungId" });
            CreateIndex("dbo.HinhAnhs", "LopHoc_LopHocId");
            CreateIndex("dbo.HinhAnhs", "KhoaHoc_KhoaHocId");
            CreateIndex("dbo.HinhAnhs", "BuoiHoc_BuoiHocId");
            AddForeignKey("dbo.HinhAnhs", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId");
            AddForeignKey("dbo.HinhAnhs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs", "KhoaHocId");
            AddForeignKey("dbo.HinhAnhs", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs", "BuoiHocId");
            RenameTable(name: "dbo.GiangVienLopHocs", newName: "LopHocGiangViens");
        }
    }
}
