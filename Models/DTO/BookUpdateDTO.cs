public class BookUpdateDTO
{
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public List<int> CategoryIds { get; set; } = new List<int>();
}
