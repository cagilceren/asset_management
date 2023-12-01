using System.Data;
using System.Security.Authentication;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NACTAM.Identity.Data;
using NACTAM.Models.Repositories;
using NACTAM.ViewModels;

namespace NACTAM.Models {

	/// <summary>
	/// helper class for everything Account and User related, like the
	/// connections between <c>PrivatePerson</c> and <c>TaxAdvisor</c>
	///
	/// author: Tuan Bui, Niklas Thuer
	/// </summary>
	public class UserContainer : IUserRepository {
		private readonly NACTAMContext _db;
		private readonly UserManager<User> _userManager;
		private readonly UserManager<PrivatePerson> _privatePersonManager;
		private readonly UserManager<Admin> _adminManager;
		private readonly UserManager<TaxAdvisor> _taxAdvisorManager;
		private readonly INotificationRepository _notRep;
		private readonly IEmailSender _emailSend;
		private readonly IPasswordValidator<User> _passwordValidator;

		/// <summary>
		/// Constructor for User Container, gets called by dependency injection
		/// </summary>
		public UserContainer(NACTAMContext db, UserManager<User> userManager, UserManager<PrivatePerson> privatePersonManager, UserManager<TaxAdvisor> taxAdvisorManager, UserManager<Admin> adminManager, INotificationRepository notRep, IEmailSender emailSend, IPasswordValidator<User> passwordValidator) {
			_db = db;
			_userManager = userManager;
			_privatePersonManager = privatePersonManager;
			_adminManager = adminManager;
			_taxAdvisorManager = taxAdvisorManager;
			_notRep = notRep;
			_emailSend = emailSend;
			_passwordValidator = passwordValidator;
		}

		/// <inheritdoc/>
		public async Task<IdentityResult> AddFromViewModel(CreateUserViewModel viewmodel) {
			if (viewmodel.Type == "Private Person") {
				PrivatePerson user = new PrivatePerson {
					Email = viewmodel.Input.Email,
					UserName = viewmodel.UserName,
					FirstName = viewmodel.FirstName,
					LastName = viewmodel.LastName,
					StreetName = viewmodel.StreetName,
					HouseNumber = viewmodel.HouseNumber,
					City = viewmodel.City,
					ZIP = viewmodel.ZIP,
					Icon = viewmodel.IconBytes ?? ColorUtils.GenerateRandomProfilePicture(),
					PhoneNumber = viewmodel.PhoneNumber,
					EmailNotification = viewmodel.EmailNotification
				};
				return await AddPrivatePerson(user, viewmodel.Input.Password);
			} else if (viewmodel.Type == "Tax Advisor") {
				TaxAdvisor user = new TaxAdvisor {
					Email = viewmodel.Input.Email,
					UserName = viewmodel.UserName,
					FirstName = viewmodel.FirstName,
					LastName = viewmodel.LastName,
					StreetName = viewmodel.StreetName,
					HouseNumber = viewmodel.HouseNumber,
					City = viewmodel.City,
					ZIP = viewmodel.ZIP,
					Icon = viewmodel.IconBytes ?? ColorUtils.GenerateRandomProfilePicture(),
					PhoneNumber = viewmodel.PhoneNumber,
					EmailNotification = viewmodel.EmailNotification
				};
				return await AddTaxAdvisor(user, viewmodel.Input.Password);
			}
			throw new InvalidConstraintException("wrong viewmodel type");
		}

		/// <inheritdoc/>
		public async Task<IdentityResult> AddPrivatePerson(PrivatePerson user, string password) {
			if (user.Icon == null)
				user.Icon = ColorUtils.GenerateRandomProfilePicture();
			var result = await _privatePersonManager.CreateAsync(user, password);
			if (result.Succeeded) {
				await _privatePersonManager.AddToRoleAsync(user, "PrivatePerson");
				await _db.SaveChangesAsync();
			}
			return result;
		}

		/// <inheritdoc/>
		public async Task<IdentityResult> AddTaxAdvisor(TaxAdvisor user, string password) {
			if (user.Icon == null)
				user.Icon = ColorUtils.GenerateRandomProfilePicture();
			var result = await _taxAdvisorManager.CreateAsync(user, password);
			if (result.Succeeded) {
				await _taxAdvisorManager.AddToRoleAsync(user, "TaxAdvisor");
				await _db.SaveChangesAsync();
			}
			return result;
		}

		/// <inheritdoc/>
		public async Task<IdentityResult> AddAdmin(Admin user, string password) {
			if (user.Icon == null)
				user.Icon = ColorUtils.GenerateRandomProfilePicture();
			var result = await _adminManager.CreateAsync(user, password);
			await _adminManager.AddToRoleAsync(user, "Admin");
			await _db.SaveChangesAsync();
			return result;
		}

		/// <inheritdoc/>
		public async Task<PrivatePerson?> GetPrivatePersonByName(string username)
			=> await _db.PrivatePerson
				.Include(x => x.Advisors)
				.SingleOrDefaultAsync(x => x.UserName == username);

