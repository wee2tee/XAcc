using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using XAcc.Migrations;
using XAcc.Models;

namespace XAcc
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
            services.AddMvc();
            
            services.AddDbContext<DBMainContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("MainDbConnection"))
            );

            //services.AddDbContext<DBContext>(options =>
            //    options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"))
            //);

            services.AddSingleton<IConfiguration>(Configuration);

            //services.AddDbContext<DBContext2>(
            //    options => options.UseMySQL(Configuration.GetConnectionString("SecConnection"))
            //);

            var serviceProvider = services.BuildServiceProvider();
            DBInitialize.INIT(serviceProvider);

            services.AddMvc(options =>
            {
                /* Automatically validate anti forgery token for every form action in whole app */
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

                /* Automatically require https:// protocal for every controller,action in whole app */
                //options.Filters.Add(new RequireHttpsAttribute());
            });


            /* Specify action when access denied, login require */
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Home/ErrorForbidden";
                    //options.LoginPath = "/Home/ErrorNotLoggedIn";
                    options.LoginPath = "/Home/LoginForm";
                });

            /* Add policy profile for use with any controller/action */
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Customer", p => p.RequireAuthenticatedUser().RequireRole("customer"));
                options.AddPolicy("User", p => p.RequireAuthenticatedUser().RequireRole("user"));

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    //template: "{controller=Home}/{action=Index}/{id?}");
                    template: "{controller=Home}/{action=LoginForm}/{id?}");
            });
        }
    }
}
