namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1603 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HocVienLopHocs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropForeignKey("dbo.HocVienLopHocs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.HocVienLopHocs", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.HocVienLopHocs", new[] { "LopHoc_LopHocId" });
            AddColumn("dbo.GiangViens", "HocHam", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.HocViens", "CoQuanLamViec", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.BuoiHocs", "GiangVien_NguoiDungId", c => c.Guid());
            AddColumn("dbo.LopHocs", "HocVien_NguoiDungId", c => c.Guid());
            AddColumn("dbo.KhoaHocs", "TaiLieuDinhKem", c => c.String(nullable: false));
            CreateIndex("dbo.BuoiHocs", "GiangVien_NguoiDungId");
            CreateIndex("dbo.LopHocs", "HocVien_NguoiDungId");
            AddForeignKey("dbo.LopHocs", "HocVien_NguoiDungId", "dbo.HocViens", "NguoiDungId");
            AddForeignKey("dbo.BuoiHocs", "GiangVien_NguoiDungId", "dbo.GiangViens", "NguoiDungId");
            DropColumn("dbo.BuoiHocs", "TaiLieuDinhKem");
            DropColumn("dbo.KhoaHocs", "HinhAnh");
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
            
            AddColumn("dbo.KhoaHocs", "HinhAnh", c => c.String());
            AddColumn("dbo.BuoiHocs", "TaiLieuDinhKem", c => c.String());
            DropForeignKey("dbo.BuoiHocs", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.LopHocs", "HocVien_NguoiDungId", "dbo.HocViens");
            DropIndex("dbo.LopHocs", new[] { "HocVien_NguoiDungId" });
            DropIndex("dbo.BuoiHocs", new[] { "GiangVien_NguoiDungId" });
            DropColumn("dbo.KhoaHocs", "TaiLieuDinhKem");
            DropColumn("dbo.LopHocs", "HocVien_NguoiDungId");
            DropColumn("dbo.BuoiHocs", "GiangVien_NguoiDungId");
            DropColumn("dbo.HocViens", "CoQuanLamViec");
            DropColumn("dbo.GiangViens", "HocHam");
            CreateIndex("dbo.HocVienLopHocs", "LopHoc_LopHocId");
            CreateIndex("dbo.HocVienLopHocs", "HocVien_NguoiDungId");
            AddForeignKey("dbo.HocVienLopHocs", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId", cascadeDelete: true);
            AddForeignKey("dbo.HocVienLopHocs", "HocVien_NguoiDungId", "dbo.HocViens", "NguoiDungId", cascadeDelete: true);
        }
    }
}
