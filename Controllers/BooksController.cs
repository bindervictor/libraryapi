using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Select(b => new
                {
                    b.Id,
                    b.Name,
                    b.Year,
                    Categories = b.BookCategories
                        .Where(bc => bc.Category != null) 
                        .Select(bc => new
                        {
                            bc.Category!.Id,
                            bc.Category.Name
                        })
                })
                .ToListAsync();

            return Ok(books);
        }

        // GET: api/Books/Id
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Where(b => b.Id == id)
                .Select(b => new
                {
                    b.Id,
                    b.Name,
                    b.Year,
                    Categories = b.BookCategories
                        .Where(bc => bc.Category != null) 
                        .Select(bc => new
                        {
                            bc.Category!.Id,
                            bc.Category.Name
                        })
                })
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }


        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromBody] BookCreateDTO bookDto)
        {
            var validationError = ValidateBook(bookDto.Year);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            if (bookDto.CategoryIds.Count > 3)
            {
                return BadRequest("Você pode associar no máximo 3 categorias.");
            }

            // Buscar categorias existentes no banco de dados
            var categories = await _context.Categories
                .Where(c => bookDto.CategoryIds.Contains(c.Id))
                .ToListAsync();

            if (categories.Count != bookDto.CategoryIds.Count)
            {
                return BadRequest("Uma ou mais categorias não existem.");
            }

            var newBook = new Book
            {
                Name = bookDto.Name,
                Year = bookDto.Year,
                BookCategories = categories.Select(category => new BookCategory
                {
                    CategoryId = category.Id
                }).ToList()
            };

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
        }


        // PUT: api/Books/Id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, [FromBody] BookUpdateDTO bookDto)
        {
            var existingBook = await _context.Books
                .Include(b => b.BookCategories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBook == null)
            {
                return NotFound();
            }

            var validationError = ValidateBook(bookDto.Year);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            if (bookDto.CategoryIds.Count > 3)
            {
                return BadRequest("Você pode associar no máximo 3 categorias.");
            }

            var categories = await _context.Categories
                .Where(c => bookDto.CategoryIds.Contains(c.Id))
                .ToListAsync();

            if (categories.Count != bookDto.CategoryIds.Count)
            {
                return BadRequest("Uma ou mais categorias não existem.");
            }

            // Atualiza os dados do livro
            existingBook.Name = bookDto.Name;
            existingBook.Year = bookDto.Year;
            existingBook.BookCategories.Clear(); // Remove categorias antigas

            // Associa as novas categorias
            existingBook.BookCategories = categories.Select(category => new BookCategory
            {
                BookId = existingBook.Id,
                CategoryId = category.Id
            }).ToList();

            _context.Entry(existingBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Método de validação do ano
        private string? ValidateBook(int year)
        {
            if (year > DateTime.UtcNow.Year)
            {
                return "O ano do livro não pode ser maior que o ano atual.";
            }
            return null;
        }


        // DELETE: api/Books/Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
