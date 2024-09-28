using Microsoft.AspNetCore.Mvc;
using AccountTemplate.Models;
using AccountTemplate.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AccountTemplate.Data;
using System.Threading.Tasks;
using System.Linq;

namespace AccountTemplate.Controllers
{
    [Authorize]
    public class BranchController : Controller
    {
        private readonly AppDbContext _context;

        public BranchController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var branches = _context.Branches.ToList();
            return View(branches);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BranchVM model)
        {
            if (ModelState.IsValid)
            {
                var branch = new Branch
                {
                    BranchName = model.BranchName,
                    Address = model.Address,
                    Phone = model.Phone,
                    WhatsApp = model.WhatsApp,
                    GoogleMapsLink = model.GoogleMapsLink,
                    Email = model.Email,
                    Description = model.Description,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            var branchVM = new BranchVM
            {
                Id = branch.Id,
                BranchName = branch.BranchName,
                Address = branch.Address,
                Phone = branch.Phone,
                WhatsApp = branch.WhatsApp,
                GoogleMapsLink = branch.GoogleMapsLink,
                Email = branch.Email,
                Description = branch.Description
            };

            return View(branchVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BranchVM model)
        {
            if (ModelState.IsValid)
            {
                var branch = await _context.Branches.FindAsync(model.Id);
                if (branch == null)
                {
                    return NotFound();
                }

                branch.BranchName = model.BranchName;
                branch.Address = model.Address;
                branch.Phone = model.Phone;
                branch.WhatsApp = model.WhatsApp;
                branch.GoogleMapsLink = model.GoogleMapsLink;
                branch.Email = model.Email;
                branch.Description = model.Description;

                _context.Branches.Update(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
