using System;
using aspCart.Core.Interface.Services.Catalog;
using aspCart.Core.Interface.Services.Messages;
using aspCart.Core.Interface.Services.Sale;
using aspCart.Core.Interface.Services.Statistics;
using aspCart.Core.Interface.Services.User;
using aspCart.Infrastructure;
using aspCart.Infrastructure.EFModels;
using aspCart.Infrastructure.EFRepository;
using aspCart.Infrastructure.Services.Catalog;
using aspCart.Infrastructure.Services.Messages;
using aspCart.Infrastructure.Services.Sale;
using aspCart.Infrastructure.Services.Statistics;
using aspCart.Infrastructure.Services.User;
using aspCart.Web.Areas.Admin.Helpers;
using aspCart.Web.Helpers;
using aspCart.Web.Middleware;
using aspCart.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;

namespace aspCart.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("tr-TR", "tr-TR");
            });

            var mapperConfiguration = new MapperConfiguration(o =>
            {
                o.AddProfile(new AutoMapperProfileConfiguration());
            });

            var mapper = mapperConfiguration.CreateMapper();

            services.AddSingleton(mapper);

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // idenity password requirement
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // configure admin account injectable
            services.Configure<AdminAccount>(
                Configuration.GetSection("AdminAccount"));

            services.Configure<UserAccount>(
                Configuration.GetSection("UserAccount"));

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.Name = "aspCart";
            });

            // Add application services.
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IBillingAddressService, BillingAddressService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IImageManagerService, ImageManagerService>();
            services.AddTransient<IManufacturerService, ManufacturerService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IReviewService, ReviewService>();
            services.AddTransient<ISpecificationService, SpecificationService>();

            services.AddTransient<IOrderCountService, OrderCountService>();
            //services.AddTransient<IVisitorCountService, VisitorCountService>();

            services.AddTransient<IContactUsService, ContactUsService>();


            services.AddTransient<ViewHelper>();
            services.AddTransient<DataHelper>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // redirect http request to https with 301 status code
                var options = new RewriteOptions().AddRedirectToHttpsPermanent();
                app.UseRewriter(options);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseRequestLocalization();
            app.UseImageResize();
            app.UseStatusCodePages();
            app.UseSession();
            //app.UseVisitorCounter();

            app.UseEndpoints(endpoints =>
            {
                 endpoints.MapControllerRoute(
                 name: "areaRoute",
                 pattern: "{area:exists}/{controller}/{action}/{id?}",
                 defaults: new { controller = "Dashboard", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "productInfo",
                    pattern: "Product/{seo}",
                    defaults: new { controller = "Home", action = "ProductInfo" });

                endpoints.MapControllerRoute(
                    name: "category",
                    pattern: "Category/{category}",
                    defaults: new { controller = "Home", action = "ProductCategory" });

                endpoints.MapControllerRoute(
                    name: "manufacturer",
                    pattern: "Manufacturer/{manufacturer}",
                    defaults: new { controller = "Home", action = "ProductManufacturer" });

                endpoints.MapControllerRoute(
                    name: "productSearch",
                    pattern: "search/{name?}",
                    defaults: new { controller = "Home", action = "ProductSearch" });

                endpoints.MapControllerRoute(
                    name: "create review",
                    pattern: "CreateReview/{id}",
                    defaults: new { controller = "Home", action = "CreateReview" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}