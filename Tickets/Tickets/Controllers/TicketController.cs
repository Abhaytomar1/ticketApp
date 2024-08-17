using Microsoft.AspNetCore.Mvc;
using Tickets.Models;
using System.Threading.Tasks;
using YourNamespace.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Tickets.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ticket/Create
        public IActionResult Create()
        {
            // Populate Status options for dropdown
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });

            return View();
        }

        // POST: Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate Status options if validation fails
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });

            return View(ticket);
        }

        // GET: Ticket/Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tickets.ToListAsync());
        }

        // GET: Ticket/PriorityDistribution
        public async Task<IActionResult> PriorityDistribution()
        {
            // Fetch data from the database
            var highPriorityTickets = await _context.Tickets.Where(t => t.Priority == "High").ToListAsync();
            var mediumPriorityTickets = await _context.Tickets.Where(t => t.Priority == "Medium").ToListAsync();
            var lowPriorityTickets = await _context.Tickets.Where(t => t.Priority == "Low").ToListAsync();

            var model = new TicketPriorityViewModel
            {
                HighPriorityCount = highPriorityTickets.Count,
                MediumPriorityCount = mediumPriorityTickets.Count,
                LowPriorityCount = lowPriorityTickets.Count,
                HighPriorityTickets = highPriorityTickets,
                MediumPriorityTickets = mediumPriorityTickets,
                LowPriorityTickets = lowPriorityTickets
            };

            return View(model);
        }

        // GET: Ticket/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            // Populate Status options for dropdown
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });

            return View(ticket);
        }

        // POST: Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketNo,Subject,TicketBody,Priority,Status")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Re-populate Status options if validation fails
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });

            return View(ticket);
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        // GET: Ticket/Status
        public IActionResult Status()
        {
            return View();
        }

        // GET: Ticket/GetStatusData
        public JsonResult GetStatusData()
        {
            var statusCounts = _context.Tickets
                .GroupBy(t => t.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            return Json(statusCounts);
        }
    }
}
