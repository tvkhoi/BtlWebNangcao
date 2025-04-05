using Microsoft.AspNetCore.Mvc.Rendering;

namespace BtlWebNangCao.ViewModels
{
    public class ThanhVienPhongCreateViewModel
    {
        public string MaNguoiDung { get; set; }
        public int MaPhong { get; set; }
        public string VaiTro { get; set; }

        public List<SelectListItem> DanhSachNguoiDung { get; set; }
        public List<SelectListItem> DanhSachPhong { get; set; }
    }
}
