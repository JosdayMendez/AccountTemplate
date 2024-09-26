using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountTemplate.Controllers
{
    public class RoleController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError(string.Empty, "The role name is required.");
            }

            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleName);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    TempData["Message"] = "Role successfully created.";
                    return RedirectToAction("RoleList", "Role");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View();
        }

        public async Task<IActionResult> RoleList()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole([Bind("Id,Name")] IdentityRole role)
        {
            if (string.IsNullOrWhiteSpace(role.Name))
            {
                ModelState.AddModelError(string.Empty, "The role name is required.");
                return View(role);
            }

            if (await _roleManager.RoleExistsAsync(role.Name) && role.Name != (await _roleManager.FindByIdAsync(role.Id)).Name)
            {
                ModelState.AddModelError(string.Empty, "The role name already exists.");
                return View(role);
            }

            var existingRole = await _roleManager.FindByIdAsync(role.Id);
            if (existingRole != null)
            {
                existingRole.Name = role.Name;
                var result = await _roleManager.UpdateAsync(existingRole);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Role successfully updated.";
                    return RedirectToAction("RoleList", "Role");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The role does not exist.");
            }

            return View(role);
        }
    }
}
