using System.Text.Json;

namespace PersonApiMock.Generators;

public class WeightedList
{
    public WeightedList(string filename)
    {
        var values = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText($"Generators/Data/{filename}"));

        var list = new List<(int weight, string name)>();

        var weightCounter = 0;
        foreach (var (value, weight) in values)
        {
            list.Add((weightCounter, value));
            weightCounter += weight;
        }

        _items = list;
        _totalWeight = weightCounter;
    }

    private readonly List<(int weight, string name)> _items;
    private readonly int _totalWeight;

    public string GetRandomValue()
    {
        var random = Random.Shared.Next(_items.Count);
        return _items[random].name;
    }
    
    public string GetWeightedValue()
    {
        var random = Random.Shared.Next(_totalWeight);

        foreach (var (weight, value) in _items)
        {
            if (random <= weight)
                return value;
        }

        return _items.Last().name;
    }
    
}