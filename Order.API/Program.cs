using Order.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// sadece 1 kere aya�a kalkar.
builder.Services.AddSingleton<IBus, Bus>(

    //var logger = sp.GetService<ILogger<Bus>>();
    //var bus = new Bus(builder.Configuration, logger!);
    //bus.CreateTopicOrQueue([BusConsts.OrderCreatedEventTopicName]);
    //return bus;
);

// Arka tarafta veritaban� i�lemleri vs oldu�u i�in ya�am d�ng�s� scoped ayarl�yoruz.
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

// Ext method ile
await app.CreateTopicsOrQueues();

// veya Uygulama aya�a kalkarken �al��s�n dersek
//using (var scope = app.Services.CreateScope())
//{
//    var ibus = scope.ServiceProvider.GetRequiredService<IBus>();

//    await ibus.CreateTopicOrQueue([BusConsts.OrderCreatedEventTopicName]);
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

