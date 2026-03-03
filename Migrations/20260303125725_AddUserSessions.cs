using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogoutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SessionDurationMinutes = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Browser = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsSuspicious = table.Column<bool>(type: "bit", nullable: false),
                    SuspiciousReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SessionToken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.SessionId);
                });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6268));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6325));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6328));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6329));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6331));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6550));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6495));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6500));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6502));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6504));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6505));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6507));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 3, 14, 57, 24, 780, DateTimeKind.Local).AddTicks(6508));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(391));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(443));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(446));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(448));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(450));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(733));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(674));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(679));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(681));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(683));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(685));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(687));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(688));
        }
    }
}
