namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d2qjejd : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.HinhAnhs", "DoiTuongId");
            DropColumn("dbo.HinhAnhs", "LoaiDoiTuong");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HinhAnhs", "LoaiDoiTuong", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.HinhAnhs", "DoiTuongId", c => c.Guid(nullable: false));
        }
    }
}
