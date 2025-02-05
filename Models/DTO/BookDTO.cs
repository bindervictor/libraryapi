public class BookDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Year { get; set; }
    public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
}

public class CategoryDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
