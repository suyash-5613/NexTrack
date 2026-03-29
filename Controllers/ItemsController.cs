using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexTrack.Data;
using NexTrack.Models;

namespace NexTrack.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if user is logged in
        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetString("UserEmail") != null;
        }

        private IActionResult RedirectIfNotAuthenticated()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");
            return null!;
        }

        // ─── DASHBOARD (Item Management) ──────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? status)
        {
            var authRedirect = RedirectIfNotAuthenticated();
            if (authRedirect != null) return authRedirect;

            var query = _context.Items.AsQueryable();

            // By default show only root items; if search is active, show all
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(i => i.Name.Contains(search));
            }
            else
            {
                query = query.Where(i => i.ParentId == null);
            }

            // Status filter
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(i => i.Status == status);
            }

            var items = await query.OrderByDescending(i => i.CreatedAt).ToListAsync();

            ViewBag.Search = search;
            ViewBag.Status = status ?? "all";
            return View(items);
        }

        // ─── CREATE ITEM ──────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, decimal weight)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Item name is required.";
                return RedirectToAction("Index");
            }

            if (weight <= 0)
            {
                TempData["Error"] = "Weight must be greater than 0.";
                return RedirectToAction("Index");
            }

            var item = new Item
            {
                Name = name.Trim(),
                Weight = weight,
                ParentId = null,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Item created successfully!";
            return RedirectToAction("Index");
        }

        // ─── EDIT ITEM ────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, decimal weight)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Item name is required.";
                return RedirectToAction("Index");
            }

            item.Name = name.Trim();
            item.Weight = weight;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Item updated successfully!";
            return RedirectToAction("Index");
        }

        // ─── DELETE ITEM ──────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            // Recursively delete all children
            await DeleteItemAndChildren(id);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Item deleted successfully!";
            return RedirectToAction("Index");
        }

        private async Task DeleteItemAndChildren(int itemId)
        {
            var children = await _context.Items.Where(i => i.ParentId == itemId).ToListAsync();
            foreach (var child in children)
            {
                await DeleteItemAndChildren(child.Id);
            }

            var item = await _context.Items.FindAsync(itemId);
            if (item != null)
            {
                _context.Items.Remove(item);
            }
        }

        // ─── PROCESS ITEM ─────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Process()
        {
            var authRedirect = RedirectIfNotAuthenticated();
            if (authRedirect != null) return authRedirect;

            var model = new ProcessItemViewModel
            {
                PendingItems = await _context.Items
                    .Where(i => i.ParentId == null && i.Status == "pending")
                    .OrderBy(i => i.Name)
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(ProcessItemViewModel model)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            // Reload pending items for dropdown
            model.PendingItems = await _context.Items
                .Where(i => i.ParentId == null && i.Status == "pending")
                .OrderBy(i => i.Name)
                .ToListAsync();

            // Remove validation errors for PendingItems (it's not submitted)
            ModelState.Remove("PendingItems");

            // Remove validation for empty child entries that were removed via JS
            model.Children = model.Children?
                .Where(c => !string.IsNullOrWhiteSpace(c.Name) || c.Weight > 0)
                .ToList() ?? new List<ChildItemInput>();

            if (model.Children.Count == 0)
            {
                ModelState.AddModelError("", "At least one child item is required.");
                return View(model);
            }

            // Validate children
            bool hasErrors = false;
            for (int i = 0; i < model.Children.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(model.Children[i].Name))
                {
                    ModelState.AddModelError($"Children[{i}].Name", "Name is required.");
                    hasErrors = true;
                }
                if (model.Children[i].Weight <= 0)
                {
                    ModelState.AddModelError($"Children[{i}].Weight", "Valid weight required.");
                    hasErrors = true;
                }
            }

            if (model.ParentId == 0)
            {
                ModelState.AddModelError("ParentId", "Please select a parent item.");
                hasErrors = true;
            }

            if (hasErrors)
            {
                return View(model);
            }

            // Mark parent as processed
            var parent = await _context.Items.FindAsync(model.ParentId);
            if (parent == null)
            {
                ModelState.AddModelError("ParentId", "Selected item not found.");
                return View(model);
            }

            parent.Status = "processed";

            // Create children
            foreach (var child in model.Children)
            {
                _context.Items.Add(new Item
                {
                    Name = child.Name.Trim(),
                    Weight = child.Weight,
                    ParentId = parent.Id,
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Item processed successfully! The parent has been marked as processed and child items have been created.";
            return RedirectToAction("Process");
        }

        // ─── TREE STRUCTURE ───────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Tree()
        {
            var authRedirect = RedirectIfNotAuthenticated();
            if (authRedirect != null) return authRedirect;

            var allItems = await _context.Items.OrderBy(i => i.Name).ToListAsync();
            return View(allItems);
        }

        // ─── PROCESSED ITEMS ──────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Processed()
        {
            var authRedirect = RedirectIfNotAuthenticated();
            if (authRedirect != null) return authRedirect;

            var processedParents = await _context.Items
                .Where(i => i.ParentId == null && i.Status == "processed")
                .Include(i => i.Children)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return View(processedParents);
        }
    }
}
