using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NACTAM.Migrations {
	/// <inheritdoc />
	public partial class InitialCreate : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "AspNetRoles",
				columns: table => new {
					Id = table.Column<string>(type: "TEXT", nullable: false),
					Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
					NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetRoles", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUsers",
				columns: table => new {
					Id = table.Column<string>(type: "TEXT", nullable: false),
					FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
					LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
					StreetName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
					HouseNumber = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
					City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
					ZIP = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
					Icon = table.Column<byte[]>(type: "BLOB", nullable: true),
					DarkMode = table.Column<bool>(type: "INTEGER", nullable: false),
					EmailNotification = table.Column<bool>(type: "INTEGER", nullable: false),
					Discriminator = table.Column<string>(type: "TEXT", nullable: false),
					UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
					NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
					Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
					NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
					EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
					PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
					SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
					PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
					TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
					LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
					AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetUsers", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "CryptoCurrency",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					ShortName = table.Column<string>(type: "TEXT", nullable: false),
					ApiId = table.Column<string>(type: "TEXT", nullable: false),
					Name = table.Column<string>(type: "TEXT", nullable: false),
					Logo = table.Column<string>(type: "TEXT", nullable: true),
					Rate = table.Column<decimal>(type: "TEXT", nullable: true),
					LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_CryptoCurrency", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "TaxAllowance",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Year = table.Column<int>(type: "INTEGER", nullable: false),
					Amount = table.Column<decimal>(type: "TEXT", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_TaxAllowance", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Transaction",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					UserId = table.Column<string>(type: "TEXT", nullable: false),
					Amount = table.Column<decimal>(type: "TEXT", nullable: false),
					Fee = table.Column<decimal>(type: "TEXT", nullable: false),
					Date = table.Column<DateTime>(type: "TEXT", nullable: false),
					CurrencyId = table.Column<int>(type: "INTEGER", nullable: false),
					ExchangeRate = table.Column<decimal>(type: "TEXT", nullable: false),
					Type = table.Column<int>(type: "INTEGER", nullable: false),
					Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
					WalletId = table.Column<int>(type: "INTEGER", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_Transaction", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AspNetRoleClaims",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					RoleId = table.Column<string>(type: "TEXT", nullable: false),
					ClaimType = table.Column<string>(type: "TEXT", nullable: true),
					ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserClaims",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					UserId = table.Column<string>(type: "TEXT", nullable: false),
					ClaimType = table.Column<string>(type: "TEXT", nullable: true),
					ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetUserClaims_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserLogins",
				columns: table => new {
					LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
					ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
					ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
					UserId = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
					table.ForeignKey(
						name: "FK_AspNetUserLogins_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserRoles",
				columns: table => new {
					UserId = table.Column<string>(type: "TEXT", nullable: false),
					RoleId = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserTokens",
				columns: table => new {
					UserId = table.Column<string>(type: "TEXT", nullable: false),
					LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
					Name = table.Column<string>(type: "TEXT", nullable: false),
					Value = table.Column<string>(type: "TEXT", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
					table.ForeignKey(
						name: "FK_AspNetUserTokens_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AssignedAdvisor",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					IsRead = table.Column<int>(type: "INTEGER", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					RecipientId = table.Column<string>(type: "TEXT", nullable: false),
					TaxAdvisorId = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_AssignedAdvisor", x => x.Id);
					table.ForeignKey(
						name: "FK_AssignedAdvisor_AspNetUsers_RecipientId",
						column: x => x.RecipientId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_AssignedAdvisor_AspNetUsers_TaxAdvisorId",
						column: x => x.TaxAdvisorId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "InsightAllowance",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					UserId = table.Column<string>(type: "TEXT", nullable: false),
					AdvisorId = table.Column<string>(type: "TEXT", nullable: false),
					Status = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_InsightAllowance", x => x.Id);
					table.ForeignKey(
						name: "FK_InsightAllowance_AspNetUsers_AdvisorId",
						column: x => x.AdvisorId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_InsightAllowance_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "InsightRequest",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					IsRead = table.Column<int>(type: "INTEGER", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					IsExtended = table.Column<bool>(type: "INTEGER", nullable: false),
					RecipientId = table.Column<string>(type: "TEXT", nullable: false),
					TaxAdvisorId = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_InsightRequest", x => x.Id);
					table.ForeignKey(
						name: "FK_InsightRequest_AspNetUsers_RecipientId",
						column: x => x.RecipientId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_InsightRequest_AspNetUsers_TaxAdvisorId",
						column: x => x.TaxAdvisorId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "InsightResponse",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					IsRead = table.Column<int>(type: "INTEGER", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					IsExtended = table.Column<bool>(type: "INTEGER", nullable: false),
					IsAccepted = table.Column<bool>(type: "INTEGER", nullable: false),
					PrivatePersonId = table.Column<string>(type: "TEXT", nullable: false),
					RecipientId = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_InsightResponse", x => x.Id);
					table.ForeignKey(
						name: "FK_InsightResponse_AspNetUsers_PrivatePersonId",
						column: x => x.PrivatePersonId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_InsightResponse_AspNetUsers_RecipientId",
						column: x => x.RecipientId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "RevokedAdvisor",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					IsRead = table.Column<int>(type: "INTEGER", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					RecipientId = table.Column<string>(type: "TEXT", nullable: false),
					TaxAdvisorId = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_RevokedAdvisor", x => x.Id);
					table.ForeignKey(
						name: "FK_RevokedAdvisor_AspNetUsers_RecipientId",
						column: x => x.RecipientId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_RevokedAdvisor_AspNetUsers_TaxAdvisorId",
						column: x => x.TaxAdvisorId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "SystemMessage",
				columns: table => new {
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Text = table.Column<string>(type: "TEXT", nullable: false),
					RecipientId = table.Column<string>(type: "TEXT", nullable: false),
					IsRead = table.Column<int>(type: "INTEGER", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_SystemMessage", x => x.Id);
					table.ForeignKey(
						name: "FK_SystemMessage_AspNetUsers_RecipientId",
						column: x => x.RecipientId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_AspNetRoleClaims_RoleId",
				table: "AspNetRoleClaims",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "RoleNameIndex",
				table: "AspNetRoles",
				column: "NormalizedName",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserClaims_UserId",
				table: "AspNetUserClaims",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserLogins_UserId",
				table: "AspNetUserLogins",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserRoles_RoleId",
				table: "AspNetUserRoles",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "EmailIndex",
				table: "AspNetUsers",
				column: "NormalizedEmail");

			migrationBuilder.CreateIndex(
				name: "UserNameIndex",
				table: "AspNetUsers",
				column: "NormalizedUserName",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_AssignedAdvisor_RecipientId",
				table: "AssignedAdvisor",
				column: "RecipientId");

			migrationBuilder.CreateIndex(
				name: "IX_AssignedAdvisor_TaxAdvisorId",
				table: "AssignedAdvisor",
				column: "TaxAdvisorId");

			migrationBuilder.CreateIndex(
				name: "IX_InsightAllowance_AdvisorId",
				table: "InsightAllowance",
				column: "AdvisorId");

			migrationBuilder.CreateIndex(
				name: "IX_InsightAllowance_UserId",
				table: "InsightAllowance",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_InsightRequest_RecipientId",
				table: "InsightRequest",
				column: "RecipientId");

			migrationBuilder.CreateIndex(
				name: "IX_InsightRequest_TaxAdvisorId",
				table: "InsightRequest",
				column: "TaxAdvisorId");

			migrationBuilder.CreateIndex(
				name: "IX_InsightResponse_PrivatePersonId",
				table: "InsightResponse",
				column: "PrivatePersonId");

			migrationBuilder.CreateIndex(
				name: "IX_InsightResponse_RecipientId",
				table: "InsightResponse",
				column: "RecipientId");

			migrationBuilder.CreateIndex(
				name: "IX_RevokedAdvisor_RecipientId",
				table: "RevokedAdvisor",
				column: "RecipientId");

			migrationBuilder.CreateIndex(
				name: "IX_RevokedAdvisor_TaxAdvisorId",
				table: "RevokedAdvisor",
				column: "TaxAdvisorId");

			migrationBuilder.CreateIndex(
				name: "IX_SystemMessage_RecipientId",
				table: "SystemMessage",
				column: "RecipientId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "AspNetRoleClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserLogins");

			migrationBuilder.DropTable(
				name: "AspNetUserRoles");

			migrationBuilder.DropTable(
				name: "AspNetUserTokens");

			migrationBuilder.DropTable(
				name: "AssignedAdvisor");

			migrationBuilder.DropTable(
				name: "CryptoCurrency");

			migrationBuilder.DropTable(
				name: "InsightAllowance");

			migrationBuilder.DropTable(
				name: "InsightRequest");

			migrationBuilder.DropTable(
				name: "InsightResponse");

			migrationBuilder.DropTable(
				name: "RevokedAdvisor");

			migrationBuilder.DropTable(
				name: "SystemMessage");

			migrationBuilder.DropTable(
				name: "TaxAllowance");

			migrationBuilder.DropTable(
				name: "Transaction");

			migrationBuilder.DropTable(
				name: "AspNetRoles");

			migrationBuilder.DropTable(
				name: "AspNetUsers");
		}
	}
}
