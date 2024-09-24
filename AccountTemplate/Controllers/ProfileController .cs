using AccountTemplate.Data;
using AccountTemplate.Models;
using AccountTemplate.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index()
        {
            var userId = await GetUserIdAsync();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                profile = new Profile
                {
                    UserId = userId,
                    BusinessName = string.Empty, 
                    Phone = string.Empty,
                    PrimaryEmail = string.Empty, 
                    SecondaryEmail = string.Empty 
                };

                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();
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
                    return RedirectToAction("Create", "Profile"); 
                }

        
                profile.BusinessName = model.BusinessName;
                profile.Phone = model.Phone;
                profile.PrimaryEmail = model.PrimaryEmail;
                profile.SecondaryEmail = model.SecondaryEmail;

                try
                {
       
                    _context.Profiles.Update(profile);
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

    }
}
