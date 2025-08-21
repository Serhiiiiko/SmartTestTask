using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartTestTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessEquipmentTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Area = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessEquipmentTypes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ProductionFacilities",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StandardArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionFacilities", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentPlacementContracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionFacilityCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProcessEquipmentTypeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EquipmentQuantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentPlacementContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentPlacementContracts_ProcessEquipmentTypes_ProcessEquipmentTypeCode",
                        column: x => x.ProcessEquipmentTypeCode,
                        principalTable: "ProcessEquipmentTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentPlacementContracts_ProductionFacilities_ProductionFacilityCode",
                        column: x => x.ProductionFacilityCode,
                        principalTable: "ProductionFacilities",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ProcessEquipmentTypes",
                columns: new[] { "Code", "Area", "Name" },
                values: new object[,]
                {
                    { "EQT-001", 25m, "Industrial Lathe" },
                    { "EQT-002", 30m, "CNC Milling Machine" },
                    { "EQT-003", 10m, "3D Printer" },
                    { "EQT-004", 40m, "Assembly Robot" },
                    { "EQT-005", 15m, "Quality Control Station" },
                    { "EQT-006", 35m, "Packaging Machine" },
                    { "EQT-007", 20m, "Welding Station" },
                    { "EQT-008", 50m, "Paint Booth" },
                    { "EQT-009", 18m, "Testing Equipment" },
                    { "EQT-010", 60m, "Storage Rack System" }
                });

            migrationBuilder.InsertData(
                table: "ProductionFacilities",
                columns: new[] { "Code", "Name", "StandardArea" },
                values: new object[,]
                {
                    { "FAC-001", "Main Production Facility", 10000m },
                    { "FAC-002", "Secondary Production Facility", 7500m },
                    { "FAC-003", "North Wing Facility", 5000m },
                    { "FAC-004", "South Wing Facility", 5000m },
                    { "FAC-005", "Research & Development Facility", 3000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentPlacementContracts_ContractNumber",
                table: "EquipmentPlacementContracts",
                column: "ContractNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentPlacementContracts_CreatedAt",
                table: "EquipmentPlacementContracts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentPlacementContracts_IsActive",
                table: "EquipmentPlacementContracts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentPlacementContracts_ProcessEquipmentTypeCode",
                table: "EquipmentPlacementContracts",
                column: "ProcessEquipmentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentPlacementContracts_ProductionFacilityCode",
                table: "EquipmentPlacementContracts",
                column: "ProductionFacilityCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessEquipmentTypes_Name",
                table: "ProcessEquipmentTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionFacilities_Name",
                table: "ProductionFacilities",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentPlacementContracts");

            migrationBuilder.DropTable(
                name: "ProcessEquipmentTypes");

            migrationBuilder.DropTable(
                name: "ProductionFacilities");
        }
    }
}
