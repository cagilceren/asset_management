using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using NACTAM.Exceptions;
using NACTAM.Hubs;
using NACTAM.Identity.Data;
using NACTAM.Models.Repositories;
using NACTAM.UnitTestExtension;

namespace NACTAM.UnitTests;

public class TaxAdvisorContainerTest {


	/// <summary>
	/// Unit Test for Tax Advisor Container
	///
	/// author: Thuer Niklas
	/// </summary>


	public List<PrivatePerson> PrivatePersonsList;
	public List<TaxAdvisor> TaxAdvisorsList;
	public List<InsightAllowance> InsightsList;

	public Mock<NACTAMContext> NactamContext;
	public MockUserManager<User> MockUserManager;
	public MockUserManager<PrivatePerson> MockPrivatePersonManager;
	public MockUserManager<TaxAdvisor> MockTaxAdvisorManager;
	public MockUserManager<Admin> MockAdminManager;
	public MockSignInManager<User> MockSignInManager;
	public Mock<INotificationRepository> _notRep = new Mock<INotificationRepository>();
	public Mock<IHubContext<NotificationHub>> _hub = new Mock<IHubContext<NotificationHub>>();
	public Mock<IUserRepository> _userRep = new Mock<IUserRepository>();
	private TaxAdvisor advisor;
	private PrivatePerson user;

	private TaxAdvisorContainer Container;

	public TaxAdvisorContainerTest() {
		NactamContext = new Mock<NACTAMContext>();
		PrivatePersonsList = new List<PrivatePerson>() {
			new PrivatePerson { Id = "coolid", UserName = "Test", FirstName = "Christian", LastName = "Jaumann" }
		};
		TaxAdvisorsList = new List<TaxAdvisor>() {
			new TaxAdvisor {  Id = "coloid",  UserName = "TaxAdvisor", FirstName = "Satanist", LastName = "NeinIfrau", Customers = PrivatePersonsList.Take(1).ToList() }
		};
		InsightsList = new List<InsightAllowance> {
			new InsightAllowance { Id = 1, UserId = "coolid", AdvisorId = "coloid" }
		};
		NactamContext
			.Setup(x => x.PrivatePerson)
			.ReturnsDbSet(PrivatePersonsList);
		NactamContext
			.Setup(x => x.TaxAdvisor)
			.ReturnsDbSet(TaxAdvisorsList);
		NactamContext
			.Setup(x => x.Users)
			.ReturnsDbSet(((IEnumerable<User>)PrivatePersonsList.Concat((IEnumerable<User>)TaxAdvisorsList)).ToList());
		NactamContext
			.Setup(x => x.InsightAllowance)
			.ReturnsDbSet(InsightsList);
		MockUserManager = new MockUserManager<User>(((IEnumerable<User>)PrivatePersonsList.Concat((IEnumerable<User>)TaxAdvisorsList)).ToList());
		MockPrivatePersonManager = new MockUserManager<PrivatePerson>(PrivatePersonsList);
		MockTaxAdvisorManager = new MockUserManager<TaxAdvisor>(TaxAdvisorsList);
		MockAdminManager = new MockUserManager<Admin>(new List<Admin> { });
		MockSignInManager = new MockSignInManager<User>(MockUserManager);

		advisor = new TaxAdvisor();
		user = new PrivatePerson();

		Container = new TaxAdvisorContainer(NactamContext.Object, MockUserManager, MockPrivatePersonManager, MockTaxAdvisorManager, _notRep.Object, _hub.Object, _userRep.Object);
	}


	[Fact]
	public async void TestCreateInvalidTransaction() {
		// Create:
		advisor.Id = "1";
		advisor.UserName = "Advisor1";
		user.Id = "2";
		user.UserName = "User1";

		var userList = new List<PrivatePerson>();
		userList.Add(user);
		var advisorList = new List<TaxAdvisor>();
		advisorList.Add(advisor);

		var allowance = new InsightAllowance {
			Id = 1,
			UserId = user.Id,
			User = user,
			AdvisorId = advisor.Id,
			Advisor = advisor,
			Status = InsightStatus.Simple
		};
		var allowanceList = new List<InsightAllowance>();
		allowanceList.Add(allowance);

		advisor.Customers = userList;
		user.Advisors = advisorList;
		advisor.Allowances = allowanceList;
		advisor.Allowances = allowanceList;

		// Run:
		var result = Container.CheckExtendedInsightStatus(advisor, user.UserName);

		// Assert:
		Assert.False(result);

	}
}
