using coworking;
using coworking.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddRegistration(configuration);

//IoCRegister.AddLogsRegistration(builder);

var app = builder.Build();

IocRegister.AddRegistration(app, app.Environment);

/*
builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();
*/
//handle migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<CoworkingManager>();
        context.Database.Migrate(); // apply all migrations

        var dataSeeder = services.GetRequiredService<DataSeeder>();
        dataSeeder.SeedData(); // seed the db
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the DB.");
    }
}


app.Run();
