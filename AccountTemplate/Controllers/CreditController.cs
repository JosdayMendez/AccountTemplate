using AccountTemplate.Data;
using AccountTemplate.Models;
using AccountTemplate.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccountTemplate.Controllers
{
    public class CreditController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CreditController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<string> GetUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id;
        }

        public IActionResult Create()
        {
            var users = _context.Users.ToList();
            ViewBag.Users = users;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("You need to be logged in to view your credits.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var credit = await _context.Credits
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            var userCredits = new UserCreditVM
            {
                UserId = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Balance = credit?.Balance ?? 0,
                LastUpdated = credit?.LastUpdated ?? DateTime.MinValue
            };

            return View(userCredits);
        }

        public async Task<IActionResult> ListCredit()
        {
            var users = await _userManager.Users.ToListAsync();
            var viewModel = new List<UserCreditVM>();

            foreach (var user in users)
            {
                var credit = await _context.Credits
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                var userVM = new UserCreditVM
                {
                    UserId = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Balance = credit?.Balance ?? 0,
                    LastUpdated = credit?.LastUpdated ?? DateTime.MinValue
                };

                viewModel.Add(userVM);
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ManageCredit(string email)
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

            var credit = await _context.Credits.FirstOrDefaultAsync(c => c.UserId == user.Id);

            var model = new CreditVM
            {
                UserId = user.Id,
                Balance = credit?.Balance ?? 0,
                LastUpdated = credit?.LastUpdated ?? DateTime.MinValue
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageCredit(CreditVM model)
        {
            if (string.IsNullOrEmpty(model.UserId))
            {
                return BadRequest("User ID is required.");
            }

            if (model.PurchaseAmount <= 10)
            {
                ModelState.AddModelError(nameof(model.PurchaseAmount), "You must enter a valid amount to purchase.");
                return View(model); 
            }


            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {model.UserId} was not found.");
            }

            var credit = await _context.Credits.FirstOrDefaultAsync(c => c.UserId == model.UserId);
            if (credit == null)
            {
                credit = new Credit
                {
                    UserId = model.UserId,
                    Balance = model.Balance + model.PurchaseAmount,
                    LastUpdated = DateTime.Now
                };
                _context.Credits.Add(credit);
            }
            else
            {
                credit.Balance += model.PurchaseAmount - model.SpendAmount;
                credit.LastUpdated = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult BuySubscription()
        {
            var subscriptions = new List<SubscriptionVM>
            {
                new SubscriptionVM { Id = 1, Name = "Basic Plan", Cost = 100 },
                new SubscriptionVM { Id = 2, Name = "Premium Plan", Cost = 250 },
                new SubscriptionVM { Id = 3, Name = "VIP Plan", Cost = 500 }
            };

            return View(subscriptions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuySubscription(int subscriptionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("You need to be logged in to purchase a subscription.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var credit = await _context.Credits.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (credit == null)
            {
                TempData["ErrorMessage"] = "You have no credits available.";
                return RedirectToAction("BuySubscription");
            }

            var subscriptionCost = subscriptionId switch
            {
                1 => 100,  
                2 => 250, 
                3 => 500, 
                _ => 0    
            };

            if (subscriptionCost == 0)
            {
                TempData["ErrorMessage"] = "Invalid subscription selected.";
                return RedirectToAction("BuySubscription");
            }

            if (credit.Balance < subscriptionCost)
            {
                TempData["ErrorMessage"] = "Not enough credits to purchase this subscription.";
                return RedirectToAction("BuySubscription");
            }

            credit.Balance -= subscriptionCost;
            credit.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"You successfully purchased the subscription for {subscriptionCost} credits.";
            return RedirectToAction("Index");
        }
    }
}
