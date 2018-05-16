using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestApplicationWithMongoBackend.Data;
using RestApplicationWithMongoBackend.Middleware;
using RestApplicationWithMongoBackend.Models;
using RestApplicationWithMongoBackend.Repository;

namespace RestApplicationWithMongoBackend
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
      services.AddCors(options =>
      {
        options.AddPolicy("CorsPolicy",
                  builder => builder.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
      });
      services.AddMvc();
      services.Configure<Settings>(option =>
      {
        option.ConnectionString = Environment.GetEnvironmentVariable("MongoConnection:ConnectionString") ??
                                        Configuration.GetSection("MongoConnection:ConnectionString").Value;
        option.Database = Environment.GetEnvironmentVariable("MongoConnection:Database") ??
                                        Configuration.GetSection("MongoConnection:Database").Value;
      });

      services.AddLogging(builder =>
      {
        builder.SetMinimumLevel(GetLogLevelFromEnvioment(LogLevel.Information));
      });

      services.AddSingleton<IKeyValueRepository, KeyValueRepository>();
      services.AddSingleton<IBackDoor, BackDoor>();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      app.UseMiddleware<RequestMiddleware>();
      app.UseMvc();
    }

    private static LogLevel GetLogLevelFromEnvioment(LogLevel defaultlogLevel)
    {
      string loglevelStr = Environment.GetEnvironmentVariable("Logging:LogLevel") ?? defaultlogLevel.ToString();
      LogLevel logLevel;
      if (!Enum.TryParse(loglevelStr, out logLevel))
      {
        logLevel = defaultlogLevel;
      }
      return logLevel;
    }
  }
}
