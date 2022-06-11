using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using SchedulerService.Infrastructure;

namespace SchedulerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.RegisterSchedulerServiceDependencies();

            builder.Services
                .AddControllers(o => o.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
                .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            
            app.UpdateDatabase();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();

            app.MapControllers();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            app.Run();
        }



    }
}