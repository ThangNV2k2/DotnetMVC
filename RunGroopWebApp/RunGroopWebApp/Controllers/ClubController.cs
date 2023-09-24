using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.data;
using RunGroopWebApp.Interface;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var clubs = await _clubRepository.GetClubs();
            return View(clubs);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var club = await _clubRepository.GetClubByIdAsync(id);
            return View(club);
        }
        public IActionResult Create()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var createClubViewModel = new CreateClubViewModel { AppUserId = curUserId };
            return View(createClubViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubViewModel)
        {
            var photo = await _photoService.AddPhotoAsync(clubViewModel.Image);
            var club = new Club
            {
                Title = clubViewModel.Title,
                Description = clubViewModel.Description,
                Image = photo.Url.ToString(),
                ClubCategory = clubViewModel.ClubCategory,
                AppUserId = clubViewModel.AppUserId,
                Address = new Address
                {
                    Street = clubViewModel.Address.Street,
                    City = clubViewModel.Address.City,
                    State = clubViewModel.Address.State
                }
            };
            _clubRepository.Add(club);
            return RedirectToAction("Index");
            //return View(clubViewModel);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetClubByIdAsync(id);
            if (club == null)
                return View("Error");
            var clubModel = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                Address = club.Address,
                ClubCategory = club.ClubCategory,
                Url = club.Image
            };
            return View(clubModel);
        }
        /*public IActionResult Create(int id)
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubViewModel)
        {
            if (ModelState.IsValid)
            {
                var photo = await _photoService.AddPhotoAsync(clubViewModel.Image);
                var club = new Club
                {
                    Title = clubViewModel.Title,
                    Description = clubViewModel.Description,
                    Image = photo.Url.ToString(),
                    ClubCategory = clubViewModel.ClubCategory,
                    Address = new Address
                    {
                        Street = clubViewModel.Address.Street,
                        City = clubViewModel.Address.City,
                        State = clubViewModel.Address.State
                    }
                };

                _clubRepository.Add(club);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }
            return View(clubViewModel);
        }*/
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club");
                return View(clubViewModel);
            }
            var userClubId = await _clubRepository.GetClubByIdAsyncNoTracking(id);
            if (userClubId != null)
            {
                try
                {
                    var deletePhoto = await _photoService.DeletePhotoAsync(userClubId.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "you couldn't delete photot");
                    return View(clubViewModel);
                }
                var photo = await _photoService.AddPhotoAsync(clubViewModel.Image);
                var club = new Club
                {
                    Id = clubViewModel.Id,
                    Title = clubViewModel.Title,
                    Description = clubViewModel.Description,
                    AddressId = clubViewModel.AddressId,
                    Address = new Address
                    {
                        Street = clubViewModel.Address.Street,
                        City = clubViewModel.Address.City,
                        State = clubViewModel.Address.State,
                    },
                    Image = photo.Url.ToString(),
                    ClubCategory = clubViewModel.ClubCategory
                };
                _clubRepository.Update(club);
                return RedirectToAction("Index");
            }
            else
            {
                return View(clubViewModel);
            }
        }
        public async Task<IActionResult> Delete (int id)
        {
            var club = await _clubRepository.GetClubByIdAsync(id);
            if(club == null)
                return View("Error");
            return View(club);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteClub (int id)
        {
            var club = await _clubRepository.GetClubByIdAsync(id);
            if (club == null)
                return View("Error");
            _clubRepository.Delete(club);
            return RedirectToAction("Index");
        }
    }
}