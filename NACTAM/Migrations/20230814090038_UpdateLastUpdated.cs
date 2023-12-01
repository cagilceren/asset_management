using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NACTAM.Migrations {
	/// <inheritdoc />
	public partial class UpdateLastUpdated : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.Sql(@"
		        UPDATE CryptoCurrency
		        SET LastUpdated = NULL
		        WHERE LastUpdated IS NOT NULL AND Rate IS NULL;
		    ");

		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {

		}
	}
}
