using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class BookCategory
    {
        [Key]
        public int BookId { get; set; }

        public Book? Book { get; set; } // Permitir nulo

        [Required]
        public int CategoryId { get; set; }

        public Category? Category { get; set; } // Permitir nulo
    }
}
