namespace AutoService.Domain;

public class WorkType
{
    public string Name { get; set; }
    public Specialization Category { get; set; }
    public decimal Cost { get; set; }
    public TimeSpan Duration { get; set; }
}