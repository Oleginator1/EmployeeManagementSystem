using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Employees",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

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
                columns: new[] { "CreatedDate", "UserId" },
                values: new object[] { new DateTime(2026, 2, 11, 14, 58, 18, 954, DateTimeKind.Local).AddTicks(733), null });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4556));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4603));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4605));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4607));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4609));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4852));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4760));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4769));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4770));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4772));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4774));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4809));

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4811));
        }
    }
}
