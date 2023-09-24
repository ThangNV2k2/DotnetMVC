using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interface
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUser();
        Task<AppUser> GetById(string id);
        bool add(AppUser user);
        bool update(AppUser user);
        bool delete(AppUser user);
        bool Save();
    }
}
