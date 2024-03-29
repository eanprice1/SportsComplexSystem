﻿namespace SportsComplex.Logic.Models;

public class SportQuery
{
    public List<int> Ids { get; init; } = new();
    public string? Name { get; init; }
    public DateTime? StartRange { get; init; }
    public DateTime? EndRange { get; init; }
    public int? Count { get; init; }
    public string? OrderBy { get; init; }
    public bool Descending { get; init; }
}