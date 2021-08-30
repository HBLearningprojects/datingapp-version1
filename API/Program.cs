using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // var host = CreateHostBuilder(args).Build();
            var webHost = CreateWebHostBuilder(args).Build();
            
            using var webScope = webHost.Services.CreateScope();
            // using var scope = host.Services.CreateScope();
            // var services = scope.ServiceProvider;
            var services = webScope.ServiceProvider;
            var context = services.GetRequiredService<DataContext>();
            // context.Database.Migrate();
            try
            {                
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                await context.Database.MigrateAsync();  
                await Seed.SeedUsers(userManager, roleManager);

                // await context.Database.MigrateAsync(); 
                //Manually run any outstanding migrations if configured to do so
                // var envAutoMigrate = Environment.GetEnvironmentVariable("AUTO_MIGRATE");
                // if (envAutoMigrate != null && envAutoMigrate == "true")
                // {                    
                //     await context.Database.MigrateAsync();  
                //     // context.Database.Migrate();
                // }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An Error Occurred during Migration.");                
            }            

            // await host.RunAsync();
            await webHost.RunAsync();
        }

        // public static IHostBuilder CreateHostBuilder(string[] args) =>
        //     Host.CreateDefaultBuilder(args)
        //         .ConfigureWebHostDefaults(webBuilder =>
        //         {
        //             webBuilder.UseStartup<Startup>();
        //         });

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    
    }
}
