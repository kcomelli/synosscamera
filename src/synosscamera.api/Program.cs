using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using synosscamera.core.Extensions;

namespace synosscamera.api
{
    // <summary>
    /// Default entry point class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point 
        /// </summary>
        /// <param name="args">Arguments</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        /// <summary>
        /// Create web host
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>An <see cref="IWebHostBuilder"/> instance</returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // clear default config
                    config.Sources.Clear();

                    // allow OS dependent config overrides
                    var osEnvironmentSwitch = System.Environment.GetEnvironmentVariable("ASPNETCORE_OPERATINGSYSTEM");
                    // run location
                    var runLocation = System.Environment.GetEnvironmentVariable("ASPNETCORE_RUNAT");
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    // different environments
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    if (osEnvironmentSwitch.IsPresent())
                        // to switch environments based on OS
                        config.AddJsonFile($"appsettings.{env.EnvironmentName}.{osEnvironmentSwitch}.json", optional: true, reloadOnChange: true);

                    if (runLocation.IsPresent())
                    {
                        // to switch environments based on run location
                        config.AddJsonFile($"appsettings.{runLocation}.json", optional: true, reloadOnChange: true);
                        // to switch environments based on run location
                        config.AddJsonFile($"appsettings.{env.EnvironmentName}.{runLocation}.json", optional: true, reloadOnChange: true);
                        if (osEnvironmentSwitch.IsPresent())
                            // to switch environments based on run location and OS
                            config.AddJsonFile($"appsettings.{env.EnvironmentName}.{runLocation}.{osEnvironmentSwitch}.json", optional: true, reloadOnChange: true);
                    }

                    // to switch environments based on computer name
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.{System.Environment.MachineName}.json", optional: true);
                    // allow overwrite using kubernetes secrets
                    config.AddJsonFile("/app/secrets/appsettings.secrets.json", optional: true);
                    // allow overwriting settings via environment variables
                    config.AddEnvironmentVariables();

                    // overwrite settings via arguments
                    if (args != null)
                        config.AddCommandLine(args);

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        config.AddUserSecrets<Program>();
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddDebug();
                    logging.AddSerilog();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
