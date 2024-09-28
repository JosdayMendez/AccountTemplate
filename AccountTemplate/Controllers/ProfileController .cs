using AccountTemplate.Data;
using AccountTemplate.Models;
using AccountTemplate.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AccountTemplate.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<string> GetUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id;
        }

        public async Task<IActionResult> ListProfile()
        {
            var profiles = await _context.Profiles.ToListAsync();

            var profileVMs = profiles.Select(p => new ProfileVM
            {
                UserName = p.UserName,
                Phone = p.Phone,
                PrimaryEmail = p.PrimaryEmail,
                SecondaryEmail = p.SecondaryEmail,
                Role = p.Role,
                BusinessName = p.BusinessName
            }).ToList();

            return View(profileVMs);
        }

        public async Task<IActionResult> Index(string selectedBranch = null)
        {
            var userId = await GetUserIdAsync();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            var appUser = await _context.Users.FindAsync(userId);

            var userBranches = await _context.UserBranches
                .Where(ub => ub.UserId == userId)
                .Include(ub => ub.Branch)
                .ToListAsync();

            var branchNames = userBranches.Select(ub => ub.Branch.BranchName).ToList();

            var businessName = selectedBranch ?? branchNames.FirstOrDefault() ?? "N/A";

            var roles = await _userManager.GetRolesAsync(appUser);
            var userRole = roles.FirstOrDefault();

            if (profile == null)
            {
                profile = new Profile
                {
                    UserId = userId,
                    BusinessName = businessName,
                    Phone = profile?.Phone ?? appUser?.PhoneNumber ?? "88888888",
                    PrimaryEmail = profile?.PrimaryEmail ?? appUser?.Email ?? "example@gmail.com",
                    SecondaryEmail = profile?.SecondaryEmail ?? "example@gmail.com",
                    UserName = profile?.UserName ?? appUser?.UserName ?? "UserName",
                    Role = userRole ?? "N/A",
                };

                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            var profileVM = new ProfileVM
            {
                BusinessName = businessName,
                Phone = profile?.Phone ?? appUser?.PhoneNumber ?? "N/A",
                PrimaryEmail = profile?.PrimaryEmail ?? appUser?.Email ?? "N/A",
                SecondaryEmail = profile?.SecondaryEmail ?? "N/A",
                UserName = profile?.UserName ?? appUser?.UserName ?? "N/A",
                Role = userRole ?? "N/A",
                Branches = branchNames,
                SelectedBranch = selectedBranch ?? branchNames.FirstOrDefault()
            };

            return View(profileVM);
        }

        public async Task<IActionResult> Edit()
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

            var appUser = await _context.Users.FindAsync(userId);
            var roles = await _userManager.GetRolesAsync(appUser);
            var userRole = roles.FirstOrDefault() ?? "N/A";

            var userBranches = await _context.UserBranches
                .Where(ub => ub.UserId == userId)
                .Include(ub => ub.Branch)
                .ToListAsync();

            var branchNames = userBranches.Select(ub => ub.Branch.BranchName).ToList();
            var combinedBranches = string.Join(", ", branchNames);

            var profileVM = new ProfileVM
            {
                BusinessName = profile.BusinessName,
                Phone = profile.Phone,
                PrimaryEmail = profile.PrimaryEmail,
                SecondaryEmail = profile.SecondaryEmail,
                UserName = profile.UserName,
                Role = userRole,
                Branches = branchNames
            };

            return View(profileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileVM model)
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
                    ModelState.AddModelError("", "Profile not found.");
                    return View(model);
                }

                profile.UserName = model.UserName;
                profile.BusinessName = model.BusinessName;
                profile.Phone = model.Phone;
                profile.PrimaryEmail = model.PrimaryEmail;
                profile.SecondaryEmail = model.SecondaryEmail;

                try
                {
                    _context.Entry(profile).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError("", "Database error: " + dbEx.Message);
                    TempData["ErrorMessage"] = "Error updating profile: " + dbEx.Message;
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating profile: " + ex.Message);
                    TempData["ErrorMessage"] = "Error updating profile: " + ex.Message;
                    return View(model);
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
                {
                    TempData["ErrorMessage"] += error.ErrorMessage + Environment.NewLine;
                }
                return View(model);
            }
        }
    }
}
