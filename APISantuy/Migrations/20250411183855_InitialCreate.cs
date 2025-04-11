using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APISantuy.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_m_city",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_m_city", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_m_user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_m_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_t_log_user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LogMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_t_log_user", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_t_log_user_tbl_m_user_UserId",
                        column: x => x.UserId,
                        principalTable: "tbl_m_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_t_trip",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AssignedById = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_t_trip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_t_trip_tbl_m_city_CityId",
                        column: x => x.CityId,
                        principalTable: "tbl_m_city",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_t_trip_tbl_m_user_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "tbl_m_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_t_trip_tbl_m_user_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "tbl_m_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_t_log_user_UserId",
                table: "tbl_t_log_user",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_t_trip_AssignedById",
                table: "tbl_t_trip",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_t_trip_CityId",
                table: "tbl_t_trip",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_t_trip_EmployeeId",
                table: "tbl_t_trip",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_t_log_user");

            migrationBuilder.DropTable(
                name: "tbl_t_trip");

            migrationBuilder.DropTable(
                name: "tbl_m_city");

            migrationBuilder.DropTable(
                name: "tbl_m_user");
        }
    }
}
