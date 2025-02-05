using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;

namespace LibraryAPI.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ficção" },
                new Category { Id = 2, Name = "Drama" },
                new Category { Id = 3, Name = "Romance" },
                new Category { Id = 4, Name = "Terror" },
                new Category { Id = 5, Name = "Fantasia" }
            );

            // Configuração do relacionamento Many-to-Many entre Book e Category
            modelBuilder.Entity<BookCategory>()
                .HasKey(bc => new { bc.BookId, bc.CategoryId });

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.BookCategories)  
                .HasForeignKey(bc => bc.BookId);

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.BookCategories)  
                .HasForeignKey(bc => bc.CategoryId);

            modelBuilder.Entity<BookCategory>().ToTable("BookCategories");    
        }
    }
}
