using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NACTAM.UnitTests;

public class MockSignInManager<T> : SignInManager<T> where T : IdentityUser {
	public MockSignInManager(MockUserManager<T> userManager)
		: base(userManager,
			new HttpContextAccessor(),
			new Mock<IUserClaimsPrincipalFactory<T>>().Object,
			new Mock<IOptions<IdentityOptions>>().Object,
			new Mock<ILogger<SignInManager<T>>>().Object,
			new Mock<IAuthenticationSchemeProvider>().Object,
			new Mock<IUserConfirmation<T>>().Object) { }
}



