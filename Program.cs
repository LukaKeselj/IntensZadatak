using Microsoft.EntityFrameworkCore;
using HRCandidateManagement.Data;
using HRCandidateManagement.Repositories;
using HRCandidateManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=hrcandidate.db"));

// Add repositories
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();

// Add services
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<ISkillService, SkillService>();

// Add controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
