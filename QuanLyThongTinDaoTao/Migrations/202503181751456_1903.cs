namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1903 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tailieus", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropIndex("dbo.Tailieus", new[] { "KhoaHoc_KhoaHocId" });
            AddColumn("dbo.GiangViens", "QR_Code_GV", c => c.String(nullable: false));
            AddColumn("dbo.HocViens", "QR_Code_HV", c => c.String(nullable: false));
            DropTable("dbo.Tailieus");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Tailieus",
                c => new
                    {
                        TaiLieuId = c.Guid(nullable: false),
                        TenTaiLieu = c.String(nullable: false, maxLength: 255),
                        NgayTao = c.DateTime(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                        KhoaHoc_KhoaHocId = c.Guid(),
                    })
                .PrimaryKey(t => t.TaiLieuId);
            
            DropColumn("dbo.HocViens", "QR_Code_HV");
            DropColumn("dbo.GiangViens", "QR_Code_GV");
            CreateIndex("dbo.Tailieus", "KhoaHoc_KhoaHocId");
            AddForeignKey("dbo.Tailieus", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs", "KhoaHocId");
        }
    }
}
