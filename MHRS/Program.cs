using MHRS.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MHRS API",
        Version = "v1",
        Description = "Hospital Appointment Management System API"
    });
});

// CORS configuration - DÜZELTİLDİ
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5071",     // Frontend port'u
            "https://localhost:7116",    // HTTPS Backend port'u
            "http://localhost:7116",     // HTTP Backend port'u (eğer varsa)
            "http://localhost:3000"      // Alternatif frontend
        )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
    });

builder.Services.AddOpenApi();
// Email service registration
builder.Services.AddScoped<MHRS.Services.EmailService>();
// DbContext configuration
builder.Services.AddDbContext<AppointmentDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    );
});

// Logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Development environment configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MHRS API v1");
        options.RoutePrefix = "swagger";
    });
    app.MapOpenApi();
}

// Middleware pipeline - CORS ÖNCE KULLANILMALI
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Statik dosyaları sun (HTML, CSS, JS)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

// Database connection test
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();
    if (dbContext.Database.CanConnect())
    {
        Console.WriteLine("✅ Database connection successful!");
    }
    else
    {
        Console.WriteLine("❌ Cannot connect to database!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database connection error: {ex.Message}");
}

Console.WriteLine("🚀 API çalışıyor...");
app.Run("https://localhost:7116");