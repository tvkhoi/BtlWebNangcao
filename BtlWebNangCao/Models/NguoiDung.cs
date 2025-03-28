using System.ComponentModel.DataAnnotations;
namespace BtlWebNangCao.Models
{
    public class NguoiDung
    {
        [Key,StringLength(450)]
        public string MaNguoiDung { get; set; }

        [Required, StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required, StringLength(255)]
        public string MatKhau { get; set; }  // Mật khẩu đã mã hóa (hash)

        [Required, StringLength(100)]
        public string Email { get; set; }

        [Required, StringLength(20)]
        public string VaiTro { get; set; } = "User"; // Mặc định là User

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Quan hệ 1-N: Một người dùng có thể tạo nhiều phòng chat
        public ICollection<PhongChat> DanhSachPhongTao { get; set; }

        // Quan hệ N-N: Người dùng có thể tham gia nhiều phòng chat
        public ICollection<ThanhVienPhong> DanhSachPhongThamGia { get; set; }

        // Quan hệ 1-N: Một người dùng có thể gửi nhiều tin nhắn
        public ICollection<TinNhan> DanhSachTinNhan { get; set; }
    }
}
