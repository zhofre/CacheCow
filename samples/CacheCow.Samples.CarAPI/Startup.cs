﻿using AutoMapper;
using CacheCow.Server.Core.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace CacheCow.Samples.CarAPI
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // configure mvc
            services
                .AddMvc(setupAction =>
                {
                    setupAction.ReturnHttpNotAcceptable = true;
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                });

            // caching support
            services.AddHttpCachingMvc();
            services.AddResponseCaching();

            // automapper
            services.AddSingleton(_ => new MapperConfiguration(ConfigureMapping).CreateMapper());
        }

        private void ConfigureMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Entities.Car, Dto.Car>();
            cfg.CreateMap<Dto.CarForCreation, Entities.Car>();
            cfg.CreateMap<Dto.CarForManipulation, Entities.Car>();
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }
            app.UseResponseCaching();
            app.UseMvc();
        }
    }
}
