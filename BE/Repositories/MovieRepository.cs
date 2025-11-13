using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.Data;
using BE.Models;
using Microsoft.EntityFrameworkCore;

namespace BE.Repositories;

public class MovieRepository
{
    private readonly ApplicationDbContext _context;

    public MovieRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Movie>> GetMoviesAsync(string? search, string? genre, string? sortBy, string? sortOrder)
    {
        var query = _context.Movies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            query = query.Where(movie => EF.Functions.ILike(movie.Title, term));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            var normalizedGenre = genre.Trim();
            query = query.Where(movie => movie.Genre != null && EF.Functions.ILike(movie.Genre, normalizedGenre));
        }

        query = ApplyOrdering(query, sortBy, sortOrder);

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        return await _context.Movies.AsNoTracking().FirstOrDefaultAsync(movie => movie.Id == id);
    }

    public async Task<Movie> CreateAsync(Movie movie)
    {
        await _context.Movies.AddAsync(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        var exists = await _context.Movies.AsNoTracking().AnyAsync(m => m.Id == movie.Id);
        if (!exists)
        {
            return null;
        }

        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie is null)
        {
            return false;
        }

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return true;
    }

    private static IQueryable<Movie> ApplyOrdering(IQueryable<Movie> query, string? sortBy, string? sortOrder)
    {
        var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        return sortBy?.ToLowerInvariant() switch
        {
            "rating" => isDescending
                ? query.OrderByDescending(movie => movie.Rating).ThenBy(movie => movie.Title)
                : query.OrderBy(movie => movie.Rating).ThenBy(movie => movie.Title),
            "title" => isDescending
                ? query.OrderByDescending(movie => movie.Title)
                : query.OrderBy(movie => movie.Title),
            "createdat" => isDescending
                ? query.OrderByDescending(movie => movie.CreatedAt)
                : query.OrderBy(movie => movie.CreatedAt),
            _ => query.OrderByDescending(movie => movie.CreatedAt)
        };
    }
}


