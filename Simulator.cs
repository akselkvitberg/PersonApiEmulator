using PersonApiMock.Generators;

namespace PersonApiMock;

public class Simulator : BackgroundService
{
    private readonly PersonGenerator _personGenerator;
    public static int Year { get; set; } = 1940;

    public Simulator()
    {
        _personGenerator = new PersonGenerator(1920, Year, 100);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Run(() =>
        {
            try
            {
                Simulate(stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }, stoppingToken);

        Console.WriteLine("Done");
    }

    private void Simulate(CancellationToken stoppingToken)
    {
        //for (var i = 1940; i <= 2022; i++)
        while (stoppingToken.IsCancellationRequested == false)
        {
            Year++;
            Console.WriteLine($"Simulating year {Year}");
            _personGenerator.SimulateYear();
            Console.WriteLine($"Persons in year {Year}: {Persons.Count}");

            var genders = Persons.Values.CountBy(x => x.Gender);
            var ages = Persons.Values.CountBy(x => ((Year - x.BirthDate?.Year) / 10) * 10);
            var maritialStatus = Persons.Values.CountBy(x => x.MaritalStatus);
            var regions = Persons.Values.CountBy(x => x.Address.Region);

            Console.WriteLine("\nGenders");
            Print(genders);
            Console.WriteLine("\nAges");
            Print(ages);
            Console.WriteLine("\nMaritial status");
            Print(maritialStatus);
            Console.WriteLine("\nOrganizations");
            Print(regions);
            //Print(xs);

            void Print<TKey>(List<(TKey? key, int count)> valueTuples)
            {
                foreach (var (key, count) in valueTuples)
                {
                    Console.WriteLine($"{key}: {count}");
                }
            }


            Console.ReadLine();
        }
    }

    public Dictionary<int, Person> Persons => _personGenerator.Persons;
}