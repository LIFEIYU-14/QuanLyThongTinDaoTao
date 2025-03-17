namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7daddadswbbv : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LopHocs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropIndex("dbo.LopHocs", new[] { "HocVien_NguoiDungId" });
            CreateTable(
                "dbo.HocVienLopHocs",
                c => new
                    {
                        HocVien_NguoiDungId = c.Guid(nullable: false),
                        LopHoc_LopHocId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.HocVien_NguoiDungId, t.LopHoc_LopHocId })
                .ForeignKey("dbo.HocViens", t => t.HocVien_NguoiDungId, cascadeDelete: true)
                .ForeignKey("dbo.LopHocs", t => t.LopHoc_LopHocId, cascadeDelete: true)
                .Index(t => t.HocVien_NguoiDungId)
                .Index(t => t.LopHoc_LopHocId);
            
            DropColumn("dbo.LopHocs", "HocVien_NguoiDungId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LopHocs", "HocVien_NguoiDungId", c => c.Guid());
            DropForeignKey("dbo.HocVienLopHocs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.HocVienLopHocs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropIndex("dbo.HocVienLopHocs", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.HocVienLopHocs", new[] { "HocVien_NguoiDungId" });
            DropTable("dbo.HocVienLopHocs");
            CreateIndex("dbo.LopHocs", "HocVien_NguoiDungId");
            AddForeignKey("dbo.LopHocs", "HocVien_NguoiDungId", "dbo.HocViens", "NguoiDungId");
        }
    }
}
