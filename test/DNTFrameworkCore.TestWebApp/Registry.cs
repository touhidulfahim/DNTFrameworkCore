using DNTFrameworkCore.TestWebApp.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.TestWebApp
{
    public static class Registry
    {
        public static void AddWeb(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.SlidingExpiration = false;
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    //options.AccessDeniedPath = new PathString("/Home/Forbidden/");
                    options.Cookie.Name = "auth.dntframework";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = context =>
                        {
                            var validator = context
                                .HttpContext
                                .RequestServices
                                .GetRequiredService<ICookieValidator>();

                            return validator.ValidateAsync(context);
                        }
                    };
                });

            services.AddLocalization();
            services.AddHttpContextAccessor();
            services.AddMvc()
                .AddMvcLocalization()
                .AddViewLocalization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }
    }
}