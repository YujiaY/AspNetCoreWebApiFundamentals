using CityInfo_Dev;
using CityInfo_Dev.DbContexts;
using CityInfo_Dev.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
// builder.Logging.ClearProviders();  // Removed because of UseSerilog.
// builder.Logging.AddConsole();  // Removed because of UseSerilog.

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (environment == Environments.Development)
{
    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
        .MinimumLevel.Debug()
        .WriteTo.Console()
    );
}
else
{
    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File(
            Path.Combine("logs", "city info.log"),
            rollingInterval: RollingInterval.Day
        ).WriteTo.ApplicationInsights(
            new TelemetryConfiguration()
            {
                InstrumentationKey =
                    builder.Configuration["ApplicationInsightsInstrumentationKey"]
            },
            TelemetryConverter.Traces
        )
    );
}

// Add services to the container.
builder.Services.AddControllers(options =>
    {
        options.ReturnHttpNotAcceptable = true;
    })
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

builder.Services.AddProblemDetails();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

builder.Services.AddSingleton<CitiesDataStore>();

builder.Services.AddDbContext<CityInfoContext>(dbContextOptions =>
    dbContextOptions.UseSqlite(builder.Configuration.GetConnectionString("cityinfo_dev"))
        .EnableSensitiveDataLogging()
    // dbContextOptions.UseSqlite("Data source=CityInfo_dev.db")
);

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

var assemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MuseBeFromBrisbane", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Ant", "Brisbane");
    });
});

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler();
}

app.UseForwardedHeaders();

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();