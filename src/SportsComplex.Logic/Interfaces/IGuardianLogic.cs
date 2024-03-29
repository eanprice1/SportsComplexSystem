﻿using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces
{
    public interface IGuardianLogic
    {
        Task<List<Guardian>> GetGuardiansAsync(GuardianQuery filters);
        Task<Guardian> GetGuardianByIdAsync(int id);
        Task<Guardian> AddGuardianAsync(Guardian guardian);
        Task<Guardian> UpdateGuardianAsync(Guardian guardian);
        Task DeleteGuardianAsync(int id);
    }
}
