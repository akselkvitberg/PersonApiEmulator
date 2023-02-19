using System.Text.Json;

namespace PersonApiMock.Generators;

public static class AddressGenerator
{
    private static readonly Random Random = new(12345);
    static AddressGenerator()
    {
        using var stream = File.OpenRead("Generators/Data/address.json");
        var countryData = JsonSerializer.Deserialize<Country>(stream);

        var list = new List<Address>();
        if (countryData != null)
        {
            foreach (var (region, cities) in countryData)
            foreach (var (city, places) in cities)
            foreach (var (postalCode, addresses) in places)
            foreach (var address in addresses)
                list.Add(new Address()
                {
                    Address1 = address,
                    PostalCode = postalCode,
                    City = city,
                    Region = region,
                    CountryName = "Norway",
                    CountryIso2Code = "NO",
                    CountryNameNative = "Norge",
                });
        }

        var regions = list.CountBy(x => x.Region).OrderByDescending(x=>x.count).Where(x=>x.key != null).Take(20).Select(x=>x.key!);

        Addresses = new Queue<Address>(list
            .Where(x=>regions.Contains(x.Region))
            .OrderBy(_ => Random.Next()));

        Regions = regions.ToList();
    }

    private static Queue<Address> Addresses { get; }
    
    public static List<string> Regions { get; private set; }

    public static Address GetRandomAddress()
    {
        return Addresses.Dequeue();
    }

    private class Country : Dictionary<string, Region> { }

    private class Region : Dictionary<string, City> {}

    private class City : Dictionary<string, PostalCode> {}

    private class PostalCode : List<string> {}

    public static void ReturnAddress(Address personAddress)
    {
        Addresses.Enqueue(personAddress);
    }
}

