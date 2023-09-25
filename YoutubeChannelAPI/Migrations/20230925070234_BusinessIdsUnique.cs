using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeChannelAPI.Migrations
{
    /// <inheritdoc />
    public partial class BusinessIdsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoId",
                table: "Videos",
                column: "VideoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_ChannelId",
                table: "Channels",
                column: "ChannelId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_VideoId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Channels_ChannelId",
                table: "Channels");
        }
    }
}
