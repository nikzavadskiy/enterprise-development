namespace AutoService.Domain;

public class Mechanic
{
    public string PassportNumber { get; set; }
    public string FullName { get; set; }
    public Specialization Specialization { get; set; }
    public int Experience { get; set; }
}