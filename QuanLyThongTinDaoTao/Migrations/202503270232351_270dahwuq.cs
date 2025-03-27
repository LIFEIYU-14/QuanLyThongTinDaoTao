namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _270dahwuq : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BuoiHoc_Attachments", "Attachment_AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.BuoiHoc_Attachments", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs");
            DropIndex("dbo.BuoiHoc_Attachments", new[] { "Attachment_AttachmentId" });
            DropIndex("dbo.BuoiHoc_Attachments", new[] { "BuoiHoc_BuoiHocId" });
            RenameColumn(table: "dbo.BuoiHoc_Attachments", name: "Attachment_AttachmentId", newName: "AttachmentId");
            RenameColumn(table: "dbo.BuoiHoc_Attachments", name: "BuoiHoc_BuoiHocId", newName: "BuoiHocId");
            AlterColumn("dbo.BuoiHoc_Attachments", "AttachmentId", c => c.Guid(nullable: false));
            AlterColumn("dbo.BuoiHoc_Attachments", "BuoiHocId", c => c.Guid(nullable: false));
            CreateIndex("dbo.BuoiHoc_Attachments", "BuoiHocId");
            CreateIndex("dbo.BuoiHoc_Attachments", "AttachmentId");
            AddForeignKey("dbo.BuoiHoc_Attachments", "AttachmentId", "dbo.Attachments", "AttachmentId", cascadeDelete: true);
            AddForeignKey("dbo.BuoiHoc_Attachments", "BuoiHocId", "dbo.BuoiHocs", "BuoiHocId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BuoiHoc_Attachments", "BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.BuoiHoc_Attachments", "AttachmentId", "dbo.Attachments");
            DropIndex("dbo.BuoiHoc_Attachments", new[] { "AttachmentId" });
            DropIndex("dbo.BuoiHoc_Attachments", new[] { "BuoiHocId" });
            AlterColumn("dbo.BuoiHoc_Attachments", "BuoiHocId", c => c.Guid());
            AlterColumn("dbo.BuoiHoc_Attachments", "AttachmentId", c => c.Guid());
            RenameColumn(table: "dbo.BuoiHoc_Attachments", name: "BuoiHocId", newName: "BuoiHoc_BuoiHocId");
            RenameColumn(table: "dbo.BuoiHoc_Attachments", name: "AttachmentId", newName: "Attachment_AttachmentId");
            CreateIndex("dbo.BuoiHoc_Attachments", "BuoiHoc_BuoiHocId");
            CreateIndex("dbo.BuoiHoc_Attachments", "Attachment_AttachmentId");
            AddForeignKey("dbo.BuoiHoc_Attachments", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs", "BuoiHocId");
            AddForeignKey("dbo.BuoiHoc_Attachments", "Attachment_AttachmentId", "dbo.Attachments", "AttachmentId");
        }
    }
}
