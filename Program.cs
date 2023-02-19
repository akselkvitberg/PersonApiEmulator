using System.Text;
using System.Text.Json;
using PersonApiMock;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Simulator>();
builder.Services.AddHostedService<Simulator>(provider => provider.GetRequiredService<Simulator>());
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapPost("/oauth/token", () =>
{
    var now = DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds();
    var obj = new {
        exp = now,
    };

    var json = JsonSerializer.Serialize(obj);
    var encoded = Encoding.UTF8.GetBytes(json);
    var base64 = Convert.ToBase64String(encoded);
    return new
    {
        access_token = Guid.NewGuid().ToString() + "." + base64, 
        // https://oauth.net/2/access-tokens/ Access tokens must not be read or interpreted by the OAuth client. The OAuth client is not the intended audience of the token.
        // But when using BccCode.Persons.Api.Client this is required
        token_type = "",
    };
});

app.Run();


