namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _5da5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BuoiHocs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.LopHocs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropIndex("dbo.BuoiHocs", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.LopHocs", new[] { "KhoaHoc_KhoaHocId" });
            RenameColumn(table: "dbo.BuoiHocs", name: "LopHoc_LopHocId", newName: "LopHocId");
            RenameColumn(table: "dbo.LopHocs", name: "KhoaHoc_KhoaHocId", newName: "KhoaHocId");
            AlterColumn("dbo.BuoiHocs", "LopHocId", c => c.Guid(nullable: false));
            AlterColumn("dbo.LopHocs", "KhoaHocId", c => c.Guid(nullable: false));
            CreateIndex("dbo.BuoiHocs", "LopHocId");
            CreateIndex("dbo.LopHocs", "KhoaHocId");
            AddForeignKey("dbo.BuoiHocs", "LopHocId", "dbo.LopHocs", "LopHocId", cascadeDelete: true);
            AddForeignKey("dbo.LopHocs", "KhoaHocId", "dbo.KhoaHocs", "KhoaHocId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LopHocs", "KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.BuoiHocs", "LopHocId", "dbo.LopHocs");
            DropIndex("dbo.LopHocs", new[] { "KhoaHocId" });
            DropIndex("dbo.BuoiHocs", new[] { "LopHocId" });
            AlterColumn("dbo.LopHocs", "KhoaHocId", c => c.Guid());
            AlterColumn("dbo.BuoiHocs", "LopHocId", c => c.Guid());
            RenameColumn(table: "dbo.LopHocs", name: "KhoaHocId", newName: "KhoaHoc_KhoaHocId");
            RenameColumn(table: "dbo.BuoiHocs", name: "LopHocId", newName: "LopHoc_LopHocId");
            CreateIndex("dbo.LopHocs", "KhoaHoc_KhoaHocId");
            CreateIndex("dbo.BuoiHocs", "LopHoc_LopHocId");
            AddForeignKey("dbo.LopHocs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs", "KhoaHocId");
            AddForeignKey("dbo.BuoiHocs", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId");
        }
    }
}
