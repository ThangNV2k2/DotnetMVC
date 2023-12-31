﻿using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interface
{
    public interface IRaceRepository
    {
        Task<IEnumerable<Race>> GetRaces();
        Task<Race> GetRaceByIdAsync(int id);
        Task<Race> GetRaceByIdAsyncNoTracking(int id);
        Task<IEnumerable<Race>> GetRacesByCity(string city);

        bool Add(Race race);
        bool Update(Race race);
        bool Delete(Race race);
        bool Save();
    }
}
