using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.Dtos;
using BE.Dtos.Requests;
using BE.Extensions;
using BE.Models;
using BE.Repositories;

namespace BE.Services;

public class MovieService
{
    private readonly MovieRepository _repository;

    public MovieService(MovieRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<MovieDto>> GetMoviesAsync(string? search, string? genre, string? sortBy, string? sortOrder)
    {
        var movies = await _repository.GetMoviesAsync(search, genre, sortBy, sortOrder);
        return movies.Select(movie => movie.ToDto()).ToList();
    }

    public async Task<MovieDto?> GetMovieByIdAsync(Guid id)
    {
        var movie = await _repository.GetByIdAsync(id);
        return movie?.ToDto();
    }

    public async Task<MovieDto> CreateMovieAsync(MovieCreateRequest request)
    {
        var movie = request.ToMovie();
        var created = await _repository.CreateAsync(movie);
        return created.ToDto();
    }

    public async Task<MovieDto?> UpdateMovieAsync(Guid id, MovieUpdateRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return null;
        }

        existing.ApplyUpdate(request);
        var updated = await _repository.UpdateAsync(existing);
        return updated?.ToDto();
    }

    public Task<bool> DeleteMovieAsync(Guid id) => _repository.DeleteAsync(id);
}


