using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFBlog.DAL;
using Microsoft.EntityFrameworkCore;
using SFBlog.DAL.Repository;
using SFBlog.DAL.UoW;
using AutoMapper;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using NLog;
using NLog.Web;
using SFBlog.Middlewares;

namespace SFBlog
{
    public class Startup
    {
        Logger _logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        
        public Startup(IConfiguration configuration)
        { 
            Configuration = configuration;
            _logger.Debug("init main");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<SFBlogDbContext>(options => options.UseSqlite(connection), ServiceLifetime.Singleton);

            services.AddSingleton<IUnitOfWork, UnitOfWork>();

            MapperConfiguration mapperConfig = new MapperConfiguration( mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);

            
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SFBlog", Version = "v1" });
            //});

            services.AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme).
                AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = redirctContent =>
                        {
                            redirctContent.HttpContext.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                    };
                });

            //services.AddControllers();
            //services.AddEndpointsApiExplorer();
            // ��������� ��������� ������������ � ���������������
            services.AddControllersWithViews();
            //services.AddSwaggerGen();
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //������������� �� ����������� ��������
            app.UseLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(options =>
                //{
                //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                //    //options.RoutePrefix = string.Empty; //������������ ���������������� ��������� Swagger � �������� �������� ����������
                //});
            }
            else
            {
                //������������ �� ���������� ���������� ����������
                app.UseErrorHandler();
                //�������������� ���������� HTTPS
                app.UseHsts();
            }
            // ��������� ������ HTTP
            //app.UseStatusCodePages();
            //app.UseStatusCodePagesWithRedirects();
            //app.UseStatusCodePagesWithReExecute();

            //�������� ����������� ����� �������
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            //��������� ��������� ��� ����������� �������� � �������������� ������ Use.
            app.Use(async (context, next) =>
            {
                // ��� ����������� ������ � ������� ���������� ������� ������� HttpContext
                Console.WriteLine($"[{DateTime.Now}]: New request to http://{context.Request.Host.Value + context.Request.Path}");
                await next.Invoke();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Post}/{action=PostList}/{id?}"
                    );
            });
        }
    }
}
