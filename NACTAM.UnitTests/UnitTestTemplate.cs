using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NACTAM.Identity.Data;
using NACTAM.Models.Repositories;
// use this instead of Moq.EntityFrameworkCore
using NACTAM.UnitTestExtension;

namespace NACTAM.UnitTests;

/// <summary>
/// template for creating unit tests, containing how to setup
/// the mocked Contexts and Databasesets
///
/// author: Tuan Bui
/// </summary>
/// <remarks>
/// You can't use any methods on either UserManager or SignInManager,
/// that use any of their inherent attributes,
/// because they are just mocked
/// </remarks>
public class UnitTestTemplate {
	public static readonly Fixture Fixture = new Fixture();

	public List<PrivatePerson> PrivatePersonsList;
	public List<TaxAdvisor> TaxAdvisorsList;
	public List<InsightAllowance> InsightsList;

	public Mock<NACTAMContext> NactamContext;
	public MockUserManager<User> MockUserManager;
	public MockUserManager<PrivatePerson> MockPrivatePersonManager;
	public MockUserManager<TaxAdvisor> MockTaxAdvisorManager;
	public MockUserManager<Admin> MockAdminManager;
	public MockSignInManager<User> MockSignInManager;

	public UserContainer UserContainer;

	public UnitTestTemplate() {
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
		Mock<IEmailSender> emailMock = new Mock<IEmailSender>();
		Mock<IPasswordValidator<User>> passwordValidator = new Mock<IPasswordValidator<User>>();
		UserContainer = new UserContainer(NactamContext.Object, MockUserManager, MockPrivatePersonManager, MockTaxAdvisorManager, MockAdminManager, (new Mock<INotificationRepository>()).Object, emailMock.Object, passwordValidator.Object);
	}

	/// <remark>
	/// not a good test, but good to display how to make a read and write operation
	/// </remark>
	/// <summary>
	/// author: Tuan Bui
	/// </summary>
	[Fact]
	public async void TestReadWrite() {
		var user1 = await MockUserManager.FindByNameAsync("Test");
		Assert.NotNull(user1);
		Assert.Equal("Jaumann", user1.LastName);
		user1.LastName = "Okmann";
		await NactamContext.Object.SaveChangesAsync();

		var updated = NactamContext.Object.PrivatePerson.First(x => x.Id == "coolid");
		Assert.Equal("Okmann", updated?.LastName);

		// don't do this, because it needs to access the IUserPasswordStore
		// await UserContainer.AddPrivatePerson(new PrivatePerson { Id = "coolid2", FirstName = "Christian2", LastName = "Jaumann2" }, "somerandomtest");
		NactamContext.Object.PrivatePerson.Add(new PrivatePerson { Id = "coolid2", FirstName = "Christian2", LastName = "Jaumann2" });
		Assert.Equal(2, NactamContext.Object.PrivatePerson.Count());
	}

	/// <summary>
	/// Manual inserts of navigation properties neccessary
	///
	/// author: Tuan Bui
	/// </summary>
	[Fact]
	public async void TestNavigationProperties() {
		var user1 = await MockTaxAdvisorManager.FindByNameAsync("TaxAdvisor");
		Assert.NotNull(user1?.Customers);
		Assert.Equal("Jaumann", user1.Customers[0].LastName);
		var user = await MockUserManager.FindByNameAsync("TaxAdvisor");
		Assert.NotNull(user);
	}

}
