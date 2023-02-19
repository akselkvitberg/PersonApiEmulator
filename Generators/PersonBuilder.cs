using System.Collections.Concurrent;

namespace PersonApiMock.Generators;

public static class PersonBuilder
{
    private static readonly Random Random = new(12345);

    private static readonly WeightedList LastNames = new("lastNames.txt");
    private static readonly WeightedList MaleFirstNames = new("maleNames.txt");
    private static readonly WeightedList FemaleFirstNames = new("femaleNames.txt");
    private static readonly WeightedList FemaleMiddleNames = new("femaleMiddleNames.txt");
    private static readonly WeightedList MaleMiddleNames = new("maleMiddleNames.txt");
    private static int _nextPersonId = 10000;

    private static T GetRandom<T>(params T[] options)
    {
        return options[Random.Next(options.Length)];
    }

    private static Gender GetGender()
    {
        return GetRandom(Gender.Male, Gender.Female);
    }

    private static bool ShouldIncludeMiddleName()
    {
        return Random.NextDouble() > 0.9;
    }

    private static int NextPersonId()
    {
        return _nextPersonId++;
    }

    public static Person GetBasePerson()
    {
        var gender = GetGender();
        var firstName = gender == Gender.Male
            ? MaleFirstNames.GetWeightedValue()
            : FemaleFirstNames.GetWeightedValue();
        var middleName = ShouldIncludeMiddleName()
            ? gender == Gender.Male ? MaleMiddleNames.GetWeightedValue() : FemaleMiddleNames.GetWeightedValue()
            : null;

        return new Person()
        {
            FirstName = firstName,
            MiddleName = middleName,
            Gender = gender,
            PersonID = NextPersonId(),
            MaritalStatus = MaritalStatus.Single,
            Relations = new List<PersonRelation>(),
            Affiliations = new List<PersonAffiliation>(),
            Consents = new List<Consent>(),
            NationalIDs = new List<NationalID>(),
        };
    }

    public static Person WithRandomLastName(this Person person)
    {
        var lastName = LastNames.GetWeightedValue();

        person.LastName = lastName;
        person.Touch();
        return person;
    }

    public static Person WithLastNameFrom(this Person person, Person parent)
    {
        person.LastName = parent.LastName;
        person.Touch();

        return person;
    }

    private static readonly ConcurrentDictionary<DateOnly, NorwegianNinGenerator> NinGenerators = new();

    private static NationalID GenerateNin(DateOnly dateOfBirth, Gender gender)
    {
        if (!NinGenerators.ContainsKey(dateOfBirth))
            NinGenerators.TryAdd(dateOfBirth, new NorwegianNinGenerator(dateOfBirth));

        return new NationalID()
            { Id = NinGenerators[dateOfBirth].GetNin(gender), CountryIso2Code = "NO" }; // NO correct?
    }

    public static Person WithRandomBirthDate(this Person person, int year)
    {
        var birthDate = new DateOnly(year, 1, 1).AddDays(Random.Next(360));
        person.BirthDate = birthDate;
        person.Touch();

        return person;
    }

    public static Person WithNorwegianNin(this Person person)
    {
        if (person.BirthDate.HasValue)
        {
            var nin = GenerateNin(person.BirthDate.Value, person.Gender ?? Gender.Male);
            person.NationalIDs.Add(nin);
            person.Touch();
            return person;
        }
        else
        {
            // generate FH-number
            return person;
        }
    }

    public static Person WithDeath(this Person person, int year)
    {
        person.DiseasedDate = new DateOnly(year, 1, 1).AddDays(Random.Next(360));
        person.Touch();
        return person;
    }

    public static Person WithRandomAddress(this Person person)
    {
        if (person.Address != null)
        {
            AddressGenerator.ReturnAddress(person.Address);
        }
        person.Address = AddressGenerator.GetRandomAddress();
        person.WithAffiliation(person.Address.Region);
        person.Touch();
        return person;
    }

    public static Person WithAddressFrom(this Person person, Person other)
    {
        if (person.Address != null)
        {
            AddressGenerator.ReturnAddress(person.Address);
        }
        person.Address = other.Address;
        person.WithAffiliation(person.Address?.Region);
        person.Touch();
        return person;
    }

    public static Person WithAffiliation(this Person person, string? affiliation)
    {
        if (affiliation == null) return person;
        person.Affiliations = new List<PersonAffiliation>()
        {
            new()
            {
                Type = PersonAffiliationType.Member,
                Active = true,
                OrgType = OrgType.Church,
                OrgID = AffiliationRepository.GetOrCreateAffiliation(affiliation)
            }
        };
        
        person.Touch();
        return person;
    }

    public static Person WithEmailFromName(this Person person)
    {
        var email = (person.FirstName + "." + person.LastName + "@orginn.no").ToLowerInvariant();

        person.Email = email;
        person.EmailVerified = true;
        person.Touch();
        return person;
    }

    public static Person WithCellPhone(this Person person)
    {
        person.CellPhone = $"+47{Random.Next(1000000, 9999999).ToString()}";
        person.CellPhoneVerified = true;
        person.Touch();
        return person;
    }

    public static Person WithHomePhone(this Person person)
    {
        person.HomePhone = $"+47{Random.Next(1000000, 9999999)}";
        person.Touch();
        return person;
    }

    public static Person WithHomePhoneFrom(this Person person, Person fromPerson)
    {
        person.HomePhone = fromPerson.HomePhone;
        person.Touch();
        return person;
    }

    public static Person Marry(this Person person, Person partner)
    {
        person.Relations.Add(new PersonRelation()
        {
            Target = partner.PersonID,
            Type = PersonRelationType.Spouse,
        });
        person.MaritalStatus = MaritalStatus.Married;
        person.Touch();
        return person;
    }

    public static Person Divorce(this Person person, Person spouse)
    {
        person.Relations.RemoveAll(x => x.Target == spouse.PersonID && x.Type == PersonRelationType.Spouse);
        spouse.Relations.RemoveAll(x => x.Target == person.PersonID && x.Type == PersonRelationType.Spouse);

        person.MaritalStatus = MaritalStatus.Separated;
        spouse.MaritalStatus = MaritalStatus.SingleParent;

        person.WithRandomAddress()
            .WithHomePhone();
        spouse.Touch();

        return person;
    }

    public static Person AddChild(this Person person, Person child)
    {
        person.Relations.Add(new PersonRelation()
        {
            Target = child.PersonID,
            Type = PersonRelationType.Child,
        });
        person.Touch();
        return person;
    }

    public static Person AddParents(this Person person, Person parent1, Person parent2)
    {
        person.Relations.AddRange(new[]
        {
            new PersonRelation()
            {
                Target = parent1.PersonID,
                Type = PersonRelationType.Parent,
            },
            new PersonRelation()
            {
                Target = parent2.PersonID,
                Type = PersonRelationType.Parent,
            },
        });
        person.Touch();
        return person;
    }

    public static Person Widow(this Person person)
    {
        person.MaritalStatus = MaritalStatus.Widowed;
        person.Touch();
        return person;
    } 
}