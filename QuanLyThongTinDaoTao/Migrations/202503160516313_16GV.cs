namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _16GV : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.TaiLieuId)
                .ForeignKey("dbo.KhoaHocs", t => t.KhoaHoc_KhoaHocId)
                .Index(t => t.KhoaHoc_KhoaHocId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tailieus", "KhoaHoc_KhoaHocId", "dbo.KhoaHocs");
            DropIndex("dbo.Tailieus", new[] { "KhoaHoc_KhoaHocId" });
            DropTable("dbo.Tailieus");
        }
    }
}
