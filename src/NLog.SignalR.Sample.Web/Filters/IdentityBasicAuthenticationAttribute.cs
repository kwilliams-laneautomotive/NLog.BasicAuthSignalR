using System.Configuration;
using System.Drawing.Text;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Lane.WebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BasicAuthentication.Filters
{
    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {

        private static readonly string AdminKey = ConfigurationManager.AppSettings["AdminKey"];

        protected override async Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
        {
            var userManager = CreateUserManager();

            cancellationToken.ThrowIfCancellationRequested(); // Unfortunately, UserManager doesn't support CancellationTokens.

            ApplicationUser user;

            if (userName == "admin" && password == AdminKey)
            {
                await CreateAdminUser(userManager);
            }

            user = await userManager.FindAsync(userName, password);

            if (user == null)
            {
                // No user with userName/password exists.
                return null;
            }

            // Create a ClaimsIdentity with all the claims for this user.
            cancellationToken.ThrowIfCancellationRequested(); // Unfortunately, IClaimsIdenityFactory doesn't support CancellationTokens.
            ClaimsIdentity identity = await userManager.ClaimsIdentityFactory.CreateAsync(userManager, user, "Basic");
            return new ClaimsPrincipal(identity);
        }

        private static async Task CreateAdminUser(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any(x => x.UserName == "admin"))
            {
                var passwordHash = userManager.PasswordHasher.HashPassword(AdminKey);
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    PasswordHash = passwordHash
                };

                await userManager.CreateAsync(adminUser);
            }
        }

        private static UserManager<ApplicationUser> CreateUserManager()
        {
            return new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }
    }
}