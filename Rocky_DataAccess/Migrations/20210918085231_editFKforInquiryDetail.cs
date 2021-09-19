using Microsoft.EntityFrameworkCore.Migrations;

namespace Rocky_DataAccess.Migrations
{
    public partial class editFKforInquiryDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetail_Product_InquiryHeaderId",
                table: "InquiryDetail");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryDetail_ProductId",
                table: "InquiryDetail",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetail_Product_ProductId",
                table: "InquiryDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetail_Product_ProductId",
                table: "InquiryDetail");

            migrationBuilder.DropIndex(
                name: "IX_InquiryDetail_ProductId",
                table: "InquiryDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetail_Product_InquiryHeaderId",
                table: "InquiryDetail",
                column: "InquiryHeaderId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