		/// <inheritdoc/>
		public async Task LoadAdvisors(PrivatePerson user) {
			await _db.Entry(user)
				.Collection(b => b.Advisors)
				.LoadAsync();

		}

		/// <inheritdoc/>
		public async Task UpdateUser(User user, User values, byte[]? profile) {
			user.UserName = values.UserName;

			if (profile != null)
				user.Icon = profile;

			user.StreetName = values.StreetName;
			user.City = values.City;
			user.HouseNumber = values.HouseNumber;
			user.ZIP = values.ZIP;
			user.Email = values.Email;
			user.PhoneNumber = values.PhoneNumber;
			user.FirstName = values.FirstName;
			user.LastName = values.LastName;
			await _userManager.UpdateAsync(user);
			await _db.SaveChangesAsync();
		}

		/// <inheritdoc/>
		public IEnumerable<InsightAllowance> GetAllowances(PrivatePerson user)
			=> _db.InsightAllowance.Include(x => x.Advisor).Where(x => x.User == user);


		/// <inheritdoc/>
		public async Task AcceptRequest(PrivatePerson user, TaxAdvisor advisor, bool isExtended) {
			var allowance = _db.InsightAllowance.First(x => x.Advisor == advisor && x.User == user);
			allowance.Status = isExtended ? InsightStatus.Extended : InsightStatus.Simple;
			await _db.SaveChangesAsync();
			await _notRep.HandleInsightRequest(advisor, user, isExtended);
			await _notRep.AddInsightResponse(user, advisor, isExtended, true);
		}

		/// <inheritdoc/>
		public async Task DenyRequest(PrivatePerson user, TaxAdvisor advisor, bool isExtended) {
			var allowance = _db.InsightAllowance.First(x => x.Advisor == advisor && x.User == user);
			if (allowance.Status == InsightStatus.ExtendedUnaccepted) {
				allowance.Status = InsightStatus.Simple;
			} else if (allowance.Status == InsightStatus.SimpleUnaccepted) {
				allowance.Status = InsightStatus.Assigned;
			}
			await _db.SaveChangesAsync();
			await _notRep.HandleInsightRequest(advisor, user, isExtended);
			await _notRep.AddInsightResponse(user, advisor, isExtended, false);
		}

		/// <inheritdoc/>
		public async Task RemoveInsight(PrivatePerson user, TaxAdvisor advisor) {
			var allowance = _db.InsightAllowance.First(x => x.Advisor == advisor && x.User == user);
			if (allowance.Status == InsightStatus.Extended) {
				allowance.Status = InsightStatus.Simple;
			} else {
				allowance.Status = InsightStatus.Assigned;
			}
			await _db.SaveChangesAsync();
		}

		/// <inheritdoc/>
		public async Task SetDarkmode(User user, bool isDark) {
			user.DarkMode = isDark;
			await _db.SaveChangesAsync();
		}

		/// <inheritdoc/>
		public IEnumerable<TaxAdvisor> GetAllTaxAdvisors()
			=> _db.TaxAdvisor;

		/// <inheritdoc/>
		public async Task DeleteUser(User user) {
			await _userManager.DeleteAsync(user);
			await _db.SaveChangesAsync();
		}

		/// <inheritdoc/>
		public async Task<IdentityResult> UpdatePassword(User user, string oldPassword, string newPassword) {
			var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
			if (result == PasswordVerificationResult.Failed) {
				throw new InvalidCredentialException("wrong old password");
			}
			var identityResult = await _passwordValidator.ValidateAsync(_userManager, user, newPassword);
			if (identityResult.Succeeded) {
				await _userManager.RemovePasswordAsync(user);
				await _userManager.AddPasswordAsync(user, newPassword);

				await _db.SaveChangesAsync();
			}
			return identityResult;
		}

		/// <inheritdoc/>
		public async Task<string> SendResetPasswordToken(User user) {
			string s = await _userManager.GeneratePasswordResetTokenAsync(user);
			var (url1, url2) = _emailSend.GetURL();
			await _emailSend.SendEmailAsync(
				new Mail(new List<User> { user }, "NACTAM: Password reset", $"This is an email to reset your password. If it wasn't you, who triggered this email, you can safely ignore it. Otherwise use this link to reset your password: <a href=\"{url1}/ResetPassword?token={s}&name={user.UserName}\" href=\"{url2}/ResetPassword?token={s}&name={user.UserName}\">{url1}/ResetPassword?token={s}&name={user.UserName}<br/>oder<br/>{url2}/ResetPassword?token={s}&name={user.UserName}</a>")
			);
			await _db.SaveChangesAsync();
			return s;
		}

		/// <inheritdoc/>
		public async Task<IdentityResult> ResetPassword(User user, string newPassword, string token) {
			var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
			await _db.SaveChangesAsync();
			return result;
		}
	}
}
