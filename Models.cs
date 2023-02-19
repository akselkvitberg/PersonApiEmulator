#nullable enable

using System.Collections.Immutable;

namespace PersonApiMock;

public record Address
{
    public string? Address1 { get; set; } = default!;

    public string? Address2 { get; set; } = default!;

    public string? Address3 { get; set; } = default!;

    public string? City { get; set; } = default!;

    public string? CountryIso2Code { get; set; } = default!;

    public string? CountryName { get; set; } = default!;

    public string? CountryNameNative { get; set; } = default!;

    public string? PostalCode { get; set; } = default!;

    public string? Region { get; set; } = default!;
}

public record Consent
{
    public int? OrgID { get; set; } = default!;

    public ConsentStatus? Status { get; set; } = default!;
}

public record Error
{
    public string? Code { get; set; } = default!;

    public string? Message { get; set; } = default!;
}

public record ErrorResponse
{
    public Error? Error { get; set; } = default!;
}

public record Metadata
{
    public int? Limit { get; set; } = default!;

    public int? Skipped { get; set; } = default!;

    public int? Total { get; set; } = default!;
}

public record NationalID
{
    public string? CountryIso2Code { get; set; } = default!;

    public string? Id { get; set; } = default!;
}

public class Person
{
    public Address? Address { get; set; }

    public List<PersonAffiliation> Affiliations { get; set; } = new();

    public int? Age => Simulator.Year - BirthDate?.Year;

    public DateOnly? BirthDate { get; set; }

    public string? CellPhone { get; set; }

    public bool? CellPhoneVerified { get; set; } = default!;

    public List<Consent> Consents { get; set; } = new();

    public DateOnly? DiseasedDate { get; set; }

    public string? DisplayName => $"{FirstName} {LastName}";

    public string? Email { get; set; } = default!;

    public bool? EmailVerified { get; set; } = default!;

    public string? FirstName { get; set; } = default!;

    public Gender? Gender { get; set; } = default!;

    public string? HomePhone { get; set; } = default!;

    public string LastChangedDate { get; set; } = DateTimeOffset.Now.ToString("O");

    public string? LastName { get; set; } = default!;

    public MaritalStatus? MaritalStatus { get; set; } = default!;
    public string? MiddleName { get; set; } = default!;
    public List<NationalID> NationalIDs { get; set; } = new();
    public int PersonID { get; set; } = default!;
    public PersonPreferences? Preferences { get; set; } = default!;
    public List<PersonRelation> Relations { get; set; } = new();

    public void Touch() => LastChangedDate = DateTimeOffset.Now.ToString("O");
}

public record PersonAffiliation
{
    public bool? Active { get; set; } = default!;
    public int? OrgID { get; set; } = default!;
    public OrgType? OrgType { get; set; } = default!;
    public PersonAffiliationType? Type { get; set; } = default!;
    public string? ValidFrom { get; set; } = default!;
}

public record PersonPreferences
{
    public List<string>? ContentLanguages { get; set; } = default!;
    public List<string>? UiLanguages { get; set; } = default!;
    public PersonPreferencesVisibility? Visibility { get; set; } = default!;
}

public record PersonPreferencesVisibility
{
    public bool? BirthdayList { get; set; } = default!;
    public PersonPreferencesVisibilitySearch? Search { get; set; } = default!;
}

public record PersonRelation
{
    public int Target { get; set; }
    public PersonRelationType? Type { get; set; } = default!;
    public DateOnly? ValidFrom { get; set; } = default!;
    public DateOnly? ValidTo { get; set; } = default!;
}

public record PersonResponse
{
    public Dictionary<string, object> Data { get; set; } = default!;
}

public record PersonsResponse
{
    public Dictionary<string, object>[] Data { get; set; } = default!;
    public Metadata? Meta { get; set; } = default!;
}

public enum ConsentStatus
{
    Given = 0,
    Withdrawn = 1,
}

public enum Gender
{
    Unknown = 0,
    Male = 1,
    Female = 2,
}

public enum MaritalStatus
{
    Unknown = 0,
    Single = 1,
    Married = 2,
    Widowed = 3,
    Separated = 4,
    SingleParent = 5,
}

public enum OrgType
{
    Unknown = 0,
    Church = 1,
    Org = 2,
    Club = 3,
}

public enum PersonAffiliationType
{
    Unknown = 0,
    Member = 1,
    Affiliate = 2,
}

public enum PersonPreferencesVisibilitySearch
{
    Global = 0,
    District = 1,
    Hidden = 2,
}

public enum PersonRelationType
{
    Child = 0,
    Parent = 1,
    Spouse = 2,
    ContactPerson = 3,
    ContactDependent = 4,
    LegalGuardian = 5,
    LegalDependent = 6,
    FosterParent = 7,
    FosterChild = 8,
}