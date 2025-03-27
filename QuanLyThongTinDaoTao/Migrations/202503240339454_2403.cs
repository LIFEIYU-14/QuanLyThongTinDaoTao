namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2403 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KhoaHocs", "HinhDaiDienKhoaHoc", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KhoaHocs", "HinhDaiDienKhoaHoc");
        }
    }
}
