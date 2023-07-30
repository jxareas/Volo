﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IssueTracking.Migrations
{
    public partial class CreationTimeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppIssues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCommentTime",
                table: "AppIssues",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppIssues");

            migrationBuilder.DropColumn(
                name: "LastCommentTime",
                table: "AppIssues");
        }
    }
}
