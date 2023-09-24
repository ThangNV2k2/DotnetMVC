using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interface;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;

        public void mapUerEdit(AppUser user, EditUserDashboardViewModel editVM, ImageUploadResult imageUpload)
        {
            user.Id = editVM.Id;
            user.Pace = editVM.Pace;
            user.Mileage = editVM.Mileage;
            user.ProfileImageUrl = imageUpload.Url.ToString();
            user.City = editVM.City;
            user.State = editVM.State;
        }
        public DashboardController(IDashboardRepository dashboardRepository, 
            IHttpContextAccessor httpContextAccessor,
            IPhotoService photoService)
        {
            _dashboardRepository = dashboardRepository;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;
        }
        public async Task<IActionResult> Index()
        {
            var userClubs = await _dashboardRepository.GetAllUserClubs();
            var userRaces = await _dashboardRepository.GetAllUserRaces();
            var dashboardViewModel = new DashboardViewModel()
            {
                Clubs = userClubs,
                Races = userRaces,
            };
            return View(dashboardViewModel);
        }
        public async Task<IActionResult> EditUserProfile()
        {
            var curUser =  _httpContextAccessor.HttpContext.User?.GetUserId();
            var user = await _dashboardRepository.GetUserById(curUser);
            var editUserProfile = new EditUserDashboardViewModel
            {
                Id = curUser,
                Pace = user.Pace,
                Mileage = user.Mileage,
                ProfileImageUrl = user.ProfileImageUrl,
                City = user.City,
                State = user.State,
            };
            return View(editUserProfile);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel editVM)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit");
                return View("EditUserProfile", editVM);
            }
            var user = await _dashboardRepository.GetUserByIdNoTracking(editVM.Id);
            if(user.ProfileImageUrl == null || user.ProfileImageUrl == "") 
            {
                var photo = await _photoService.AddPhotoAsync(editVM.Image);
                //optimistic concurrency: tracking error
                //using no tracking
                mapUerEdit(user, editVM,photo);
                _dashboardRepository.Update(user);
                return RedirectToAction("Index");
            } 
            else
            {
                try
                {
                    await _photoService.DeletePhotoAsync(user.ProfileImageUrl);

                } 
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photograp");
                    return View(editVM);
                }
                var photo = await _photoService.AddPhotoAsync(editVM.Image);
                //optimistic concurrency: tracking error
                //using no tracking
                mapUerEdit(user, editVM, photo);
                _dashboardRepository.Update(user);
                return RedirectToAction("Index");
            }
        }
    }
}
