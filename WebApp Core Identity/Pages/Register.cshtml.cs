using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_Core_Identity.ViewModels;

namespace WebApp_Core_Identity.Pages
{
    //Initialize the build-in ASP.NET Identity
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        [BindProperty]
        public Register RModel { get; set; }
        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        //Save data into the database
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email
                };

                //Create the Admin role if NOT exist
                IdentityRole role = await roleManager.FindByIdAsync("Admin");
                if (role == null)
                {
                    IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role admin failed");
                    }
                }

                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    //Add users to Admin Role
                    result = await userManager.AddToRoleAsync(user,"Admin");

                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return Page();
            }
            return Page();
        }
    }
}
