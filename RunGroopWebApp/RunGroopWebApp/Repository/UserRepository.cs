using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.data;
using RunGroopWebApp.Interface;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }
        public bool add(AppUser user)
        {
            _context.Add(user);
            return Save();
        }

        public bool delete(AppUser user)
        {
            _context.Remove(user);
            return Save();
        }

        public async Task<IEnumerable<AppUser>> GetAllUser()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<AppUser> GetById(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public bool Save()
        {
            var result =  _context.SaveChanges();
            return result > 0 ? true : false;
        }

        public bool update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
    }
}
