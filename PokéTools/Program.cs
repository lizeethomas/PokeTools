using PokéTools.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<PokemonService>();
builder.Services.AddScoped<TypeService>();
builder.Services.AddScoped<AbilityService>();
builder.Services.AddScoped<MoveService>();
builder.Services.AddScoped<TsvService>();
builder.Services.AddScoped<PokemonService>();
builder.Services.AddScoped<ItemService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
