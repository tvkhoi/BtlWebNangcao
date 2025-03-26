using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BtlWebNangCao.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.MaNguoiDung);
                });

            migrationBuilder.CreateTable(
                name: "PhongChats",
                columns: table => new
                {
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LaCongKhai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaNguoiTao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongChats", x => x.MaPhong);
                    table.ForeignKey(
                        name: "FK_PhongChats_NguoiDungs_MaNguoiTao",
                        column: x => x.MaNguoiTao,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhVienPhongs",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    NgayThamGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VaiTroPhong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhVienPhongs", x => new { x.MaNguoiDung, x.MaPhong });
                    table.ForeignKey(
                        name: "FK_ThanhVienPhongs_NguoiDungs_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_ThanhVienPhongs_PhongChats_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PhongChats",
                        principalColumn: "MaPhong");
                });

            migrationBuilder.CreateTable(
                name: "TinNhans",
                columns: table => new
                {
                    MaTinNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    MaNguoiGui = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinNhans", x => x.MaTinNhan);
                    table.ForeignKey(
                        name: "FK_TinNhans_NguoiDungs_MaNguoiGui",
                        column: x => x.MaNguoiGui,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TinNhans_PhongChats_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PhongChats",
                        principalColumn: "MaPhong");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_Email",
                table: "NguoiDungs",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_TenDangNhap",
                table: "NguoiDungs",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhongChats_MaNguoiTao",
                table: "PhongChats",
                column: "MaNguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVienPhongs_MaPhong",
                table: "ThanhVienPhongs",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_MaNguoiGui",
                table: "TinNhans",
                column: "MaNguoiGui");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_MaPhong",
                table: "TinNhans",
                column: "MaPhong");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThanhVienPhongs");

            migrationBuilder.DropTable(
                name: "TinNhans");

            migrationBuilder.DropTable(
                name: "PhongChats");

            migrationBuilder.DropTable(
                name: "NguoiDungs");
        }
    }
}
