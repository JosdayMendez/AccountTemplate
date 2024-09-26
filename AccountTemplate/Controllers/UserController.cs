using AccountTemplate.Data;
using AccountTemplate.Models;
using AccountTemplate.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountTemplate.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private async Task<string> GetUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id;
        }


        public async Task<IActionResult> ListUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [Route("EditUser/{id}")]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new RegisterVM
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AgreeToTerms = true
            };

            return View(model);
        }

        public async Task<IActionResult> ListUsersRoles()
        {
            var users = await _userManager.Users.ToListAsync();

            var viewModel = new List<UserRolesVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userVM = new UserRolesVM
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Roles = roles.ToList()
                };

                viewModel.Add(userVM);
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AssignRoleToUser(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User with email {email} was not found.");
            }

            var model = new AssignUserRoleVM
            {
                Email = user.Email,
                Roles = _roleManager.Roles.Select(r => r.Name).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRoleToUser(AssignUserRoleVM model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email is required.");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound($"User with email {model.Email} was not found.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.SelectedRole);
            if (!roleExists)
            {
                ModelState.AddModelError("", "Role does not exist.");
                return View(model);
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, userRoles);
            }

            var result = await _userManager.AddToRoleAsync(user, model.SelectedRole);
            if (result.Succeeded)
            {
                return RedirectToAction("ListUsersRoles"); 
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        
    }
}
