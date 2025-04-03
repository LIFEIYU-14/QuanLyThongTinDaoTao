namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vbnx : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.GiangViens", new[] { "Email" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.GiangViens", "Email", unique: true);
        }
    }
}
