namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConfirmationToDangKyHoc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DangKyHocs", "IsConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.DangKyHocs", "ConfirmationToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DangKyHocs", "ConfirmationToken");
            DropColumn("dbo.DangKyHocs", "IsConfirmed");
        }
    }
}
