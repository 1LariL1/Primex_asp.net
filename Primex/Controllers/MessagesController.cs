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
    public class MessagesController : Controller
    {
        private readonly PrimexContext _context;

        public MessagesController(PrimexContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> MessageTable()
        {
            ViewBag.Title = "Административная панель | Заявки";
            var sessionCheck = CheckSessionAndRole();
            if (sessionCheck != null)
            {
                return sessionCheck; 
            }

            var primexContext = _context.Messages.Include(m => m.IdServiceNavigation).Include(m => m.IdUserNavigation);
            return View(await primexContext.ToListAsync());


        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.IdServiceNavigation)
                .Include(m => m.IdUserNavigation)
                .FirstOrDefaultAsync(m => m.IdMessage == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["IdService"] = new SelectList(_context.Services, "IdService", "IdService");
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "IdUser");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMessage,IdService,IdUser,Address,Comment,Time,Date")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdService"] = new SelectList(_context.Services, "IdService", "IdService", message.IdService);
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "IdUser", message.IdUser);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["IdService"] = new SelectList(_context.Services, "IdService", "IdService", message.IdService);
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "IdUser", message.IdUser);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMessage,IdService,IdUser,Address,Comment,Time,Date")] Message message)
        {
            if (id != message.IdMessage)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.IdMessage))
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
            ViewData["IdService"] = new SelectList(_context.Services, "IdService", "IdService", message.IdService);
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "IdUser", message.IdUser);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> MessageDelete(int? id)
        {
            ViewBag.Title = "Удаление заявки";
            var sessionCheck = CheckSessionAndRole();
            if (sessionCheck != null)
            {
                return sessionCheck;
            }
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.IdServiceNavigation)
                .Include(m => m.IdUserNavigation)
                .FirstOrDefaultAsync(m => m.IdMessage == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("MessageDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MessageTable));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.IdMessage == id);
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
