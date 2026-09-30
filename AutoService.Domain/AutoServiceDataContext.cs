using Bogus;
using AutoService.Domain;

namespace AutoService.Domain;

public class AutoServiceDataContext
{
    public List<Car> Cars { get; set; } = new();
    public List<WorkType> WorkTypes { get; set; } = new();
    public List<Client> Clients { get; set; } = new();
    public List<Mechanic> Mechanics { get; set; } = new();
    public List<Order> Orders { get; set; } = new();

    // Метод для генерации тестовых данных
    public void SeedData(int count = 15)  
    {
        Randomizer.Seed = new Random(42); 

        var carFaker = new Faker<Car>("ru")
            .RuleFor(c => c.GovNumber, f => f.Random.String2(10))
            .RuleFor(c => c.Brand, f => f.Vehicle.Manufacturer())
            .RuleFor(c => c.Model, f => f.Vehicle.Model())
            .RuleFor(c => c.Year, f => f.Date.Past(20).Year);

        var workTypeFaker = new Faker<WorkType>("ru")
            .RuleFor(w => w.Name, f => f.Commerce.ProductName())
            .RuleFor(w => w.Category, f => f.PickRandom<Specialization>())
            .RuleFor(w => w.Cost, f => f.Finance.Amount(500, 15000))
            .RuleFor(w => w.Duration, f => TimeSpan.FromHours(f.Random.Double(0.5, 8)));

        var mechanicFaker = new Faker<Mechanic>("ru")
            .RuleFor(m => m.PassportNumber, f => f.Random.ReplaceNumbers("#### ######"))
            .RuleFor(m => m.FullName, f => f.Name.FullName())
            .RuleFor(m => m.Specialization, f => f.PickRandom<Specialization>())
            .RuleFor(m => m.Experience, f => f.Random.Int(1, 30));

        Cars = carFaker.Generate(count);
        WorkTypes = workTypeFaker.Generate(count);
        Mechanics = mechanicFaker.Generate(count);

        var clientFaker = new Faker<Client>("ru")
            .RuleFor(c => c.FullName, f => f.Name.FullName())
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumberFormat())
            .RuleFor(c => c.Cars, (f, c) => carFaker.Generate(f.Random.Int(1, 3))); 

        Clients = clientFaker.Generate(count);

        var orderFaker = new Faker<Order>("ru")
            .RuleFor(o => o.Client, f => f.PickRandom(Clients))
            .RuleFor(o => o.Car, (f, o) => f.PickRandom(o.Client.Cars)) 
            .RuleFor(o => o.Mechanic, f => f.PickRandom(Mechanics))
            .RuleFor(o => o.WorkTypes, f => f.PickRandom(WorkTypes, f.Random.Int(1, 4)).ToList())
            .RuleFor(o => o.ReceiptDate, f => f.Date.Recent(30)) // За последний месяц
            .RuleFor(o => o.IssueDate, (f, o) => o.ReceiptDate.AddHours(f.Random.Double(1, 48)));

        Orders = orderFaker.Generate(count * 3); 
    }


    // 1. Вывести информацию о всех механиках, специализирующихся на выбранном виде работ.
    public List<Mechanic> GetMechanicsByWorkType(WorkType workType)
    {
        return Mechanics.Where(m => m.Specialization == workType.Category).ToList();
    }

    // 2. Вывести информацию обо всех клиентах, чьи автомобили обслуживались у указанного механика, упорядочить по ФИО.
    public List<Client> GetClientsByMechanic(Mechanic mechanic)
    {
        return Orders
            .Where(o => o.Mechanic == mechanic)
            .Select(o => o.Client)
            .Distinct()
            .OrderBy(c => c.FullName)
            .ToList();
    }

    // 3. Вывести информацию о количестве повторных обращений клиентов за последний месяц.
    public Dictionary<Client, int> GetRepeatClientsLastMonth()
    {
        var lastMonth = DateTime.Now.AddMonths(-1);
        return Orders
            .Where(o => o.ReceiptDate >= lastMonth)
            .GroupBy(o => o.Client)
            .Where(g => g.Count() > 1) // Только повторные (больше 1 заказа)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    // 4. Для выбранного заказа подсчитать суммарную стоимость работ.
    public decimal GetTotalOrderCost(Order order)
    {
        return order.WorkTypes.Sum(w => w.Cost);
    }

    // 5. Вывести топ 5 наиболее частых видов работ.
    public List<WorkType> GetTop5FrequentWorkTypes()
    {
        return Orders
            .SelectMany(o => o.WorkTypes)
            .GroupBy(w => w)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.Key)
            .ToList();
    }
}