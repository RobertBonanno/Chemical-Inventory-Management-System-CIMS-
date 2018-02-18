using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS4Carroll.Migrations
{
    public partial class changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemLog_ChemInventory_ChemInventoryId",
                table: "ChemLog");

            migrationBuilder.DropColumn(
                name: "CAT",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "CAT",
                table: "ChemInventory",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LOT",
                table: "ChemInventory",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CAT",
                table: "ChemicalEquipments",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LOT",
                table: "ChemicalEquipments",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CAT",
                table: "BioEquipments",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LOT",
                table: "BioEquipments",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CAT",
                table: "Animal",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LOT",
                table: "Animal",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChemInventoryId",
                table: "ChemLog",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemLog_ChemInventory_ChemInventoryId",
                table: "ChemLog",
                column: "ChemInventoryId",
                principalTable: "ChemInventory",
                principalColumn: "ChemInventoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChemLog_ChemInventory_ChemInventoryId",
                table: "ChemLog");

            migrationBuilder.DropColumn(
                name: "CAT",
                table: "ChemInventory");

            migrationBuilder.DropColumn(
                name: "LOT",
                table: "ChemInventory");

            migrationBuilder.DropColumn(
                name: "CAT",
                table: "ChemicalEquipments");

            migrationBuilder.DropColumn(
                name: "LOT",
                table: "ChemicalEquipments");

            migrationBuilder.DropColumn(
                name: "CAT",
                table: "BioEquipments");

            migrationBuilder.DropColumn(
                name: "LOT",
                table: "BioEquipments");

            migrationBuilder.DropColumn(
                name: "CAT",
                table: "Animal");

            migrationBuilder.DropColumn(
                name: "LOT",
                table: "Animal");

            migrationBuilder.AddColumn<string>(
                name: "CAT",
                table: "Orders",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Orders",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChemInventoryId",
                table: "ChemLog",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ChemLog_ChemInventory_ChemInventoryId",
                table: "ChemLog",
                column: "ChemInventoryId",
                principalTable: "ChemInventory",
                principalColumn: "ChemInventoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
