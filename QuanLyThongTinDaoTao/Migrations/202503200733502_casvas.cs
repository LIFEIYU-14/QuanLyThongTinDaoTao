namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class casvas : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KhoaHoc_Attachments", "Attachment_AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.KhoaHoc_Attachments", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropIndex("dbo.KhoaHoc_Attachments", new[] { "Attachment_AttachmentId" });
            DropIndex("dbo.KhoaHoc_Attachments", new[] { "KhoaHoc_KhoaHocId" });
            DropIndex("dbo.LopHoc_Attachments", new[] { "LopHoc_LopHocId" });
            RenameColumn(table: "dbo.KhoaHoc_Attachments", name: "Attachment_AttachmentId", newName: "AttachmentId");
            RenameColumn(table: "dbo.KhoaHoc_Attachments", name: "KhoaHoc_KhoaHocId", newName: "KhoaHocId");
            AlterColumn("dbo.KhoaHoc_Attachments", "AttachmentId", c => c.Guid(nullable: false));
            AlterColumn("dbo.KhoaHoc_Attachments", "KhoaHocId", c => c.Guid(nullable: false));
            AlterColumn("dbo.LopHoc_Attachments", "LopHoc_LopHocId", c => c.Guid(nullable: false));
            CreateIndex("dbo.KhoaHoc_Attachments", "KhoaHocId");
            CreateIndex("dbo.KhoaHoc_Attachments", "AttachmentId");
            CreateIndex("dbo.LopHoc_Attachments", "LopHoc_LopHocId");
            AddForeignKey("dbo.KhoaHoc_Attachments", "AttachmentId", "dbo.Attachments", "AttachmentId", cascadeDelete: true);
            AddForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId", cascadeDelete: true);
            AddForeignKey("dbo.KhoaHoc_Attachments", "KhoaHocId", "dbo.KhoaHocs", "KhoaHocId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KhoaHoc_Attachments", "KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.KhoaHoc_Attachments", "AttachmentId", "dbo.Attachments");
            DropIndex("dbo.LopHoc_Attachments", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.KhoaHoc_Attachments", new[] { "AttachmentId" });
            DropIndex("dbo.KhoaHoc_Attachments", new[] { "KhoaHocId" });
            AlterColumn("dbo.LopHoc_Attachments", "LopHoc_LopHocId", c => c.Guid());
            AlterColumn("dbo.KhoaHoc_Attachments", "KhoaHocId", c => c.Guid());
            AlterColumn("dbo.KhoaHoc_Attachments", "AttachmentId", c => c.Guid());
            RenameColumn(table: "dbo.KhoaHoc_Attachments", name: "KhoaHocId", newName: "KhoaHoc_KhoaHocId");
            RenameColumn(table: "dbo.KhoaHoc_Attachments", name: "AttachmentId", newName: "Attachment_AttachmentId");
            CreateIndex("dbo.LopHoc_Attachments", "LopHoc_LopHocId");
            CreateIndex("dbo.KhoaHoc_Attachments", "KhoaHoc_KhoaHocId");
            CreateIndex("dbo.KhoaHoc_Attachments", "Attachment_AttachmentId");
            AddForeignKey("dbo.KhoaHoc_Attachments", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs", "KhoaHocId");
            AddForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId");
            AddForeignKey("dbo.KhoaHoc_Attachments", "Attachment_AttachmentId", "dbo.Attachments", "AttachmentId");
        }
    }
}
