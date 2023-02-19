namespace PersonApiMock.Generators;

public static class AffiliationRepository
{
    private static readonly Dictionary<string, int> Affiliations;
    private static int _counter = 1;

    static AffiliationRepository(){
        Affiliations = AddressGenerator.Regions.Select((r, i) => (r, i)).ToDictionary(x=>x.r, x=>x.i);
        foreach(var a in Affiliations){
            Console.WriteLine($"{a.Key}: {a.Value}");
        }
    }

    public static int GetOrCreateAffiliation(string affiliation)
    {
        if (Affiliations.TryGetValue(affiliation, out var id))
        {
            return id;
        }
        Affiliations.Add(affiliation, _counter);
        return _counter++;
    }
}