using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartamentDescriptionAndAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Departments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Departments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Departments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Description", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2142), null, null });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Description", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2201), null, null });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Description", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2203), null, null });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                columns: new[] { "CreatedDate", "Description", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2204), null, null });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 5,
                columns: new[] { "CreatedDate", "Description", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2206), null, null });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2403));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Departments");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 6, 23, 38, 47, 611, DateTimeKind.Local).AddTicks(7179));
        }
    }
}
