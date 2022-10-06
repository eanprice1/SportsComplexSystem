using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using SportsComplex.API.Api.JSend;
using SportsComplex.Logic;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;
using SportsComplex.Repository;
using SportsComplex.Repository.Read;
using SportsComplex.Repository.Write;

namespace SportsComplex.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SportsComplex API",
                    Version = "v1",
                    Description =
                        "An API used to manage SportsComplex operations such as registration of players and match/practice schedules."
                });
                c.EnableAnnotations();
            });

            services.AddControllers();

            var sqlConnection = new SqlConnection(_configuration.GetConnectionString("SportsComplexConnection"));
            var sportsComplexDbContextOptions = new DbContextOptionsBuilder<SportsComplexDbContext>()
                .UseSqlServer(sqlConnection)
                .Options;

            services.AddSingleton(sportsComplexDbContextOptions);

            //Repos
            services.AddTransient<IGuardianReadRepo, GuardianReadRepo>();
            services.AddTransient<IGuardianWriteRepo, GuardianWriteRepo>();

            //Logic
            services.AddTransient<IGuardianLogic, GuardianLogic>();
            services.AddTransient<GuardianValidator>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStatusCodePages();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SportsComplex API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseHttpsRedirection();
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = feature.Error;
                context.Response.StatusCode = exception is InvalidRequestException ? 400 : context.Response.StatusCode;
                var errorMessage = $"{exception.GetType()} - {exception.Message}";

                var response = GetJSendErrorResponse(exception, errorMessage);

                Log.Error(exception, errorMessage);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response);
            }));

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static string GetJSendErrorResponse(Exception ex, string errorMessage)
        {
            var status = ex is InvalidRequestException ? JSendStatus.Fail : JSendStatus.Error;
            var jSendResponse = new JSendResponse
            {
                Status = status,
                Message = errorMessage
            };
            return JsonSerializer.Serialize(jSendResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}