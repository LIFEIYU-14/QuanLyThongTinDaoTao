namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7daddadswbbvfs : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GiangVienLopHocs", newName: "LopHocGiangViens");
            RenameTable(name: "dbo.HocVienThongBaos", newName: "ThongBaoHocViens");
            DropPrimaryKey("dbo.LopHocGiangViens");
            DropPrimaryKey("dbo.ThongBaoHocViens");
            AddPrimaryKey("dbo.LopHocGiangViens", new[] { "LopHoc_LopHocId", "GiangVien_NguoiDungId" });
            AddPrimaryKey("dbo.ThongBaoHocViens", new[] { "ThongBao_ThongBaoId", "HocVien_NguoiDungId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ThongBaoHocViens");
            DropPrimaryKey("dbo.LopHocGiangViens");
            AddPrimaryKey("dbo.ThongBaoHocViens", new[] { "HocVien_NguoiDungId", "ThongBao_ThongBaoId" });
            AddPrimaryKey("dbo.LopHocGiangViens", new[] { "GiangVien_NguoiDungId", "LopHoc_LopHocId" });
            RenameTable(name: "dbo.ThongBaoHocViens", newName: "HocVienThongBaos");
            RenameTable(name: "dbo.LopHocGiangViens", newName: "GiangVienLopHocs");
        }
    }
}
