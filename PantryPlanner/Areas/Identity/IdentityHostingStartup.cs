using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PantryPlanner.Models;
using PantryPlanner.Services;

[assembly: HostingStartup(typeof(PantryPlanner.Areas.Identity.IdentityHostingStartup))]
namespace PantryPlanner.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<PantryPlannerContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("PantryPlannerIdentityContextConnection")));

                services.AddDefaultIdentity<PantryPlannerUser>()
                    .AddEntityFrameworkStores<PantryPlannerContext>();
            });
        }
    }
}