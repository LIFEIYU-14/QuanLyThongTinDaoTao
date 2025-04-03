namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _030 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GiangViens", "LopHoc_LopHocId", "dbo.LopHocs");
            DropForeignKey("dbo.HocViens", "LopHoc_LopHocId", "dbo.LopHocs");
            DropIndex("dbo.GiangViens", new[] { "LopHoc_LopHocId" });
            DropIndex("dbo.HocViens", new[] { "LopHoc_LopHocId" });
            DropColumn("dbo.GiangViens", "LopHoc_LopHocId");
            DropColumn("dbo.HocViens", "LopHoc_LopHocId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HocViens", "LopHoc_LopHocId", c => c.Guid());
            AddColumn("dbo.GiangViens", "LopHoc_LopHocId", c => c.Guid());
            CreateIndex("dbo.HocViens", "LopHoc_LopHocId");
            CreateIndex("dbo.GiangViens", "LopHoc_LopHocId");
            AddForeignKey("dbo.HocViens", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId");
            AddForeignKey("dbo.GiangViens", "LopHoc_LopHocId", "dbo.LopHocs", "LopHocId");
        }
    }
}
