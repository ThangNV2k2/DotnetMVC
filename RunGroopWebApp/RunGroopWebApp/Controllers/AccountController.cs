﻿using Microsoft.AspNetCore.Identity;
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

        public IActionResult Register()
        {
            var register = new RegisterViewModel();
            return View(register);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            // check user
            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if(user != null) 
            {
                TempData["Error"] = "This email address is already use";
                return View(registerViewModel); 
            }
            var newUser = new AppUser
            {
                UserName = registerViewModel.EmailAddress,
                Email = registerViewModel.EmailAddress
            };
            var newResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);
            if(newResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                return RedirectToAction("Index", "Race");
            }
            else
            {
                TempData["Error"] = "Error";
            }
            return View(registerViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Race");
        }
    }
}
