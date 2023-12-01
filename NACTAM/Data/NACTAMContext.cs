using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using NACTAM.Models;

namespace NACTAM.Identity.Data;


/// <summary>
/// single Context based of IdentityDbContext for easier usage with
/// <c>UserManager</c> and <c>SignInManager</c>
///
/// author: Tuan Bui
/// </summary>
public class NACTAMContext : IdentityDbContext<User> {
	/// <summary>
	/// Data table for Private Persons
	/// </summary>
	public virtual DbSet<PrivatePerson> PrivatePerson { get; set; }
	/// <summary>
	/// Data table for Tax Advisors, autoloads customers
	/// </summary>
	public virtual DbSet<TaxAdvisor> TaxAdvisor { get; set; }
	/// <summary>
	/// Data table for Admins
	/// </summary>
	public virtual DbSet<Admin> Admin { get; set; }
	/// <summary>
	/// Data table for Insight Requests, autloads Recipient and Requester
	/// </summary>
	public virtual DbSet<InsightRequest> InsightRequest { get; set; }
	/// <summary>
	/// Data table for Insight Requests, autloads Recipient and Respondant
	/// </summary>
	public virtual DbSet<InsightResponse> InsightResponse { get; set; }
	/// <summary>
	/// Data table for Insight Allowance, connects Tax advisor with private person
	/// </summary>
	public virtual DbSet<InsightAllowance> InsightAllowance { get; set; }
	/// <summary>
	/// Data table for TaxAllowances
	/// </summary>
	public virtual DbSet<TaxAllowance> TaxAllowance { get; set; }
	/// <summary>
	/// Data table for Systemmessages, autoloads the Recipient
	/// </summary>
	public virtual DbSet<SystemMessage> SystemMessage { get; set; }
	/// <summary>
	/// Data table for Insight Requests, autloads Recipient and Advisor
	/// </summary>
	public virtual DbSet<AssignedAdvisor> AssignedAdvisor { get; set; }
	/// <summary>
	/// Data table for Insight Requests, autloads Recipient and Advisor
	/// </summary>
	public virtual DbSet<RevokedAdvisor> RevokedAdvisor { get; set; }
	/// <summary>
	/// Data table for transactions
	/// </summary>
	public virtual DbSet<Transaction> Transaction { get; set; }
	/// <summary>
	/// Data table for cryptocurrencies
	/// </summary>
	public virtual DbSet<CryptoCurrency> CryptoCurrency { get; set; }



	/// <remarks>
	/// for unit tests
	/// </remarks>
	public NACTAMContext() { }


	/// <remarks>
	/// for main usage
	/// </remarks>
	public NACTAMContext(DbContextOptions<NACTAMContext> options)
		: base(options) {
	}

	protected override void OnModelCreating(ModelBuilder builder) {
		base.OnModelCreating(builder);

		builder.Entity<PrivatePerson>()
			.HasMany(e => e.Advisors)
			.WithMany(e => e.Customers)
			.UsingEntity<InsightAllowance>();
		builder.Entity<TaxAdvisor>()
			.Navigation(x => x.Customers)
			.AutoInclude();
		builder.Entity<InsightRequest>()
			.Navigation(x => x.Recipient)
			.AutoInclude();
		builder.Entity<InsightRequest>()
			.Navigation(x => x.TaxAdvisor)
			.AutoInclude();
		builder.Entity<InsightResponse>()
			.Navigation(x => x.PrivatePerson)
			.AutoInclude();
		builder.Entity<InsightResponse>()
			.Navigation(x => x.Recipient)
			.AutoInclude();
		builder.Entity<SystemMessage>()
			.Navigation(x => x.Recipient)
			.AutoInclude();
		builder.Entity<AssignedAdvisor>()
			.Navigation(x => x.Recipient)
			.AutoInclude();
		builder.Entity<AssignedAdvisor>()
			.Navigation(x => x.TaxAdvisor)
			.AutoInclude();
		builder.Entity<RevokedAdvisor>()
			.Navigation(x => x.Recipient)
			.AutoInclude();
		builder.Entity<RevokedAdvisor>()
			.Navigation(x => x.TaxAdvisor)
			.AutoInclude();
		builder.Entity<CryptoCurrency>()
				.Property(e => e.Rate)
				.HasPrecision(18, 6);
		builder.Entity<Transaction>()
				.Property(e => e.Amount)
				.HasPrecision(18, 6);
		builder.Entity<Transaction>().Property(e => e.ExchangeRate).HasPrecision(18, 6);
		builder.Entity<Transaction>().Property(e => e.Fee).HasPrecision(18, 6);
		// Customize the ASP.NET Identity model and override the defaults if needed.
		// For example, you can rename the ASP.NET Identity table names and more.
		// Add your customizations after calling base.OnModelCreating(builder);
	}
}
