using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NACTAM.Migrations {
	/// <inheritdoc />
	public partial class RemoveWallet : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropColumn(
				name: "WalletId",
				table: "Transaction");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.AddColumn<int>(
				name: "WalletId",
				table: "Transaction",
				type: "INTEGER",
				nullable: true);
		}
	}
}
