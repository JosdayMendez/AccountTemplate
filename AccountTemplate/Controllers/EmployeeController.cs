using AccountTemplate.Data;
using AccountTemplate.Models;
using AccountTemplate.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountTemplate.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeController(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
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

        public async Task<IActionResult> ListProfile()
        {
            var profiles = await _context.Profiles.ToListAsync();

            var profileViewModels = profiles.Select(profile => new ProfileVM
            {
                BusinessName = profile.BusinessName,
                Phone = profile.Phone,
                PrimaryEmail = profile.PrimaryEmail,
                SecondaryEmail = profile.SecondaryEmail
            }).ToList();

            return View(profileViewModels);
        }

        public async Task<IActionResult> EditProfile(int id)
        {
            var userId = await GetUserIdAsync();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profileVM = new ProfileVM
            {
                BusinessName = profile.BusinessName,
                Phone = profile.Phone,
                PrimaryEmail = profile.PrimaryEmail,
                SecondaryEmail = profile.SecondaryEmail
            };

            return View(profileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileVM model)
        {
            if (ModelState.IsValid)
            {
                var userId = await GetUserIdAsync();
                if (userId == null)
                {
                    ModelState.AddModelError("", "Authenticated user not found.");
                    return View(model);
                }

                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
                if (profile == null)
                {
                    return RedirectToAction("Create", "Profile");
                }

                profile.BusinessName = model.BusinessName;
                profile.Phone = model.Phone;
                profile.PrimaryEmail = model.PrimaryEmail;
                profile.SecondaryEmail = model.SecondaryEmail;

                try
                {
                    _context.Entry(profile).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError("", "Database error: " + dbEx.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating profile: " + ex.Message);
                }
            }

            return View(model);
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

        [HttpGet]
        public async Task<IActionResult> AssignBranchesToUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User with email {email} was not found.");
            }

            var profile = await _context.Profiles.FindAsync(user.Id);
            if (profile == null)
            {
                return NotFound($"Profile for user with email {email} was not found.");
            }

            var branches = await _context.Branches.ToListAsync();
            var assignedBranches = await _context.ProfileBranches
                .Where(pb => pb.ProfileId == profile.Id)
                .ToListAsync();

            var viewModel = new ProfileBranchVM
            {
                UserId = email,
                Profile = profile,
                Branches = branches,
                AssignedBranches = assignedBranches,
                SelectedBranchIds = assignedBranches.Select(ab => ab.BranchId).ToArray()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignBranchesToUser(ProfileBranchVM model)
        {
            if (model.UserId == null || string.IsNullOrEmpty(model.UserId))
            {
                return BadRequest("Email is required.");
            }

            var user = await _userManager.FindByEmailAsync(model.UserId);
            if (user == null)
            {
                return NotFound($"User with email {model.UserId} was not found.");
            }

            var profile = await _context.Profiles.FindAsync(user.Id);
            if (profile == null)
            {
                return NotFound($"Profile for user with email {model.UserId} was not found.");
            }

            model.Profile = profile;

            var assignedBranches = await _context.ProfileBranches
                .Where(pb => pb.ProfileId == profile.Id)
                .ToListAsync();

            // Remove unselected branches
            foreach (var assignedBranch in assignedBranches)
            {
                if (!model.SelectedBranchIds.Contains(assignedBranch.BranchId))
                {
                    _context.ProfileBranches.Remove(assignedBranch);
                }
            }

            // Add selected branches
            foreach (var selectedBranchId in model.SelectedBranchIds)
            {
                if (!assignedBranches.Any(ab => ab.BranchId == selectedBranchId))
                {
                    var newAssignedBranch = new ProfileBranch
                    {
                        ProfileId = profile.Id,
                        BranchId = selectedBranchId
                    };
                    _context.ProfileBranches.Add(newAssignedBranch);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
