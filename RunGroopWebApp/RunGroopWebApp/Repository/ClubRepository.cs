using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.data;
using RunGroopWebApp.Interface;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository
{
    public class ClubRepository : IClubRepository
    {
        private readonly ApplicationContext _context;

        public ClubRepository(ApplicationContext context) 
        {
            _context = context;
        }
        public bool Add(Club club)
        {
            _context.Add(club);
            return Save();
        }

        public bool Delete(Club club)
        {
            _context.Remove(club);
            return Save();
        }

        public async Task<Club> GetClubByIdAsync(int id)
        {
            var club = await _context.Clubs.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            return club;
        }
        public async Task<Club> GetClubByIdAsyncNoTracking(int id)
        {
            var club = await _context.Clubs.Include(c => c.Address).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return club;
        }

        public async Task<IEnumerable<Club>> GetClubs()
        {
            var clubs = await _context.Clubs.ToListAsync();
            return clubs;
        }

        public async Task<IEnumerable<Club>> GetClubsByCity(string city)
        {
            var clubs = await _context.Clubs.Where(c => c.Address.City.Contains(city)).ToListAsync();
            return clubs;
        }

        public bool Save()
        {
            var result = _context.SaveChanges();
            return result > 0 ? true : false;
        }

        public bool Update(Club club)
        {
            _context.Update(club);
            return Save();
        }
    }
}
