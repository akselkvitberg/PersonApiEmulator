
namespace PersonApiMock.Generators;

public class NorwegianNinGenerator
{
    private int _counterFemale = 0;
    private int _counterMale = 1;
    private readonly string _first;
    private readonly int _start;
    private static readonly int[] C1 = new int[]{3, 7, 6, 1, 8, 9, 4, 5, 2, 1};
    private static readonly int[] C2 = new int[]{5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 1};

    public NorwegianNinGenerator(DateOnly date)
    {
        _first = date.ToString("ddMMyy");
        _start = date.Year switch
        {
            <= 1899 => 500,
            >= 1940 and <= 1999 => 900,
            <= 1999 => 0,
            _ => 500,
        };
    }

    public string GetNin(Gender gender)
    {
        var individNr = GetIndividNr(gender);
        var (valid, control) = GetControlCipher(_first, individNr);
        if (!valid)
            return GetNin(gender); // get next number in line
        return $"{_first}{individNr}{control}";
    }

    private (bool, string) GetControlCipher(string first, string individNr)
    {
        var s1 = 0;
        var s2 = 0;
        for (var i = 0; i < 6; i++)
        {
            s1 += (first[i] - '0') *  C1[i];
            s2 += (first[i] - '0') *  C2[i];
        }

        for (var i = 0; i < 3; i++)
        {
            s1 += (individNr[i] - '0') *  C1[i+6];
            s2 += (individNr[i] - '0') *  C2[i+6];
        }

        var mod1 = s1 % 11;
        if (mod1 == 1)
            return (false, string.Empty);
        
        var control1 = 11 - mod1;
        s2 += control1 * C2[9];

        var mod2 = s2 % 11;
        if (mod2 == 1)
            return (false, string.Empty);

        var control2 = 11 - mod2;
        return (true, $"{control1}{control2}");
    }

    private string GetIndividNr(Gender gender)
    {
        int nr;
        if (gender == Gender.Male)
        {
            lock (this)
            {
                nr = _start + _counterMale;
                _counterMale += 2;    
            }
        }
        else
        {
            lock (this)
            {
                nr = _start + _counterFemale;
                _counterFemale += 2;
            }
        }

        return nr.ToString("000");
    }
}