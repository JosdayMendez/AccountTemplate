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

        public async Task<IActionResult> ListUsers()
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
        public async Task<IActionResult> AssignRole(string email)
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
        public async Task<IActionResult> AssignRole(AssignUserRoleVM model)
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
                return RedirectToAction("ListUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> AssignBranches()
        {
            var userId = await GetUserIdAsync();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Create", "Profile");
            }

            var branches = await _context.Branches.ToListAsync();
            var assignedBranches = await _context.UserBranches
                .Where(pb => pb.UserId == userId)
                .Include(pb => pb.Branch)
                .ToListAsync();

            var viewModel = new UserBranchVM
            {
                User = user,
                Branches = branches,
                AssignedBranches = assignedBranches
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignBranches(UserBranchVM model)
        {
            var userId = await GetUserIdAsync();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Create", "Profile");
            }

            if (model.SelectedBranchIds == null || model.SelectedBranchIds.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select at least one branch.");

                var branches = await _context.Branches.ToListAsync();
                var assignedBranches = await _context.UserBranches
                    .Where(pb => pb.UserId == userId)
                    .Include(pb => pb.Branch)
                    .ToListAsync();

                model.Branches = branches;
                model.AssignedBranches = assignedBranches;

                return View(model);
            }

            var selectedBranchIds = model.SelectedBranchIds;
            var assignedBranchesToUpdate = await _context.UserBranches
                .Where(pb => pb.UserId == userId)
                .ToListAsync();

            foreach (var assignedBranch in assignedBranchesToUpdate)
            {
                if (!selectedBranchIds.Any(id => id == assignedBranch.BranchId))
                {
                    _context.UserBranches.Remove(assignedBranch);
                }
            }

            foreach (var selectedBranchId in selectedBranchIds)
            {
                if (!assignedBranchesToUpdate.Any(pb => pb.BranchId == selectedBranchId))
                {
                    var userBranch = new UserBranch
                    {
                        UserId = userId,
                        BranchId = selectedBranchId
                    };
                    _context.UserBranches.Add(userBranch);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("AssignBranches");
        }
    }
}
