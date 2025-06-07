namespace BtlWebNangCao.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalRooms { get; set; }
        public int TotalMessages { get; set; }
        public int ActiveUsersToday { get; set; }

        // Các dữ liệu cho biểu đồ
        public List<string> TotalMessagesPerDayLabels { get; set; }
        public List<int> TotalMessagesPerDay { get; set; }
        public List<int> RoomTypes { get; set; }
    }
}
