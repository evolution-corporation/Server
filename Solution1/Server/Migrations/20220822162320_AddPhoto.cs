using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Server.Migrations
{
    public partial class AddPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMDs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    MeditationsId = table.Column<List<int>>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMDs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meditations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Language = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TypeMeditation = table.Column<int>(type: "integer", nullable: false),
                    CountDay = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<int>(type: "integer", nullable: false),
                    IsSubscribed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meditations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ExpoToken = table.Column<string>(type: "text", nullable: false),
                    NotificationFrequency = table.Column<int>(type: "integer", nullable: false),
                    LastNotification = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSubscribedToNotification = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    PaymentDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Confirm = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscribes",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    WhenSubscribe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeSubscribe = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribes", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NickName = table.Column<string>(type: "text", nullable: true),
                    Birthday = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    DateTimeRegistration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Photo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserMeditations",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    MeditationId = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMeditations", x => new { x.UserId, x.MeditationId });
                    table.ForeignKey(
                        name: "FK_UserMeditations_Meditations_MeditationId",
                        column: x => x.MeditationId,
                        principalTable: "Meditations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMeditations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMeditations_MeditationId",
                table: "UserMeditations",
                column: "MeditationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMDs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Subscribes");

            migrationBuilder.DropTable(
                name: "UserMeditations");

            migrationBuilder.DropTable(
                name: "Meditations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
