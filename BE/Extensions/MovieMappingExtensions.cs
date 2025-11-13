using BE.Dtos;
using BE.Dtos.Requests;
using BE.Models;

namespace BE.Extensions;

public static class MovieMappingExtensions
{
    public static MovieDto ToDto(this Movie movie)
    {
        return new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Genre = movie.Genre,
            Rating = movie.Rating,
            PosterUrl = movie.PosterUrl
        };
    }

    public static Movie ToMovie(this MovieCreateRequest request)
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Genre = string.IsNullOrWhiteSpace(request.Genre) ? null : request.Genre.Trim(),
            Rating = request.Rating,
            PosterUrl = string.IsNullOrWhiteSpace(request.PosterUrl) ? null : request.PosterUrl.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void ApplyUpdate(this Movie movie, MovieUpdateRequest request)
    {
        movie.Title = request.Title.Trim();
        movie.Genre = string.IsNullOrWhiteSpace(request.Genre) ? null : request.Genre.Trim();
        movie.Rating = request.Rating;
        movie.PosterUrl = string.IsNullOrWhiteSpace(request.PosterUrl) ? null : request.PosterUrl.Trim();
        movie.UpdatedAt = DateTime.UtcNow;
    }
}


