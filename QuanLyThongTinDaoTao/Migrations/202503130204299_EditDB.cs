namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditDB : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DangKyHoc", newName: "DangKyHocs");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DangKyHocs", newName: "DangKyHoc");
        }
    }
}
