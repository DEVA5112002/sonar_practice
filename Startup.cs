/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

//SAML
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
//END SAML

namespace AeS.ADFSSAML_MAX
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // Configuration = configuration;
            var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // Base path of the app
            .AddJsonFile(@"C:\Users\devananth.s\Downloads\Copmany_Auth\CompanyAuth\Comapny_Details.json", optional: true, reloadOnChange: true); // Load the JSON file
           //.AddConfiguration(configuration); // Add existing configuration

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            //SAML
            services.Configure<Saml2Configuration>(Configuration.GetSection("Saml2"));

            var idpMetadata = Configuration["Saml2:IdPMetadata"];
            if (string.IsNullOrWhiteSpace(idpMetadata))
            {
                throw new Exception("Saml2:IdPMetadata value is missing or invalid in configuration.");
            }

            services.Configure<Saml2Configuration>(saml2Configuration =>
            {
                saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);

                var entityDescriptor = new EntityDescriptor();
                entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(Configuration["Saml2:IdPMetadata"]));
                if (entityDescriptor.IdPSsoDescriptor != null)
                {
                    saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                    //saml2Configuration.SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location;
                    saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                }
                else
                {
                    throw new Exception("IdPSsoDescriptor not loaded from metadata.");
                }
            });

            services.AddSaml2();
            //END SAML
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSaml2(); //SAML
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                //SAML
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //END SAML
            });
        }
    }
}*/




using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// SAML
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.AspNetCore.Http;

namespace AeS.ADFSSAML_MAX
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath) // Use the application's base directory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
                .AddJsonFile(@"C:\Users\devananth.s\Downloads\Copmany_Auth\Comapny_Details.json", optional: true, reloadOnChange: true); // Load external JSON

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        public string CustomerID { get; set; }



   /*     public void ConfigureServices(IServiceCollection services)
        {
            // Add IHttpContextAccessor for accessing HttpContext during request processing
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRazorPages();

            // Configure SAML settings for later use
            services.Configure<Saml2Configuration>(Configuration.GetSection("Saml2"));

            // Add the SAML middleware service
            services.AddSaml2();
        }
*/






        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRazorPages();




            CustomerID = "AMNSUAT";
            services.AddRazorPages();

            // Configure SAML
            services.Configure<Saml2Configuration>(Configuration.GetSection("Saml2"));
     
            var idpMetadata = Configuration[$"Saml2:{CustomerID}:IdPMetadata"];
            if (string.IsNullOrWhiteSpace(idpMetadata))
            {
                throw new Exception("Saml2:IdPMetadata value is missing or invalid in configuration.");
            }

            services.Configure<Saml2Configuration>(saml2Configuration =>
            {
                saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);

                var entityDescriptor = new EntityDescriptor();
                entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(Configuration[$"Saml2:{CustomerID}:IdPMetadata"]));
                if (entityDescriptor.IdPSsoDescriptor != null)
                {
                    saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                    saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                }
                else
                {
                    throw new Exception("IdPSsoDescriptor not loaded from metadata.");
                }
            });

            services.AddSaml2();
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
                app.UseHsts();
            }


            // Middleware to capture CustomerID from query string and store in HttpContext
            app.Use(async (context, next) =>
            {
                // Extract CustomerID from query string
                var customerId = context.Request.Query["CustomerID"].ToString();

                if (!string.IsNullOrEmpty(customerId))
                {
                    // Store it in HttpContext to be accessed later in the pipeline
                    context.Items["CustomerID"] = customerId;
                }

                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSaml2(); // Add SAML middleware
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}



