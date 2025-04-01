using BtlWebNangCao.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BtlWebNangCao.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PhongChat> PhongChats { get; set; }
        public DbSet<ThanhVienPhong> ThanhVienPhongs { get; set; }
        public DbSet<TinNhan> TinNhans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính cho bảng ThanhVienPhong (bảng trung gian)
            modelBuilder.Entity<ThanhVienPhong>()
                .HasKey(tv => new { tv.MaNguoiDung, tv.MaPhong });

            // Quan hệ giữa ThanhVienPhong và NguoiDung
            modelBuilder.Entity<ThanhVienPhong>()
                .HasOne(tv => tv.NguoiDung)
                .WithMany(nd => nd.DanhSachPhongThamGia)
                .HasForeignKey(tv => tv.MaNguoiDung)
                .OnDelete(DeleteBehavior.NoAction);

            // Quan hệ giữa ThanhVienPhong và PhongChat
            modelBuilder.Entity<ThanhVienPhong>()
                .HasOne(tv => tv.PhongChat)
                .WithMany(pc => pc.DanhSachThanhVien)
                .HasForeignKey(tv => tv.MaPhong)
                .OnDelete(DeleteBehavior.NoAction);

            // Quan hệ giữa PhongChat và NguoiDung (người tạo)
            modelBuilder.Entity<PhongChat>()
                .HasOne(pc => pc.NguoiTao)
                .WithMany(nd => nd.DanhSachPhongTao)
                .HasForeignKey(pc => pc.MaNguoiTao)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ giữa TinNhan và PhongChat
            modelBuilder.Entity<TinNhan>()
                .HasOne(tn => tn.PhongChat)
                .WithMany(pc => pc.DanhSachTinNhan)
                .HasForeignKey(tn => tn.MaPhong)
                .OnDelete(DeleteBehavior.NoAction);

            // Quan hệ giữa TinNhan và NguoiDung (người gửi)
            modelBuilder.Entity<TinNhan>()
                .HasOne(tn => tn.NguoiGui)
                .WithMany(nd => nd.DanhSachTinNhan)
                .HasForeignKey(tn => tn.MaNguoiGui)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
