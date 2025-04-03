namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _030420251 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BuoiHocs", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.LopHocGiangViens", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.LopHocGiangViens", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.BuoiHocs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.HocVienLopHocs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.HocVienLopHocs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.BuoiHocs", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.BuoiHocs", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.LopHocGiangViens", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.LopHocGiangViens", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.HocVienLopHocs", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.HocVienLopHocs", new[] { "LopHoc_LopHocId" });
            AddColumn("dbo.GiangViens", "LopHoc_LopHocId", c => c.Guid());
            AddColumn("dbo.HocViens", "LopHoc_LopHocId", c => c.Guid());
            CreateIndex("dbo.GiangViens", "LopHoc_LopHocId");
            CreateIndex("dbo.HocViens", "LopHoc_LopHocId");
            AddForeignKey("dbo.GiangViens", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId");
            AddForeignKey("dbo.HocViens", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId");
            DropColumn("dbo.BuoiHocs", "GiangVien_NguoiDungId");
            DropColumn("dbo.BuoiHocs", "HocVien_NguoiDungId");
            DropTable("dbo.LopHocGiangViens");
            DropTable("dbo.HocVienLopHocs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.HocVienLopHocs",
                c => new
                    {
                        HocVien_NguoiDungId = c.Guid(nullable: false),
                        LopHoc_LopHocId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.HocVien_NguoiDungId, t.LopHoc_LopHocId });
            
            CreateTable(
                "dbo.LopHocGiangViens",
                c => new
                    {
                        LopHoc_LopHocId = c.Guid(nullable: false),
                        GiangVien_NguoiDungId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.LopHoc_LopHocId, t.GiangVien_NguoiDungId });
            
            AddColumn("dbo.BuoiHocs", "HocVien_NguoiDungId", c => c.Guid());
            AddColumn("dbo.BuoiHocs", "GiangVien_NguoiDungId", c => c.Guid());
            DropForeignKey("dbo.HocViens", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.GiangViens", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.HocViens", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.GiangViens", new[] { "LopHoc_LopHocId" });
            DropColumn("dbo.HocViens", "LopHoc_LopHocId");
            DropColumn("dbo.GiangViens", "LopHoc_LopHocId");
            CreateIndex("dbo.HocVienLopHocs", "LopHoc_LopHocId");
            CreateIndex("dbo.HocVienLopHocs", "HocVien_NguoiDungId");
            CreateIndex("dbo.LopHocGiangViens", "GiangVien_NguoiDungId");
            CreateIndex("dbo.LopHocGiangViens", "LopHoc_LopHocId");
            CreateIndex("dbo.BuoiHocs", "HocVien_NguoiDungId");
            CreateIndex("dbo.BuoiHocs", "GiangVien_NguoiDungId");
            AddForeignKey("dbo.HocVienLopHocs", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId", cascadeDelete: true);
            AddForeignKey("dbo.HocVienLopHocs", "HocVien_NguoiDungId", "dbo.HocViens", "NguoiDungId", cascadeDelete: true);
            AddForeignKey("dbo.BuoiHocs", "HocVien_NguoiDungId", "dbo.HocViens", "NguoiDungId");
            AddForeignKey("dbo.LopHocGiangViens", "GiangVien_NguoiDungId", "dbo.GiangViens", "NguoiDungId", cascadeDelete: true);
            AddForeignKey("dbo.LopHocGiangViens", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId", cascadeDelete: true);
            AddForeignKey("dbo.BuoiHocs", "GiangVien_NguoiDungId", "dbo.GiangViens", "NguoiDungId");
        }
    }
}
