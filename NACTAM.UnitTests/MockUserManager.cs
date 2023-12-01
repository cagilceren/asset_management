using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NACTAM.UnitTests;

public class MockUserManager<T> : UserManager<T> where T : IdentityUser {
	public List<T> Users;

	public MockUserManager(List<T> users)
		: base(new Mock<IUserStore<T>>().Object,
			new Mock<IOptions<IdentityOptions>>().Object,
			new Mock<IPasswordHasher<T>>().Object,
			new IUserValidator<T>[0],
			new IPasswordValidator<T>[0],
			new Mock<ILookupNormalizer>().Object,
			new Mock<IdentityErrorDescriber>().Object,
			new Mock<IServiceProvider>().Object,
			new Mock<ILogger<UserManager<T>>>().Object) { Users = users; }

	public async System.Threading.Tasks.Task<Microsoft.AspNetCore.Identity.IdentityResult> CreateAsync(T user) {
		Users.Add(user);
		return IdentityResult.Success;
	}

	public async System.Threading.Tasks.Task<Microsoft.AspNetCore.Identity.IdentityResult> DeleteAsync(T user) {
		return IdentityResult.Success;
	}

	public async System.Threading.Tasks.Task<Microsoft.AspNetCore.Identity.IdentityResult> UpdateAsync(T user) {
		return IdentityResult.Success;
	}

	public async Task<T> FindByNameAsync(string name)
		=> Users.First(x => x.UserName == name);

	public async Task<T> FindByIdAsync(string id)
		=> Users.First(x => x.Id == id);
}



