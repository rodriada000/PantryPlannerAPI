using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PantryPlanner.Areas.Identity.Data;
using PantryPlanner.Models;

[assembly: HostingStartup(typeof(PantryPlanner.Areas.Identity.IdentityHostingStartup))]
namespace PantryPlanner.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<PantryPlannerIdentityContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("PantryPlannerIdentityContextConnection")));

                services.AddDefaultIdentity<PantryPlannerUser>()
                    .AddEntityFrameworkStores<PantryPlannerIdentityContext>();
            });
        }
    }
}