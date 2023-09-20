using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.data;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationContext _context;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel) 
        {
            if(!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
            //check user
            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);
            if(user != null)
            {
                // user was found. Check Password
                var checkPassword = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if(checkPassword)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if(result.Succeeded)
                    {
                        return RedirectToAction("Index", "Race");
                    }
                }
                //Password is incorrect
                TempData["Error"] = "Password are wrong. Please, try game";
                return View(loginViewModel);
            }
            TempData["Error"] = "Email or Password are wrong. Please, try game";
            return View(loginViewModel);
        }
    }
}
