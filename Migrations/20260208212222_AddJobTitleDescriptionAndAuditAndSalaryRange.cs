using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddJobTitleDescriptionAndAuditAndSalaryRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "JobTitles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "JobTitles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxSalary",
                table: "JobTitles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinSalary",
                table: "JobTitles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "JobTitles",
                type: "datetime2",
                nullable: true);

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
                columns: new[] { "CreatedDate", "Description", "MaxSalary", "MinSalary", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4760), null, null, null, null });

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Description", "MaxSalary", "MinSalary", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4769), null, null, null, null });

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Description", "MaxSalary", "MinSalary", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4770), null, null, null, null });

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 4,
                columns: new[] { "CreatedDate", "Description", "MaxSalary", "MinSalary", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4772), null, null, null, null });

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 5,
                columns: new[] { "CreatedDate", "Description", "MaxSalary", "MinSalary", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4774), null, null, null, null });

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 6,
                columns: new[] { "CreatedDate", "Description", "MaxSalary", "MinSalary", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4809), null, null, null, null });

            migrationBuilder.UpdateData(
                table: "JobTitles",
                keyColumn: "JobTitleId",
                keyValue: 7,
                columns: new[] { "CreatedDate", "Description", "MaxSalary", "MinSalary", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 2, 8, 23, 22, 21, 522, DateTimeKind.Local).AddTicks(4811), null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "JobTitles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "JobTitles");

            migrationBuilder.DropColumn(
                name: "MaxSalary",
                table: "JobTitles");

            migrationBuilder.DropColumn(
                name: "MinSalary",
                table: "JobTitles");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "JobTitles");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2142));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2201));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2203));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2204));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2206));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 8, 22, 44, 48, 226, DateTimeKind.Local).AddTicks(2403));
        }
    }
}
