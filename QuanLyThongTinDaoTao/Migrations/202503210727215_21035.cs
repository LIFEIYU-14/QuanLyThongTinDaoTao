namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _21035 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LopHoc_Attachments", "Attachment_AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.LopHoc_Attachments", new[] { "Attachment_AttachmentId" });
            DropIndex("dbo.LopHoc_Attachments", new[] { "LopHoc_LopHocId" });
            RenameColumn(table: "dbo.LopHoc_Attachments", name: "Attachment_AttachmentId", newName: "AttachmentId");
            RenameColumn(table: "dbo.LopHoc_Attachments", name: "LopHoc_LopHocId", newName: "LopHocId");
            AlterColumn("dbo.LopHoc_Attachments", "AttachmentId", c => c.Guid(nullable: false));
            AlterColumn("dbo.LopHoc_Attachments", "LopHocId", c => c.Guid(nullable: false));
            CreateIndex("dbo.LopHoc_Attachments", "LopHocId");
            CreateIndex("dbo.LopHoc_Attachments", "AttachmentId");
            AddForeignKey("dbo.LopHoc_Attachments", "AttachmentId", "dbo.Attachments", "AttachmentId", cascadeDelete: true);
            AddForeignKey("dbo.LopHoc_Attachments", "LopHocId", "dbo.LopHocs", "LopHocId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LopHoc_Attachments", "LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.LopHoc_Attachments", "AttachmentId", "dbo.Attachments");
            DropIndex("dbo.LopHoc_Attachments", new[] { "AttachmentId" });
            DropIndex("dbo.LopHoc_Attachments", new[] { "LopHocId" });
            AlterColumn("dbo.LopHoc_Attachments", "LopHocId", c => c.Guid());
            AlterColumn("dbo.LopHoc_Attachments", "AttachmentId", c => c.Guid());
            RenameColumn(table: "dbo.LopHoc_Attachments", name: "LopHocId", newName: "LopHoc_LopHocId");
            RenameColumn(table: "dbo.LopHoc_Attachments", name: "AttachmentId", newName: "Attachment_AttachmentId");
            CreateIndex("dbo.LopHoc_Attachments", "LopHoc_LopHocId");
            CreateIndex("dbo.LopHoc_Attachments", "Attachment_AttachmentId");
            AddForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId");
            AddForeignKey("dbo.LopHoc_Attachments", "Attachment_AttachmentId", "dbo.Attachments", "AttachmentId");
        }
    }
}
