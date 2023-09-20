using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.data;
using RunGroopWebApp.Interface;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ApplicationContext _context;

        public RaceRepository(ApplicationContext context)
        {
            _context = context;
        }
        public bool Add(Race race)
        {
            _context.Add(race);
            return Save();
        }

        public bool Delete(Race race)
        {
            _context.Remove(race);
            return Save();
        }

        public async Task<Race> GetRaceByIdAsync(int id)
        {
            var race = await _context.Races.Include(r => r.Address).FirstOrDefaultAsync(r => r.Id == id);
            return race;
        }
        public async Task<Race> GetRaceByIdAsyncNoTracking(int id)
        {
            var race = await _context.Races.Include(r => r.Address).AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            return race;
        }

        public async Task<IEnumerable<Race>> GetRaces()
        {
            var races = await _context.Races.ToListAsync();
            return races;
        }

        public async Task<IEnumerable<Race>> GetRacesByCity(string city)
        {
            var races = await _context.Races.Where(r => r.Address.City.Contains(city)).ToListAsync();
            return races;
        }

        public bool Save()
        {
            var result = _context.SaveChanges();
            return result > 0 ? true : false;
        }

        public bool Update(Race race)
        {
            _context.Update(race);
            return Save();
        }
    }
}
