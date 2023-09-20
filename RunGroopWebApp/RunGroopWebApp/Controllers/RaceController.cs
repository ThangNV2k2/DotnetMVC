using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.data;
using RunGroopWebApp.Interface;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;

        public RaceController(IRaceRepository raceRepository, IPhotoService photoService)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
        }
        public async Task<IActionResult> Index()
        {
            var races = await _raceRepository.GetRaces();
            return View(races);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var race = _raceRepository.GetRaceByIdAsync(id);
            return View(race);
        }
        public IActionResult Create(int id)
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceView)
        {
            if (ModelState.IsValid)
            {
                var photo = await _photoService.AddPhotoAsync(raceView.Image);
                var race = new Race
                {
                    Title = raceView.Title,
                    Description = raceView.Description,
                    Image = photo.Url.ToString(),
                    Address = new Address
                    {
                        Street = raceView.Address.Street,
                        City = raceView.Address.City,
                        State = raceView.Address.State
                    },
                    RaceCategory = raceView.RaceCategory
                };
                _raceRepository.Add(race);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }
            return View(raceView);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetRaceByIdAsync(id);
            if (race == null)
                return View("Error");
            var raceModel = new EditRaceViewModel
            {
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                Address = race.Address,
                RaceCategory = race.RaceCategory,
                Url = race.Image
            };
            return View(raceModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit race");
                return View(raceViewModel);
            }
            var userRaceId = await _raceRepository.GetRaceByIdAsyncNoTracking(id);
            if (userRaceId != null)
            {
                try
                {
                    var deletePhoto = await _photoService.DeletePhotoAsync(userRaceId.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "you couldn't delete photot");
                    return View(raceViewModel);
                }
                var photo = await _photoService.AddPhotoAsync(raceViewModel.Image);
                var race = new Race
                {
                    Id = raceViewModel.Id,
                    Title = raceViewModel.Title,
                    Description = raceViewModel.Description,
                    AddressId = raceViewModel.AddressId,
                    Address = new Address
                    {
                        Street = raceViewModel.Address.Street,
                        City = raceViewModel.Address.City,
                        State = raceViewModel.Address.State,
                    },
                    Image = photo.Url.ToString(),
                    RaceCategory = raceViewModel.RaceCategory,
                };
                _raceRepository.Update(race);
                return RedirectToAction("Index");
            }
            else
            {
                return View(raceViewModel);
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var race = await _raceRepository.GetRaceByIdAsync(id);
            if (race == null)
                return View("Error");
            return View(race);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteRace(int id)
        {
            var race = await _raceRepository.GetRaceByIdAsync(id);
            if (race == null)
                return View("Error");
            _raceRepository.Delete(race);
            return RedirectToAction("Index");
        }
    }
}
