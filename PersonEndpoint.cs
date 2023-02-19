using Microsoft.AspNetCore.Mvc;

namespace PersonApiMock;

public class PersonEndpoint : ControllerBase
{
    private readonly Simulator _simulator;

    public PersonEndpoint(Simulator simulator)
    {
        _simulator = simulator;
    }
    
    /// <summary>
    /// Find persons
    /// </summary>
    /// <remarks>
    /// Person retrieval is permitted through the use of scopes. For scope definitions go to https://bcc-code.github.io/projects/bcc-membership-docs/data-structures-and-scopes.
    /// </remarks>
    /// <param name="limit">Number of returned records. Must be in range 0 - 1000. (default: 100)</param>
    /// <param name="offset">Skip the first n items in the response (default: 0)</param>
    /// <param name="page">An alternative to offset. Page is a way to set offset under the hood by calculating ```limit * (page - 1)```. &lt;br&gt;If both offset and page are specified their effects will be added together  (default: 1)</param>
    /// <param name="filter">Filter the results as described in [directus](https://docs.directus.io/reference/query/#filter) json format &lt;br&gt; Allowed filter fields: ```age, brithDate, diseasedDate, displayName, email, firstName, middleName, gender, personID``` &lt;br&gt; Allowed operations ```_eq, _ne, _gt, _gte, _lt, _lte, _in, _nin``` (default: empty)</param>
    /// <param name="fields">Comma delimited list of fields to return. Leave empty for all fields. Supports ```*``` as a wildcard (default: empty) &lt;br&gt;Example: ```?fields=displayName,currentAddress.*```</param>
    /// <param name="sort">Comma delimited list of fields to sort by as described in [directus](https://docs.directus.io/reference/query/#sort), without ```?``` support. &lt;br&gt;Allowed sort fields: ```&lt;same as for filter&gt;```.&lt;br&gt;Example: ```?sort=displayName,-age```</param>
    /// <param name="search">Search term to filter and sort documents by, the documents are searched by ```fullName``` (fuzzy), ```displayName``` (fuzzy) and ```email``` (exact), if the value is in quotes then it will perform exact comparison with ```displayName```</param>
    /// <returns>OK</returns>
    [HttpGet("persons")]
    public PersonsResponse GetPersons(int? limit, int? offset, int? page, string? filter, string? fields, string? sort, string? search)
    {
        var fieldArray = fields?.Split(",") ?? new []{"*"};
        var queryable = _simulator.Persons.Values.Select(x=>new Lazy<Dictionary<string, object>>(() => x.ToDictionary(fieldArray))).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            // figure a way to emulate directus filter...
        }

        if (!string.IsNullOrWhiteSpace(sort))
        {
            var sorts = sort.Split(",").Select(x=>x.StartsWith("-") ? (false, x[1..]) : (true, x)).ToList();

            var sortedQueryable = sorts[0].Item1 ? queryable.OrderBy(x => x.Value[sorts[0].Item2]) : queryable.OrderByDescending(x => x.Value[sorts[0].Item2]);

            foreach (var (ascending, property) in sorts.Skip(1))
            {
                sortedQueryable = ascending ? sortedQueryable.ThenBy(x => x.Value[property]) : sortedQueryable.ThenByDescending(x => x.Value[property]);
            }

            queryable = sortedQueryable;
        }

        limit ??= 100;
        
        if (offset.HasValue)
            queryable = queryable.Skip(offset.Value);
        else if (page.HasValue)
            queryable = queryable.Skip(limit.Value * page.Value - 1);

        queryable = queryable.Take(limit.Value);

        return new PersonsResponse()
        {
            Data = queryable.Select(x=>x.Value).ToArray(),
            Meta = new Metadata()
            {
                Limit = limit.Value,
                Skipped = 0,
                Total = _simulator.Persons.Count
            },
        };
    }

    /// <summary>
    /// Get person by personID
    /// </summary>
    /// <remarks>
    /// Person retrieval is permitted through the use of scopes. For scope definitions go to https://bcc-code.github.io/projects/bcc-membership-docs/data-structures-and-scopes.
    /// </remarks>
    /// <param name="personId">personID</param>
    /// <param name="fields">Comma delimited list of fields to return. Leave empty for all fields. Supports ```*``` as a wildcard (default: empty) &lt;br&gt;Example: ```?fields=displayName,currentAddress.*```</param>
    /// <returns>OK</returns>
    [HttpGet("persons/{personId}")]
    public PersonResponse GetPerson(int personId, string? fields)
    {
        var fieldArray = fields?.Split(",") ?? new []{"*"};

        return new PersonResponse()
        {
            Data = _simulator.Persons[personId].ToDictionary(fieldArray),
        };
    }
}