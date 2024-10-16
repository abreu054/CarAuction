using CarAuction.Business.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Application.App.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersApiController(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IWebHostEnvironment webHostEnvironment) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        [HttpGet("create/default/{secret}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateDefaultUserAsync([FromRoute] string secret)
        {
            await CreateUserRoles();
            return Ok();
        }

        private async Task CreateUserRoles()
        {
            try
            {
                IdentityResult roleResult;

                string[] sRoles =
                [
                    Roles.BackOfficeUser.ToString(),
                    Roles.BackOfficeAdmin.ToString(),
                    Roles.AuctionAdmin.ToString(),
                    Roles.AuctionUser.ToString(),
                ];

                foreach (string sRole in sRoles)
                {
                    var roleCheck = await _roleManager.RoleExistsAsync(sRole);
                    if (!roleCheck)
                    {
                        roleResult = await _roleManager.CreateAsync(new IdentityRole(sRole));
                    }
                }

                await CreateDefaultUser(_userManager, _webHostEnvironment);
            }
            catch { }
        }

        private static async Task CreateDefaultUser(UserManager<IdentityUser> userManager, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment()) return;

            var adminUser = await userManager.FindByEmailAsync("adm@mailinator.com");

            if (adminUser != null) return;

            adminUser = new IdentityUser(userName: "adm@mailinator.com");
            adminUser.Email = "adm@mailinator.com";
            adminUser.EmailConfirmed = true;

            var result = await userManager.CreateAsync(adminUser, "1qazZAQ!");

            if (!result.Succeeded) return;

            await userManager.AddToRoleAsync(adminUser, Roles.BackOfficeAdmin.ToString());
        }
    }
}