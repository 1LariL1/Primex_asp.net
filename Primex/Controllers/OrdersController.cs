using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Primex.Models;

namespace Primex.Controllers
{
    public class OrdersController : Controller
    {
        private readonly PrimexContext _context;

        public OrdersController(PrimexContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> OrderTable()
        {
            ViewBag.Title = "Административная панель | Заказы";
            var sessionCheck = CheckSessionAndRole();
            if (sessionCheck != null)
            {
                return sessionCheck;
            }
            var primexContext = _context.Orders.Include(o => o.IdServiceNavigation).Include(o => o.IdUserNavigation);
            return View(await primexContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.IdServiceNavigation)
                .Include(o => o.IdUserNavigation)
                .FirstOrDefaultAsync(m => m.IdOrders == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult OrderCreateForm()
        {
            ViewBag.Title = "Добавление заказа";
            var sessionCheck = CheckSessionAndRole();
            if (sessionCheck != null)
            {
                return sessionCheck;
            }
            ViewData["IdService"] = new SelectList(_context.Services, "IdService", "Service1");
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Login");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdOrders,IdUser,IdService,Price,Status,Date,Time,Address")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(OrderTable));
            }
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                }
            }
            ViewData["IdService"] = new SelectList(_context.Services, "IdService", "Service1");
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Login");
            return View(order);

        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> OrderEditForm(int? id)
        {
            ViewBag.Title = "Изменение заказа";

            var sessionCheck = CheckSessionAndRole();
            if (sessionCheck != null)
            {
                return sessionCheck;
            }

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["IdService"] = new SelectList(_context.Services, "IdService", "Service1", order.IdService);
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Login", order.IdUser);
            var formattedDate = order.Date.ToString("yyyy-MM-dd");
            ViewBag.FormattedDate = formattedDate;
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("OrderEditForm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdOrders,IdUser,IdService,Price,Status,Date,Time,Address")] Order order)
        {
            if (id != order.IdOrders)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.IdOrders))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(OrderTable));
            }
            ViewData["IdService"] = new SelectList(_context.Services, "IdService", "Service1", order.IdService);
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "Login", order.IdUser);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> OrderDelete(int? id)
        {
            ViewBag.Title = "Удаление заказа";
            var sessionCheck = CheckSessionAndRole();
            if (sessionCheck != null)
            {
                return sessionCheck;
            }
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.IdServiceNavigation)
                .Include(o => o.IdUserNavigation)
                .FirstOrDefaultAsync(m => m.IdOrders == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("OrderDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OrderTable));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.IdOrders == id);
        }

        private IActionResult CheckSessionAndRole()
        {
            var IdUserString = HttpContext.Session.GetString("IdUser");

            if (IdUserString == null)
            {
                return RedirectToAction("AuthReg", "Account");
            }

            var user = _context.Users.Find(int.Parse(IdUserString));
            if (user.Access != "admin")
            {
                return RedirectToAction("AuthReg", "Account");
            }
            return null;
        }
    }
}
