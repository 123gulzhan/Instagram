using System.Threading.Tasks;
using Instagram.Models;
using Microsoft.AspNetCore.Identity;

namespace Instagram.Services
{
    public class UserRole
    {
        public static async Task SetUserRole(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            var roles = new [] {"user"};
            
            foreach (var role in roles)
            {
                if (await roleManager.FindByNameAsync(role) is null)
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}