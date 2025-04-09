namespace BtlWebNangCao.ViewModels
{
    public class TinNhanViewModel
    {
        public int MaTinNhan { get; set; }
        public string NoiDung { get; set; }
        public string NguoiGui { get; set; } // Lấy từ bảng AspNetUsers
        public DateTime NgayGui { get; set; }
        public string PhongChat { get; set; } // Tên phòng chat
    }
}
