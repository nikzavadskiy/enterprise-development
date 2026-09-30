namespace AutoService.Domain;

public class Client
{
    public string FullName { get; set; }
    public string Phone { get; set; }
    public List<Car> Cars { get; set; } = new();
}