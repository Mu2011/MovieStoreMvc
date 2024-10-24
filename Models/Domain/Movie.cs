using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieStoreMvc.Models.Domain;

public class Movie
{
  public int Id { get; set; }

  [Required]
  [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
  public string Title { get; set; } = string.Empty;

  // [Required]
  [Range(1888, 2100, ErrorMessage = "Please enter a valid year.")]
  public int? ReleaseYear { get; set; }

  [StringLength(255, ErrorMessage = "Movie image name cannot exceed 255 characters.")]
  public string MovieImage { get; set; } = string.Empty;

  [Required]
  [StringLength(1000, ErrorMessage = "Cast list cannot exceed 1000 characters.")]
  public string Cast { get; set; } = string.Empty;

  [Required]
  [StringLength(255, ErrorMessage = "Director's name cannot exceed 255 characters.")]
  public string Director { get; set; } = string.Empty;

  // This property is not mapped to the database, but required for UI handling
  [NotMapped]
  // [Required(ErrorMessage = "Please upload a movie image.")]
  public IFormFile? ImageFile { get; set; }

  // List of genre IDs selected from a dropdown
  [NotMapped]
  [Required(ErrorMessage = "Please select at least one genre.")]
  public List<int> Genres { get; set; } = []; // Initialize to avoid null

  // Genre options for UI dropdown
  [NotMapped] // Ensure EF does not try to map this to the database
  public IEnumerable<SelectListItem>? GenreList { get; set; }

  [NotMapped] // Ensure EF does not try to map this to the database
  public string GenreNames { get; set; } = string.Empty;

  // List of genre IDs selected from a dropdown
  [NotMapped]
  [Required(ErrorMessage = "Please select at least one genre.")]
  public List<int> MultiGenres { get; set; } = []; // Initialize to avoid null

  // Genre options for UI dropdown
  [NotMapped] // Ensure EF does not try to map this to the database
  public MultiSelectList? MultiGenreList { get; set; }
}