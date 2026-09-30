namespace AutoService.Domain;

public class Order
{
    public Car Car { get; set; }
    public Client Client { get; set; }
    public Mechanic Mechanic { get; set; }
    public List<WorkType> WorkTypes { get; set; } = new();
    public DateTime ReceiptDate { get; set; }
    public DateTime IssueDate { get; set; }
}