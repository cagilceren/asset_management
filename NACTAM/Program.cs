using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NACTAM.Hubs;
using NACTAM.Identity.Data;
using NACTAM.Models;
using NACTAM.Models.API;
using NACTAM.Models.Repositories;
using NACTAM.Models.TaxRecommendation;


using NACTAM.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();


var connectionString = builder.Configuration.GetConnectionString("NACTAMDb") ??
	throw new InvalidOperationException("Connection string 'NACTAMIdentityDbContextConnection' not found.");

var emailConfiguration = builder.Configuration.GetSection("EmailConfiguration")?.Get<EmailConfiguration>() ??
	throw new InvalidOperationException("Missing Email configuration");
builder.Services.AddSingleton<EmailConfiguration>(emailConfiguration);


builder.Services.AddDbContext<NACTAMContext>(options => {
	options.UseSqlite(connectionString).EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<User, IdentityRole>(options => {
	options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
})
	.AddEntityFrameworkStores<NACTAMContext>()
	.AddDefaultTokenProviders();
builder.Services.AddIdentityCore<PrivatePerson>()
	.AddRoles<IdentityRole>()
	.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<PrivatePerson, IdentityRole>>()
	.AddEntityFrameworkStores<NACTAMContext>();
builder.Services.AddIdentityCore<TaxAdvisor>()
	.AddRoles<IdentityRole>()
	.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<TaxAdvisor, IdentityRole>>()
	.AddEntityFrameworkStores<NACTAMContext>();
builder.Services.AddIdentityCore<Admin>()
	.AddRoles<IdentityRole>()
	.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<Admin, IdentityRole>>()
	.AddEntityFrameworkStores<NACTAMContext>();

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IUserRepository, UserContainer>();
builder.Services.AddScoped<INotificationRepository, NotificationContainer>();

builder.Services.AddSignalR();

builder.Services.AddScoped<ITaxAdvisorRepository, TaxAdvisorContainer>();
builder.Services.AddScoped<ITransactionRepository, TransactionContainer>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyContainer>();
builder.Services.AddScoped<ICurrencyApi, CoingeckoApi>();
builder.Services.AddScoped<ITaxRule, GermanTaxRule>();
builder.Services.AddScoped<ITaxCalculations, TaxCalculationsService>();

builder.Services.AddScoped<ISeeding, Seeding>();

builder.Services.Configure<IdentityOptions>(options => {
	// Password settings.
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequireUppercase = true;
	options.Password.RequiredLength = 6;
	options.Password.RequiredUniqueChars = 1;

	// Lockout settings.
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;

	// User settings.
	options.User.AllowedUserNameCharacters =
	"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options => {
	// Cookie settings
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(45);

	options.LoginPath = "/Login";
	options.LogoutPath = "/Logout";
	options.AccessDeniedPath = "/AccessDenied";
	options.SlidingExpiration = true;
});


// Add services to the container.
builder.Services.AddControllersWithViews(options => {
	options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});



var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithRedirects("/Error/{0}");
app.UseRouting();
app.UseAuthentication(); ;


// initializing the roles
using (var scope = app.Services.CreateScope()) {
	var seeding = scope.ServiceProvider.GetRequiredService<ISeeding>();
	var mailClient = scope.ServiceProvider.GetRequiredService<IEmailSender>();
	await seeding.Init();
}


app.UseAuthorization();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<NotificationHub>("/NotificationHub");
app.Run();

