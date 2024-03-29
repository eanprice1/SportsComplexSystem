﻿using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Write;

public class CoachWriteRepo : ICoachWriteRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public CoachWriteRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<int> InsertCoachAsync(Coach model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        await context.Coach.AddAsync(entity);

        try
        {
            await context.SaveChangesAsync();
            return entity.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert coach into database. See inner exception for details.", ex);
        }
    }

    public async Task<Coach> UpdateCoachAsync(Coach model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        context.Coach.Update(entity);

        try
        {
            await context.SaveChangesAsync();
            return model;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update coach. Coach may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeleteCoachAsync(int id)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Coach.FindAsync(id);

        if (entity == null)
            throw new EntityNotFoundException($"Coach with 'Id={id}' does not exist.");

        context.Coach.Remove(entity);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not delete coach. See inner exception for details.", ex);
        }
    }

    private static CoachDb Map(Coach model)
    {
        return new CoachDb
        {
            Id = model.Id,
            TeamId = model.TeamId,
            FirstName = model.FirstName,
            LastName = model.LastName,
            BirthDate = model.BirthDate,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            Address = model.Address,
            IsHeadCoach = model.IsHeadCoach
        };
    }
}