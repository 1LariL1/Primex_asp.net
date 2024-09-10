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
    public class ServicesController : Controller
    {
        private readonly PrimexContext _context;

        public ServicesController(PrimexContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Services
        public async Task<IActionResult> ServiceTable()
        {
            ViewBag.Title = "Административная панель | Услуги";
            var sessionCheck = CheckSessionAndRole();
            if (sessionCheck != null)
            {
                return sessionCheck;
            }
            var services = await _context.Services.ToListAsync();
            return View(services);
        }



        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(m => m.IdService == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: Services/Create
        public IActionResult ServiceCreateForm()
        {
            ViewBag.Title = "Добавление услуги";
            var sessionCheck = CheckSessionAndRole();
            if (sessionCheck != null)
            {
                return sessionCheck;
            }
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdService,Service1,Price,Description")] Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ServiceTable));
            }
            return View(service);
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> ServiceEditForm(int? id)
        {
            ViewBag.Title = "Изменение услуги";
            var sessionCheck = CheckSessionAndRole();
            if (sessionCheck != null)
            {
                return sessionCheck;
            }
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("ServiceEditForm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdService,Service1,Price,Description")] Service service)
        {
            if (id != service.IdService)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.IdService))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ServiceTable));
            }
            return View(service);
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> ServiceDelete(int? id)
        {
            ViewBag.Title = "Удаление услуги";
            var sessionCheck = CheckSessionAndRole();
            if (sessionCheck != null)
            {
                return sessionCheck;
            }
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(m => m.IdService == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("ServiceDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ServiceTable));
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.IdService == id);
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
