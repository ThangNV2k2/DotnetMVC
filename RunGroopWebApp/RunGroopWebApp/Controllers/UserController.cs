using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interface;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet("User")]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUser();
            List<UserViewModel> result = new List<UserViewModel>();
            foreach (var user in users)
            {
                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Pace = user.Pace,
                    Mileage = user.Mileage,
                    ProfileImageUrl = user.ProfileImageUrl,
                };
                result.Add(userViewModel);
            }
            return View(result);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userRepository.GetById(id);
            var result = new UserDetailViewModel
            {
                Id = user.Id,
                Username= user.UserName,
                Pace= user.Pace,
                Mileage = user.Mileage,
                ProfileImageUrl = user.ProfileImageUrl
            };
            return View(result);
        }
    }
}
