using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BtlWebNangCao.Models
{
    public class PhongChat
    {
        [Key]
        public int MaPhong { get; set; }

        [Required, StringLength(100)]
        public string TieuDe { get; set; }

        [StringLength(255)]
        public string MoTa { get; set; }

        public bool LaCongKhai { get; set; } = true; // Mặc định là phòng công khai

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Khóa ngoại: Người tạo phòng
        public string MaNguoiTao { get; set; }
        [ForeignKey("MaNguoiTao")]
        public ApplicationUser NguoiTao { get; set; }

        // Quan hệ N-N: Phòng có nhiều thành viên
        public ICollection<ThanhVienPhong> DanhSachThanhVien { get; set; }

        // Quan hệ 1-N: Phòng có nhiều tin nhắn
        public ICollection<TinNhan> DanhSachTinNhan { get; set; }
    }
}
