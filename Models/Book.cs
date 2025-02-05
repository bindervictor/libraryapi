using System.ComponentModel.DataAnnotations;
namespace LibraryAPI.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Name { get; set; }

        [Required]
        [Range(1, 9999, ErrorMessage = "Ano inválido")]
        public int Year { get; set; }

        // Propriedade de navegação para o relacionamento Many-to-Many
        public List<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}

