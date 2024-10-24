using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MovieStoreMvc.Models.Domain;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : IdentityDbContext<ApplicationUser>(options)
{
  public DbSet<Genre> Genre { get; set; } = null!;
  public DbSet<Movie> Movie { get; set; } = null!;
  public DbSet<MovieGenre> MovieGenre { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    // Additional configurations...
  }
}