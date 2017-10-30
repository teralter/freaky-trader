using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FreakyTrader.Data.Migrations
{
    public partial class Transaction1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_RepDate",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RepDate",
                table: "Transactions",
                column: "RepDate")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_RepDate",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RepDate",
                table: "Transactions",
                column: "RepDate",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }
    }
}
