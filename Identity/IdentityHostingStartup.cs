using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSN.BlazorServer.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;

[assembly: HostingStartup(typeof(MSN.BlazorServer.Areas.Identity.IdentityHostingStartup))]
namespace MSN.BlazorServer.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<MSNBlazorServerContext>(options =>
                    options.UseSqlite("Data Source=Data/MSNSecurity.db;"));

                services.AddDefaultIdentity<IdentityUser>(options =>
                {

                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Lockout.MaxFailedAccessAttempts = int.MaxValue;
                    options.Lockout.AllowedForNewUsers = false;
                }
                )
                    .AddEntityFrameworkStores<MSNBlazorServerContext>();
            });
        }
    }
}