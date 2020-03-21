using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace PantryPlanner
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.None;
                options.Secure = true ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
            });

            services.AddCors();

            services.AddAuthentication(options =>
                    {
                        //options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                        //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.Cookie.HttpOnly = true;
                        options.Cookie.SecurePolicy = true ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
                        options.Cookie.SameSite = SameSiteMode.Lax;
                        options.Cookie.Name = "Pantry.AuthCookieAspNetCore";
                        options.LoginPath = "/Identity/Account/Login";
                        options.LogoutPath = "/Identity/Account/Logout";
                    })
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false; // TODO: make conditional if in dev environment
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidIssuer = Configuration["JwtIssuer"],
                            ValidAudience = Configuration["JwtIssuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                            ClockSkew = TimeSpan.Zero // remove delay of token when expire
                        };
                    })
                    .AddOpenIdConnect("Google", "Google", options =>
                    {
                        IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");

                        options.Authority = "https://accounts.google.com/";
                        options.ClientId = googleAuthNSection["ClientId"];
                        options.CallbackPath = "/signin-google";
                        options.SignedOutCallbackPath = "/signout-callback-google";
                        options.RemoteSignOutPath = "/signout-google";
                        options.SaveTokens = true;
                        options.Scope.Add("email");
                    });


            IConfigurationSection connStrings = Configuration.GetSection("ConnectionStrings");

            string connection = connStrings["PantryPlannerIdentityContextConnection"];
            services.AddDbContext<Services.PantryPlannerContext>(options => options.UseSqlServer(connection));

            services.AddMvc(options => options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter()))
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
