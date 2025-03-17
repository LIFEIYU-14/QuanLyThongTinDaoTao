namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d2qjej : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LopHocs", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropIndex("dbo.LopHocs", new[] { "GiangVien_NguoiDungId" });
            CreateTable(
                "dbo.HinhAnhs",
                c => new
                    {
                        HinhAnhId = c.Guid(nullable: false),
                        DuongDan = c.String(nullable: false, maxLength: 500),
                        DoiTuongId = c.Guid(nullable: false),
                        LoaiDoiTuong = c.String(nullable: false, maxLength: 50),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                        BuoiHoc_BuoiHocId = c.Guid(),
                        KhoaHoc_KhoaHocId = c.Guid(),
                        LopHoc_LopHocId = c.Guid(),
                    })
                .PrimaryKey(t => t.HinhAnhId)
                .ForeignKey("dbo.BuoiHocs", t => t.BuoiHoc_BuoiHocId)
                .ForeignKey("dbo.KhoaHocs", t => t.KhoaHoc_KhoaHocId)
                .ForeignKey("dbo.LopHocs", t => t.LopHoc_LopHocId)
                .Index(t => t.BuoiHoc_BuoiHocId)
                .Index(t => t.KhoaHoc_KhoaHocId)
                .Index(t => t.LopHoc_LopHocId);
            
            CreateTable(
                "dbo.LopHocGiangViens",
                c => new
                    {
                        LopHoc_LopHocId = c.Guid(nullable: false),
                        GiangVien_NguoiDungId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.LopHoc_LopHocId, t.GiangVien_NguoiDungId })
                .ForeignKey("dbo.LopHocs", t => t.LopHoc_LopHocId, cascadeDelete: true)
                .ForeignKey("dbo.GiangViens", t => t.GiangVien_NguoiDungId, cascadeDelete: true)
                .Index(t => t.LopHoc_LopHocId)
                .Index(t => t.GiangVien_NguoiDungId);
            
            DropColumn("dbo.LopHocs", "GiangVien_NguoiDungId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LopHocs", "GiangVien_NguoiDungId", c => c.Guid());
            DropForeignKey("dbo.HinhAnhs", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.HinhAnhs", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropForeignKey("dbo.HinhAnhs", "BuoiHoc_BuoiHocId", "dbo.BuoiHocs");
            DropForeignKey("dbo.LopHocGiangViens", "GiangVien_NguoiDungId", "dbo.GiangViens");
            DropForeignKey("dbo.LopHocGiangViens", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.LopHocGiangViens", new[] { "GiangVien_NguoiDungId" });
            DropIndex("dbo.LopHocGiangViens", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.HinhAnhs", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.HinhAnhs", new[] { "KhoaHoc_KhoaHocId" });
            DropIndex("dbo.HinhAnhs", new[] { "BuoiHoc_BuoiHocId" });
            DropTable("dbo.LopHocGiangViens");
            DropTable("dbo.HinhAnhs");
            CreateIndex("dbo.LopHocs", "GiangVien_NguoiDungId");
            AddForeignKey("dbo.LopHocs", "GiangVien_NguoiDungId", "dbo.GiangViens", "NguoiDungId");
        }
    }
}
