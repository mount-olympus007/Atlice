using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Microsoft.EntityFrameworkCore;
using WorkerService1;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContextFactory<EFDbContext>(options => options.UseSqlServer("Server=tcp:soskyhigh.database.windows.net,1433;Initial Catalog=atlicemaster;Persist Security Info=False;User ID=dartholympus;Password=5tgb^YHN7ujm;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));

        services.AddTransient<IDataRepository, EFDataRepository>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
