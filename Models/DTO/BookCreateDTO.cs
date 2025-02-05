public class BookCreateDTO
{
    public required string Name { get; set; }
    public int Year { get; set; }
    public List<int> CategoryIds { get; set; } = new List<int>();
}

