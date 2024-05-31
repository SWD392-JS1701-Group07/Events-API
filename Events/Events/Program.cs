using Events.API.Controllers;
using Events.Business.Interfaces;
using Events.Business.Mapping;
using Events.Business.Services;
using Events.Data.Interfaces;
using Events.Data.Models;
using Events.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<IEventsDbContext, EventsDbContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true; 
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.Conventions.Controller<EventController>().HasApiVersion(new Microsoft.AspNetCore.Mvc.ApiVersion(2, 0));
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "V1.0",
        Title = "API V1",
    });

    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "V2.0",
        Title = "API V2",
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"}
            },
            Array.Empty<string>()
        }
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
}); 

//Authentication
builder.Services.AddAuthentication(options => 
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
      //      ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = false,
      //      ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = ClaimTypes.Role,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1c4890495b93b9e71fee12bf1880242771ad287f814d9553b120de5b82428b0b"))
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var token = context.SecurityToken as JwtSecurityToken;
                if (token != null && !JWTGenerator.IsTokenValid(token.RawData))
                {
                    context.Fail("Token is invalid");
                }
                return Task.CompletedTask;
            }
        };
    });
    
builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder =>
        policyBuilder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

// Register the background service
//builder.Services.AddHostedService<EventStatusUpdateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "API V2");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors();

app.Run();