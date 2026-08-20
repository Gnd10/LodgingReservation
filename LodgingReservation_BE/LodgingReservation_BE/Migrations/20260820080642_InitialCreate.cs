using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LodgingReservation_BE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EXTRA_SERVICE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EXTRA_SERVICE_NAME = table.Column<string>(type: "text", nullable: false),
                    PRICE = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    UNIT_TYPE = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXTRA_SERVICE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PROMOTION",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PROMO_CODE = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DISCOUNT_PERCENTAGE = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    MAX_DISCOUNT_CAP = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    VALID_UNTIL = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROMOTION", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ROOM_TYPE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ROOM_TYPE_NAME = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BASE_PRICE = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CAPACITY = table.Column<int>(type: "integer", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROOM_TYPE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EMAIL = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PASSWORD = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NAMA = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ROOM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ROOM_TYPE_ID = table.Column<long>(type: "bigint", nullable: false),
                    ROOM_NUMBER = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    STATUS = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROOM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ROOM_ROOM_TYPE_ROOM_TYPE_ID",
                        column: x => x.ROOM_TYPE_ID,
                        principalTable: "ROOM_TYPE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RESERVATION",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BOOKING_CODE = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    USER_ID = table.Column<long>(type: "bigint", nullable: false),
                    PROMOTION_ID = table.Column<long>(type: "bigint", nullable: false),
                    CHECK_IN_DATE = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CHECK_OUT_DATE = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    STATUS = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TOTAL_NIGHTS = table.Column<int>(type: "integer", nullable: false),
                    ROOM_SUB_TOTAL = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    LATE_CHECK_OUT_FEE = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    ADD_ONS_TOTAL = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    PROMO_DISCOUNT = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    GRAND_TOTAL = table.Column<decimal>(type: "numeric(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RESERVATION", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RESERVATION_PROMOTION_PROMOTION_ID",
                        column: x => x.PROMOTION_ID,
                        principalTable: "PROMOTION",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RESERVATION_USER_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PAYMENT",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RESERVATION_ID = table.Column<long>(type: "bigint", nullable: false),
                    INVOICE_NUMBER = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AMOUNT_PAID = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    METHOD = table.Column<string>(type: "text", nullable: false),
                    STATUS = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYMENT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PAYMENT_RESERVATION_RESERVATION_ID",
                        column: x => x.RESERVATION_ID,
                        principalTable: "RESERVATION",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RESERVATION_ADD_ON",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RESERVATION_ID = table.Column<long>(type: "bigint", nullable: false),
                    EXTRA_SERVICE_ID = table.Column<long>(type: "bigint", nullable: false),
                    QUANTITY = table.Column<int>(type: "integer", nullable: false),
                    UNIT_PRICE = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    SUB_TOTAL = table.Column<decimal>(type: "numeric(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RESERVATION_ADD_ON", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RESERVATION_ADD_ON_EXTRA_SERVICE_EXTRA_SERVICE_ID",
                        column: x => x.EXTRA_SERVICE_ID,
                        principalTable: "EXTRA_SERVICE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RESERVATION_ADD_ON_RESERVATION_RESERVATION_ID",
                        column: x => x.RESERVATION_ID,
                        principalTable: "RESERVATION",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RESERVATION_ROOM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RESERVATION_ID = table.Column<long>(type: "bigint", nullable: false),
                    ROOM_ID = table.Column<long>(type: "bigint", nullable: false),
                    PRICE_PER_NIGHT = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    TOTAL_ROOM_COST = table.Column<decimal>(type: "numeric(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RESERVATION_ROOM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RESERVATION_ROOM_RESERVATION_RESERVATION_ID",
                        column: x => x.RESERVATION_ID,
                        principalTable: "RESERVATION",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RESERVATION_ROOM_ROOM_ROOM_ID",
                        column: x => x.ROOM_ID,
                        principalTable: "ROOM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PAYMENT_INVOICE_NUMBER",
                table: "PAYMENT",
                column: "INVOICE_NUMBER",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PAYMENT_RESERVATION_ID",
                table: "PAYMENT",
                column: "RESERVATION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROMOTION_PROMO_CODE",
                table: "PROMOTION",
                column: "PROMO_CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_BOOKING_CODE",
                table: "RESERVATION",
                column: "BOOKING_CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_PROMOTION_ID",
                table: "RESERVATION",
                column: "PROMOTION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_USER_ID",
                table: "RESERVATION",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_ADD_ON_EXTRA_SERVICE_ID",
                table: "RESERVATION_ADD_ON",
                column: "EXTRA_SERVICE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_ADD_ON_RESERVATION_ID",
                table: "RESERVATION_ADD_ON",
                column: "RESERVATION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_ROOM_RESERVATION_ID",
                table: "RESERVATION_ROOM",
                column: "RESERVATION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_ROOM_ROOM_ID",
                table: "RESERVATION_ROOM",
                column: "ROOM_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROOM_ROOM_NUMBER",
                table: "ROOM",
                column: "ROOM_NUMBER",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ROOM_ROOM_TYPE_ID",
                table: "ROOM",
                column: "ROOM_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USER_EMAIL",
                table: "USER",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PAYMENT");

            migrationBuilder.DropTable(
                name: "RESERVATION_ADD_ON");

            migrationBuilder.DropTable(
                name: "RESERVATION_ROOM");

            migrationBuilder.DropTable(
                name: "EXTRA_SERVICE");

            migrationBuilder.DropTable(
                name: "RESERVATION");

            migrationBuilder.DropTable(
                name: "ROOM");

            migrationBuilder.DropTable(
                name: "PROMOTION");

            migrationBuilder.DropTable(
                name: "USER");

            migrationBuilder.DropTable(
                name: "ROOM_TYPE");
        }
    }
}
