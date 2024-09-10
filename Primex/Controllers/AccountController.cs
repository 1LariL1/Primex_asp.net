using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Primex.Models;

namespace Primex.Controllers
{
    public class AccountController : Controller
    {
        private readonly PrimexContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(PrimexContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context; 
            _passwordHasher = passwordHasher;
        }

        public IActionResult AuthReg()
        {
            return View();
        }
        public async Task<IActionResult> Reg(User user)
        {
            if (ModelState.IsValid)
            {
                var EmailExistance = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (EmailExistance != null)
                {
                    ModelState.AddModelError("Email", "Эта почта уже зарегистрирована");
                    return View("AuthReg");
                }
                else
                {
                    user.Password = _passwordHasher.HashPassword(user, user.Password);
                    user.FullName = "";
                    user.Telephone = "";
                    user.Access = "user";
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(AuthReg));
                }

            }
            return View();
        }

        public async Task<IActionResult> Account()
        {
            ViewBag.Title = "Личный кабинет";
            var IdUserString = HttpContext.Session.GetString("IdUser");
            if (IdUserString == null)
            {
                return RedirectToAction("AuthReg", "Account");
            }
            int IdUser = int.Parse(IdUserString);
            var user = await _context.Users
                .Include(u => u.Orders)
                    .ThenInclude(o => o.IdServiceNavigation)
                .FirstOrDefaultAsync(u => u.IdUser == IdUser);
            var ViewModel = new AccountModel
            {
                User = user,
            };
            ViewData["IdService"] = new SelectList(_context.Services, "IdService", "Service1");
            return View(ViewModel);
        }

        public async Task<IActionResult> AccountUpdateInfo()
        {
            ViewBag.Title = "Изменение данных";
            var IdUserString = HttpContext.Session.GetString("IdUser");
            if (IdUserString == null)
            {
                return RedirectToAction("AuthReg", "Account");
            }

            var user = await _context.Users.FindAsync(int.Parse(IdUserString));
            return View(user);
        }

        [HttpPost, ActionName("AccountUpdateInfo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User model, string oldPassword, string newPassword)
        {
            var IdUserString = HttpContext.Session.GetString("IdUser");
            if (string.IsNullOrEmpty(IdUserString))
            {
                return RedirectToAction("AuthReg", "Account");
            }
            int IdUser = int.Parse(IdUserString);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == IdUser);
            if (user == null)
            {
                return NotFound();
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, oldPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Неверный пароль");
                return View("AccountUpdateInfo", model);
            }

            user.Login = model.Login;
            user.Email = model.Email;
            user.FullName = model.FullName;
            user.Telephone = model.Telephone;

            if (!string.IsNullOrEmpty(newPassword)) 
            {
                user.Password = _passwordHasher.HashPassword(user, newPassword);
            }

            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Account));
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
                if (result == PasswordVerificationResult.Success)
                {
                    if (user.Access == "user")
                    {
                        HttpContext.Session.SetString("IdUser", user.IdUser.ToString());
                        return RedirectToAction(nameof(Account));
                    }
                    else if (user.Access == "admin")
                    {
                        HttpContext.Session.SetString("IdUser", user.IdUser.ToString());
                        return RedirectToAction("UserTable", "Users");
                    }
                    else 
                    {
                        return RedirectToAction("AuthReg", "Account");
                    }

                }
            }
            ModelState.AddModelError("", "Неверная почта или пароль");
            return View("AuthReg");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IdUser");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMessage([Bind("IdMessage,IdService,IdUser,Address,Comment,Time,Date")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Account));
            }
            ViewData["IdService"] = new SelectList(_context.Services, "IdService", "IdService", message.IdService);
            return View(message);
        }

    }
}
