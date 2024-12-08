using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialApi.Migrations
{
    /// <inheritdoc />
    public partial class SixthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "UserNfts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "long");

            migrationBuilder.AlterColumn<long>(
                name: "RecordId",
                table: "UserNfts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "long");

            migrationBuilder.AlterColumn<long>(
                name: "UserRecordId",
                table: "UserNfts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "long")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "LogUser",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "long")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "Views",
                table: "LogRecord",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "long");

            migrationBuilder.AlterColumn<long>(
                name: "Likes",
                table: "LogRecord",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "long");

            migrationBuilder.AlterColumn<long>(
                name: "RecordId",
                table: "LogRecord",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "long")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserNfts",
                type: "long",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "RecordId",
                table: "UserNfts",
                type: "long",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "UserRecordId",
                table: "UserNfts",
                type: "long",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "LogUser",
                type: "long",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Views",
                table: "LogRecord",
                type: "long",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Likes",
                table: "LogRecord",
                type: "long",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "RecordId",
                table: "LogRecord",
                type: "long",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");*/
        }
    }
}
