using System.ComponentModel.DataAnnotations;

namespace BE.Dtos.Requests;

public class MovieUpdateRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Genre { get; set; }

    [Range(1, 5)]
    public int? Rating { get; set; }

    [Url]
    [MaxLength(500)]
    public string? PosterUrl { get; set; }
}


