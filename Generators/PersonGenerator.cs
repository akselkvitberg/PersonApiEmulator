
namespace PersonApiMock.Generators;

public class PersonGenerator
{
    private static readonly Random Random = new (12345);

    public Dictionary<int, Person> Persons { get; }

    public PersonGenerator(int startYear, int endYear, int count)
    {
        Persons = GenerateInitialPopulation(startYear, endYear, count);
    }

    private static Dictionary<int, Person> GenerateInitialPopulation(int startYear, int endYear, int count)
    {
        var persons = new List<Person>();
        for (var i = startYear; i <= endYear; i++)
            persons.AddRange(GenerateInitialGeneration(i, count / (endYear - startYear + 1)));

        return persons.ToDictionary(x=>x.PersonID);
    }

    private static List<Person> GenerateInitialGeneration(int year, int count)
    {
        var persons = new List<Person>();
        for (var i = 0; i < count; i++)
        {
            var person = PersonBuilder
                .GetBasePerson()
                .WithRandomLastName()
                .WithEmailFromName()
                .WithRandomAddress()
                .WithRandomBirthDate(year)
                .WithNorwegianNin()
                .WithCellPhone()
                .WithHomePhone();
            persons.Add(person);
        }

        return persons;
    }

    public void SimulateYear()
    {
        var randomList = Persons.Values.OrderBy(_ => Random.Next()).ToList();
        Births(randomList);
        Marry(randomList);
        Divorce(randomList);
        Move(randomList);
        Die(randomList);
    }

    private void Births(IEnumerable<Person> randomList)
    {
        var mothers = randomList
                          .Where(x => x is { Gender: Gender.Female, MaritalStatus: MaritalStatus.Married })
                          .Where(x => x.Relations.Count(y=>y.Type == PersonRelationType.Child) <= x.PersonID % 4)
                          .ToList();
        
        foreach (var mother in mothers)
        {
            var father = Persons[mother.Relations.First(x => x.Type == PersonRelationType.Spouse).Target];
            var child = PersonBuilder.GetBasePerson()
                .WithRandomBirthDate(Simulator.Year)
                .WithNorwegianNin()
                .WithLastNameFrom(mother)
                .WithAddressFrom(mother)
                .WithEmailFromName()
                .WithCellPhone() // they get younger and younger! In my day and age...
                .AddParents(mother, father);

            mother.AddChild(child);
            father.AddChild(child);

            Persons.Add(child.PersonID, child);
        }
    }

    private void Die(List<Person> randomList)
    {
        foreach (var person in randomList.Where(x => x.Age > 80))
        {
            person.WithDeath(Simulator.Year);
            var spouse = person.Relations.FirstOrDefault(x => x.Type == PersonRelationType.Spouse);
            if (spouse != null)
            {
                Persons[spouse.Target].Widow();
            }
        }
    }

    private void Move(List<Person> randomList)
    {
        var toMove = randomList.Take(Persons.Count / 40);

        foreach (var person in toMove)
        {
            person.WithRandomAddress().WithHomePhone();

            foreach (var relation in person.Relations.Where(x=>x.Type != PersonRelationType.Parent))
            {
                var relatedPerson = Persons[relation.Target];
                if (person.Address!.Equals(relatedPerson.Address))
                {
                    relatedPerson.WithAddressFrom(person).WithHomePhoneFrom(person);
                }
            }
        }
    }

    private void Marry(List<Person> randomList)
    {
        var elig = randomList.Where(x => x is { MaritalStatus: MaritalStatus.Single, Age: > 20 and < 50 }).ToList();
        var bachelors = elig.Where(x => x.Gender == Gender.Male).OrderBy(x=>x.Age/10);
        var bachelorettes = elig.Where(x => x.Gender == Gender.Female).OrderBy(x=>x.Age/10);
        var couples = bachelors.Zip(bachelorettes).ToList();
        foreach (var (first, second) in couples.OrderBy(_=> Random.Next()).Take(couples.Count/2*3))
        {
            if (IsCloselyRelated(first, second)) continue;

            first
                .WithRandomAddress()
                .Marry(second);
            
            second
                .Marry(first)
                .WithLastNameFrom(first)
                .WithAddressFrom(first)
                .WithEmailFromName()
                .WithHomePhoneFrom(first);
        }
    }

    private void Divorce(List<Person> randomList)
    {
        var elig = randomList.Where(x => x is { MaritalStatus: MaritalStatus.Married, Gender: Gender.Male }).ToList();
        foreach (var person in elig.Take(elig.Count / 1000))
        {
            var spouse = Persons[person.Relations.First(x => x.Type == PersonRelationType.Spouse).Target];
            person.Divorce(spouse);
        }
    }

    private bool IsCloselyRelated(Person first, Person second)
    {
        var ancestors1 = GetAncestors(first, 2);
        var ancestors2 = GetAncestors(second, 2);

        return ancestors1.Intersect(ancestors2).Any();
    }

    private IEnumerable<int> GetAncestors(Person person, int generations)
    {
        if(generations <= 0) yield break;
        
        var parents = person.Relations.Where(x => x.Type == PersonRelationType.Parent);

        foreach (var parent in parents)
        {
            yield return parent.Target;
            foreach (var ancestor in GetAncestors(Persons[parent.Target], generations - 1))
            {
                yield return ancestor;
            }
        }
    }
}