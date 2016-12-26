using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WeatherMicroservice {
  public class Startup {
    // This method gets called by the runtime.
    // Use this method to add services to the container.
    // For more information on how to configure your application,
    // visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services) { }

    // This method gets called by the runtime.
    // Use this method to configure the HTTP request pipeline.
    public void Configure(
      IApplicationBuilder app,
      IHostingEnvironment env,
      ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole();

      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }

      app.Run(async (context) => {
        var lat = context.Request.Query["lat"].FirstOrDefault().TryParse();
        var lng = context.Request.Query["long"].FirstOrDefault().TryParse();

        if (lat.HasValue && lng.HasValue) {
          var forecast = new List<WeatherReport>();
          for (var days = 1; days < 6; days++) {
            forecast.Add(new WeatherReport(lat.Value, lng.Value, days));
          }

          await context.Response.WriteAsync(
            $"Weather forecast for lat: {lat}, long: {lng}\n" +
            $"  Today:\n" +
            $"    {forecast[0].Format()}\n" +
            $"  Tomorrow:\n" +
            $"    {forecast[1].Format()}\n" +
            $"  After tomorrow:\n" +
            $"    {forecast[2].Format()}\n" +
            $"  Three days from now:\n" +
            $"    {forecast[3].Format()}\n" +
            $"  Four days from now:\n" +
            $"    {forecast[4].Format()}\n"
          );
        }
      });
    }
  }

  public static class Extensions {
    public static double? TryParse(this string input) {
      double result;
      if (double.TryParse(input, out result)) {
        return result;
      } else {
        return default(double?);
      }
    }

    public static string Format(this WeatherReport report) {
      return (
        $"General condition: {report.Conditions}, " +
        $"Highest Temperature: {report.HiTemperature}, " +
        $"Lowest Temperature: {report.LoTemperature}, " +
        $"Average Wind Speed: {report.AverageWindSpeed}."
      );
    }
  }
}
