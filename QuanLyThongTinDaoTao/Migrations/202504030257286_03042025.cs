namespace QuanLyThongTinDaoTao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _03042025 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GiangViens", "MaGiangVien", c => c.String());
            AlterColumn("dbo.HocViens", "MaHocVien", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HocViens", "MaHocVien", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.GiangViens", "MaGiangVien", c => c.String(nullable: false, maxLength: 20));
        }
    }
}
