using Microsoft.EntityFrameworkCore;
using PermitManagement.Api;
using PermitManagement.Core.Interfaces;
using PermitManagement.Core.Services;
using PermitManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PermitDbContext>(
    o => o.UseSqlite("Data Source=permit.db"));

builder.Services.AddScoped<IPermitRepository, PermitRepository>();
builder.Services.AddScoped<IPermitService, PermitService>();
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PermitDbContext>();
    context.Database.EnsureCreated();
    SeedData.Initialize(context);
}

app.UseHttpsRedirection();

PermitEndpoints.Map(app);  

app.Run();