using Microsoft.EntityFrameworkCore;
using QuotesManagement.Domain.Repositories;
using QuotesManagement.Domain.Services;
using QuotesManagement.Persistence.Data;
using QuotesManagement.Persistence.Repositories;
using QuotesManagement.Persistence.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:7247")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddControllers();


// Add DB services to the container.
builder.Services.AddDbContext<QuoteContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<IQuoteService, QuoteService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();

app.MapControllers();

app.Run();
