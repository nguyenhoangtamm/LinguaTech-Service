using LinguaTech.Application;
using LinguaTech.Application.Common.Security;
using LinguaTech.Infrastructure;
using LinguaTech.API.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application and infrastructure layers
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// Bind JwtSettings and register for IOptions
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Register a lightweight authentication scheme that trusts the HttpContext.User set by JwtMiddleware
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "ContextUser";
    options.DefaultChallengeScheme = "ContextUser";
})
.AddScheme<AuthenticationSchemeOptions, ContextUserAuthenticationHandler>("ContextUser", options => { });

// Add authorization services (required for [Authorize])
builder.Services.AddAuthorization();

// Note: using custom JwtMiddleware for authentication. It validates token and sets HttpContext.User

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Custom JWT middleware validates token and sets HttpContext.User
app.UseMiddleware<JwtMiddleware>();

// Authentication middleware ensures challenge/authorize uses the scheme above
app.UseAuthentication();

// Authorization middleware enforces [Authorize] based on HttpContext.User
app.UseAuthorization();

app.MapControllers();

app.Run();
