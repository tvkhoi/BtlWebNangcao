namespace BtlWebNangCao.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; } // Danh sách vai trò
        public bool IsLockedOut { get; set; } // Trạng thái bị khóa
    }
}
