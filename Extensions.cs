using System.Runtime.CompilerServices;

namespace PersonApiMock;

public static class Extensions {
    public static List<(TKey key, int count)> CountBy<TObj, TKey>(this IEnumerable<TObj> enumerable, Func<TObj, TKey> func)
    {
        return enumerable.GroupBy(func).Select(x => (x.Key, x.Count())).ToList();
    }

    public static Dictionary<string, object> ToDictionary(this Person person, string[] fields)
    {
        var dictionary = new Dictionary<string, object>();
        // todo: add scopes
        void Add(object? value, [CallerArgumentExpression(nameof(value))] string key = "")
        {
            key = key.Replace("person.", "");
            key = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(key);
            if (fields.Contains(key) || fields.Contains("*"))
            {
                if (value != null) dictionary.Add(key, value);
            }
        }

        Add(person.DisplayName);
        Add(person.Address);
        Add(person.Gender);
        Add(person.BirthDate);
        Add(person.MaritalStatus);
        Add(person.Affiliations);
        Add(person.Age);
        Add(person.Consents);
        Add(person.Email);
        Add(person.Preferences);
        Add(person.Relations);
        Add(person.CellPhone);
        Add(person.DiseasedDate);
        Add(person.EmailVerified);
        Add(person.FirstName);
        Add(person.LastName);
        Add(person.MiddleName);
        Add(person.CellPhoneVerified);
        Add(person.HomePhone);
        Add(person.LastChangedDate);
        Add(person.NationalIDs);
        Add(person.PersonID);
        
        return dictionary;
    }
}