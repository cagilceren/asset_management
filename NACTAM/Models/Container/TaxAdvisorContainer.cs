using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using NACTAM.Hubs;
using NACTAM.Identity.Data;
using NACTAM.Models.Repositories;


namespace NACTAM.Models {

	public class TaxAdvisorContainer : ITaxAdvisorRepository {

		private readonly NACTAMContext _db;
		private readonly UserManager<User> _userManager;
		private readonly UserManager<PrivatePerson> _privatePersonManager;
		private readonly UserManager<TaxAdvisor> _taxAdvisorManager;
		private readonly IUserRepository _userRepository;
		private readonly INotificationRepository _notificationRepository;
		private readonly IHubContext<NotificationHub> _hub;

		public TaxAdvisorContainer(NACTAMContext db, UserManager<User> userManager, UserManager<PrivatePerson> privatePersonManager, UserManager<TaxAdvisor> taxAdvisorManager, INotificationRepository notificationRepository, IHubContext<NotificationHub> hub, IUserRepository userRep) {
			_db = db;
			_userManager = userManager;
			_privatePersonManager = privatePersonManager;
			_taxAdvisorManager = taxAdvisorManager;
			_userRepository = userRep;
			_notificationRepository = notificationRepository;
			_hub = hub;
		}

		/// <inheritdoc/>
		public IEnumerable<PrivatePerson> GetAllCustomers(TaxAdvisor myData)
			=> _db.PrivatePerson.Include(x => x.Advisors).Where(x => !x.Advisors.Contains(myData));

		/// <inheritdoc/>
		public IEnumerable<PrivatePerson> GetMyCustomers(TaxAdvisor myData)
			=> _db.PrivatePerson.Include(x => x.Advisors).Where(x => x.Advisors.Contains(myData));

		/// <inheritdoc/>
		public async Task ChangeAllowanceStatus(TaxAdvisor myData, String userName, InsightStatus status) {
			var user = await _privatePersonManager.FindByNameAsync(userName);
			var allowance = _db.InsightAllowance.FirstOrDefault(x => x.Advisor.UserName == myData.UserName && x.User == user);
			if (allowance != null) {
				switch (status) {
					case InsightStatus.Assigned: break;
					case InsightStatus.SimpleUnaccepted: await _notificationRepository.AddInsightRequest(myData, user, false); break;
					case InsightStatus.Simple: break;
					case InsightStatus.ExtendedUnaccepted: await _notificationRepository.AddInsightRequest(myData, user, true); break;
				}
				allowance.Status = status;
				await _db.SaveChangesAsync();
				await SendNotifyCenter(myData, user);
				await SendNotifyHeader(myData, user);
			} else {
				Console.WriteLine("Allowance NULL");
			}
		}

		/// <summary>
		/// helper function for managing notifications
		///
		/// author: Tuan Bui
		/// </summary>

		public async Task SendNotifyCenter(TaxAdvisor advisor, PrivatePerson recipient) {
			if (advisor.Customers.Any(x => x == recipient)) {
				string text = _notificationRepository.NotificationsToHTML(_notificationRepository.NotificationsFor(recipient).OrderBy(x => x.IsRead));
				await _hub.Clients.User(recipient.Id).SendAsync("ReceiveNotifyCenter", text);
			}
		}

		/// <summary>
		/// helper function for managing notifications
		///
		/// author: Tuan Bui
		/// </summary>
		public async Task SendNotifyHeader(TaxAdvisor advisor, PrivatePerson recipient) {
			if (advisor.Customers.Any(x => x == recipient)) {
				string text = _notificationRepository.NotificationsToHTML(_notificationRepository.NotificationsHeader(recipient).OrderBy(x => x.IsRead));
				await _hub.Clients.User(recipient.Id).SendAsync("ReceiveNotify", text, _notificationRepository.CountUnread(recipient));
			}
		}

		/// <inheritdoc/>
		public async Task AssignUser(TaxAdvisor myData, string userName) {
			var user = await _privatePersonManager.FindByNameAsync(userName);
			var allowance = new InsightAllowance { UserId = user.Id, User = user, AdvisorId = myData.Id, Advisor = myData, Status = InsightStatus.Assigned };
			await _db.InsightAllowance.AddAsync(allowance);
			await _notificationRepository.AddAssignedAdvisor(myData, user);
			await _db.SaveChangesAsync();
		}

		/// <inheritdoc/>
		public async Task RevokeUser(TaxAdvisor myData, string userName) {
			var user = await _privatePersonManager.FindByNameAsync(userName);
			var allowance = _db.InsightAllowance.FirstOrDefault(x => x.Advisor.UserName == myData.UserName && x.User == user);
			if (allowance != null)
				_db.InsightAllowance.Remove(allowance);
			await _notificationRepository.RemoveAssignedAdvisor(myData, user);
			await _db.SaveChangesAsync();
		}

		/// <inheritdoc/>
		public bool CheckExtendedInsightStatus(TaxAdvisor myData, string userId) {
			if (myData.Allowances != null) {
				var allMyAllowances = myData.Allowances.Where(x => x.UserId == userId && x.Status == InsightStatus.Extended);
				return allMyAllowances.Any();
			} else {
				return false;
			}
		}

		/// <inheritdoc/>
		public async Task<bool> CheckUserName(string userName) {
			return await _userManager.FindByNameAsync(userName) != null;
		}

		/// <inheritdoc/>
		public async Task<bool> CheckInsightStatus(string advisorUserName, string userName, InsightStatus shouldStatus) {
			var user = await _privatePersonManager.FindByNameAsync(userName);
			_db.Entry(user).Collection(x => x.Advisors).Load();
			if (user.Allowances != null) {
				return user.Allowances.Where(x => x.Advisor.UserName == advisorUserName && x.Status == shouldStatus).Any();
			} else {
				return false;
			}
		}

		/// <inheritdoc/>
		public async Task<bool> CheckIsAssigned(string advisorUserName, string userName) {
			var user = await _privatePersonManager.FindByNameAsync(userName);
			_db.Entry(user).Collection(x => x.Advisors).Load();
			if (user.Allowances != null) {
				return user.Allowances.Where(x => x.Advisor.UserName == advisorUserName).Any();
			} else {
				return false;
			}
		}

		/// <inheritdoc/>
		public async Task<bool> CheckIsUnassigned(string advisorUserName, string userName) {
			var user = await _privatePersonManager.FindByNameAsync(userName);
			_db.Entry(user).Collection(x => x.Advisors).Load();
			if (user.Allowances != null) {
				return user.Allowances.Where(x => x.Advisor.UserName == advisorUserName).Any() == false;
			} else {
				return true;
			}
		}

		/// <inheritdoc/>
		public async Task<bool> CheckImplyStatus(string advisorUserName, string userName, InsightStatus shouldStatus) {
			var user = await _privatePersonManager.FindByNameAsync(userName);
			_db.Entry(user).Collection(x => x.Advisors).Load();
			if (user.Allowances != null) {
				return user.Allowances.FirstOrDefault(x => x.Advisor.UserName == advisorUserName)?.Status >= shouldStatus;
			} else {
				return false;
			}
		}
	}

}
