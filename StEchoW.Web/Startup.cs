using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StEchoW.Web.Hubs;

namespace StEchoW.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            // -------------------------------
            // I want http and https!
            // app.UseHttpsRedirection();
            // -------------------------------
            
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            app.UseStaticFiles(new StaticFileOptions() {
                // To serve extensionless files (needed for "let's encrypt"'s certbot)
                // we must serve unknown file types
                // ------------------------------------------------------------------------
                // NOTE: in other project I don't go this way, but
                // I add the following action to HomeController (or any other one)
                //      [AllowAnonymous]
                //      [HttpGet]
                //      [Route(".well-known/acme-challenge/{id}")]
                //      public ActionResult LetsEncrypt(string id)
                //      {
                //          var file = Path.Combine(this.HostingEnv.WebRootPath, ".well-known", "acme-challenge", id);
                //          return PhysicalFile(file, "text/plain");            
                //      }
                // ------------------------------------------------------------------------
                DefaultContentType = "application/octet-stream",
                ServeUnknownFileTypes = true,
            });
            
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chatep");
            });
        }
    }
}