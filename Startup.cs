using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using MSN.BlazorServer.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSN.ModelContext;
using MSN.Model.Util;
using Microsoft.EntityFrameworkCore;

namespace MySoloNetwork
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // Ensure the DBs are created/loaded 
            string databasePath = System.IO.Path.Combine(_env.ContentRootPath, "Data");
            if (!System.IO.Directory.Exists(databasePath))
                System.IO.Directory.CreateDirectory(databasePath);
            MSNContext msnContext = (new MSNContext(Configuration.GetConnectionString("DefaultConnection")));
            MSNBlazorServerContext msnBlazorServerContext = (new MSNBlazorServerContext(Configuration.GetConnectionString("SecurityConnection")));
            bool defaultContextPresent = msnContext.Database.CanConnect();
            bool securityContextPresent = msnBlazorServerContext.Database.CanConnect();
            if (!defaultContextPresent)
            {
                defaultContextPresent = (msnContext.Database.EnsureCreated());
            }
            else
                try
                {
                    defaultContextPresent = (msnContext?.ApplicationSettings?.Count() ?? 0) > 0;
                }
                catch (Microsoft.Data.Sqlite.SqliteException)
                {
                    msnContext.Database.EnsureDeleted();
                    msnContext.Database.EnsureCreated();
                    defaultContextPresent = true;
                }
            if (!securityContextPresent)
                securityContextPresent = (msnBlazorServerContext.Database.EnsureCreated());
            else
                try
                {
                    securityContextPresent = msnBlazorServerContext.Users.Count() > 0;
                }
                catch (Microsoft.Data.Sqlite.SqliteException)
                {
                    msnBlazorServerContext.Database.EnsureDeleted();
                    msnBlazorServerContext.Database.EnsureCreated();
                    securityContextPresent = false;
                }



            // Populate serverFileInfo
            string wwwRootFolder = _env.WebRootPath;
            string imagesDirectory;
            try
            {
                imagesDirectory = System.IO.Path.Combine(new string[] { wwwRootFolder, "segmi" });
                if (!System.IO.Directory.Exists(imagesDirectory))
                    System.IO.Directory.CreateDirectory(imagesDirectory);
            }
            catch (Exception)
            {
                System.Text.StringBuilder sb = new();
                sb.AppendLine($"WebRootPath: {_env.WebRootPath}");
                throw new Exception(sb.ToString());
            }
            string unsafeDirectory = System.IO.Path.Combine(new string[] { _env.ContentRootPath, "unsafe_uploads" });
            if (!System.IO.Directory.Exists(unsafeDirectory))
                System.IO.Directory.CreateDirectory(unsafeDirectory);


            services.AddSingleton(typeof(ServerFileInfo), new ServerFileInfo()
            {
                ImagesRootFolder = imagesDirectory,
                WWWRootFolder = wwwRootFolder,
                UnsafeImagesRootFolder = unsafeDirectory,
                AppRootDirectory = _env.ContentRootPath,
                DefaultContextPresent = defaultContextPresent,
                SecurityContextPresent = securityContextPresent,
            });


            services.AddDbContext<MSNContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<MSNBlazorServerContext>(options => options.UseSqlite(Configuration.GetConnectionString("SecurityConnection")));
            services.AddResponseCompression();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ServerFileInfo serverFileInfo)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapRazorPages();
            });



        }
    }
}
