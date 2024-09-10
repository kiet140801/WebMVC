using App.Core.Commons;
using App.Core.Entities;
using App.Core.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebDemoApplication.Models.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace WebDemoApplication.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUser _currentUser;
        public UserController(IBaseRepository<User> userRepository, IPasswordHasher passwordHasher, ICurrentUser currentUser)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _currentUser = currentUser;
        }
        public async Task<IActionResult> Index()
        {
            if (!_currentUser.IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            var users = await _userRepository.GetAll().ToListAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserDto user)
        {            
            if (ModelState.IsValid)
            {
                var (hash, salt) = _passwordHasher.HashPassword(user.Password);
                await _userRepository.AddAsync(new User
                {
                    Email = user.Email,
                    HashPassword = hash,
                    SaltPassword = salt,
                    IsAdmin = user.IsAdmin
                });
                await _userRepository.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetAll().Where(x => x.Email == model.Email)
                                                .FirstOrDefaultAsync();

                if (user != null && _passwordHasher.VerifyPassword(model.Password, user.SaltPassword, user.HashPassword))
                {
                    // Set authentication cookie or session here
                    // For simplicity, we'll use TempData to simulate successful login
                    //TempData["Message"] = "Login successful!";
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("IsAdmin", user.IsAdmin.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }
    }
}
