using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilkCollector.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Farmers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Village = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RecordedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farmers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FatRateRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RateKind = table.Column<string>(type: "TEXT", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RupeesPerFatPointPerLiterPaise = table.Column<long>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    RecordedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FatRateRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settlements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    RecordedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settlements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false),
                    District = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LedgerEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FarmerId = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Kind = table.Column<string>(type: "TEXT", nullable: false),
                    AmountPaise = table.Column<long>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    SettlementId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LedgerEntries_Farmers_FarmerId",
                        column: x => x.FarmerId,
                        principalTable: "Farmers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PashuAahars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FarmerId = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PashuAaharName = table.Column<string>(type: "TEXT", nullable: false),
                    PricePaise = table.Column<long>(type: "INTEGER", nullable: false),
                    PaidPaise = table.Column<long>(type: "INTEGER", nullable: false),
                    Remark = table.Column<string>(type: "TEXT", nullable: true),
                    RecordedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PashuAahars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PashuAahars_Farmers_FarmerId",
                        column: x => x.FarmerId,
                        principalTable: "Farmers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettlementLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SettlementId = table.Column<int>(type: "INTEGER", nullable: false),
                    FarmerId = table.Column<int>(type: "INTEGER", nullable: false),
                    MilkAmountPaise = table.Column<long>(type: "INTEGER", nullable: false),
                    LedgerNetPaise = table.Column<long>(type: "INTEGER", nullable: false),
                    NetPayablePaise = table.Column<long>(type: "INTEGER", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaidAmountPaise = table.Column<long>(type: "INTEGER", nullable: true),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: true),
                    PaymentNote = table.Column<string>(type: "TEXT", nullable: true),
                    PaymentRecordedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettlementLines_Farmers_FarmerId",
                        column: x => x.FarmerId,
                        principalTable: "Farmers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SettlementLines_Settlements_SettlementId",
                        column: x => x.SettlementId,
                        principalTable: "Settlements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FarmerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CollectionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Shift = table.Column<string>(type: "TEXT", nullable: false),
                    Liters = table.Column<decimal>(type: "TEXT", nullable: false),
                    FatReading = table.Column<decimal>(type: "TEXT", nullable: false),
                    MilkAmountPaise = table.Column<long>(type: "INTEGER", nullable: true),
                    RecordedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collections_Farmers_FarmerId",
                        column: x => x.FarmerId,
                        principalTable: "Farmers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Collections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FarmerFatKindSegments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FarmerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Kind = table.Column<string>(type: "TEXT", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmerFatKindSegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FarmerFatKindSegments_Farmers_FarmerId",
                        column: x => x.FarmerId,
                        principalTable: "Farmers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FarmerFatKindSegments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Managers",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$67mgVTZJs3aFX4TSCLOumuUNOi.q0cnoVjjoFHso2SQ.MNiWBNTJO", "manager", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Collections_FarmerId_CollectionDate_Shift",
                table: "Collections",
                columns: new[] { "FarmerId", "CollectionDate", "Shift" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserId",
                table: "Collections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmerFatKindSegments_FarmerId",
                table: "FarmerFatKindSegments",
                column: "FarmerId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmerFatKindSegments_UserId",
                table: "FarmerFatKindSegments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Farmers_Name",
                table: "Farmers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntries_FarmerId",
                table: "LedgerEntries",
                column: "FarmerId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_Username",
                table: "Managers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PashuAahars_FarmerId",
                table: "PashuAahars",
                column: "FarmerId");

            migrationBuilder.CreateIndex(
                name: "IX_SettlementLines_FarmerId",
                table: "SettlementLines",
                column: "FarmerId");

            migrationBuilder.CreateIndex(
                name: "IX_SettlementLines_SettlementId",
                table: "SettlementLines",
                column: "SettlementId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "FarmerFatKindSegments");

            migrationBuilder.DropTable(
                name: "FatRateRules");

            migrationBuilder.DropTable(
                name: "LedgerEntries");

            migrationBuilder.DropTable(
                name: "Managers");

            migrationBuilder.DropTable(
                name: "PashuAahars");

            migrationBuilder.DropTable(
                name: "SettlementLines");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Farmers");

            migrationBuilder.DropTable(
                name: "Settlements");
        }
    }
}
