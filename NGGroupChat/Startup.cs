using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NGGroupChat.Service.Api.DBModels;
using NGGroupChat.Service.Api.Providers;
using NGGroupChat.Service.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace NGGroupChat
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
            services.AddControllers();
            services.AddSignalR();
            services.AddDbContext<ChatDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ChatDBCon"));
            });
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IUserService, UserService>();
            services.AddCors();
            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder =>
            builder.WithOrigins("https://localhost:44396")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials());
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/notify");
            });
        }
    }
}
