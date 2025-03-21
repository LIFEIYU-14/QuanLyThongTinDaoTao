namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.LopHoc_Attachments", new[] { "LopHoc_LopHocId" });
            AlterColumn("dbo.LopHoc_Attachments", "LopHoc_LopHocId", c => c.Guid());
            CreateIndex("dbo.LopHoc_Attachments", "LopHoc_LopHocId");
            AddForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.LopHoc_Attachments", new[] { "LopHoc_LopHocId" });
            AlterColumn("dbo.LopHoc_Attachments", "LopHoc_LopHocId", c => c.Guid(nullable: false));
            CreateIndex("dbo.LopHoc_Attachments", "LopHoc_LopHocId");
            AddForeignKey("dbo.LopHoc_Attachments", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId", cascadeDelete: true);
        }
    }
}
