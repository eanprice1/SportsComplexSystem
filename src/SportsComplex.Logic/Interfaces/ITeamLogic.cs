﻿using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces;

public interface ITeamLogic
{
    Task<List<Team>> GetTeamsAsync(TeamQuery filters);
    Task<Team> GetTeamByIdAsync(int teamId);
    Task<Team> AddTeamAsync(Team team);
    Task<Team> UpdateTeamAsync(Team team);
    Task DeleteTeamAsync(int teamId);
}