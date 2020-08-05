using System;
using System.Text;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace SqlToFlatFile
{
    class Program
    {
        private static Logger _serilogLogger;

        /* sample command line
        -w "ISO with Reseller Info" -c "Server=intreport1;Database=aprivapos;Trusted_Connection=True;" -q "..\..\..\TestSqlToFlatFile\IsoWithResellerQuery.sql" -o "test1.xlsx"
        */
        static void Main(string[] args)
        {
            //IConfiguration configuration = new ConfigurationBuilder()
            //    .AddEnvironmentVariables()
            //    .Build();

            _serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("SqlToFlatFile..log", rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Verbose,
                    encoding: Encoding.UTF8)
                .WriteTo.Debug()
                .CreateLogger();

            _serilogLogger.Information("App starting up.");

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //var logger = serviceProvider.GetService<ILogger<Program>>();  // way to get a logger via DI logger factory

            Options parsedOptions = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => parsedOptions = options);

            if (null == parsedOptions)
            {
                _serilogLogger.Fatal("Application could not parse the command line parameters provided");
                Environment.Exit(-1);
            }

            ApplicationLogic app = serviceProvider.GetService<ApplicationLogic>();
            // Start up logic here
            try
            {
                app.Run(parsedOptions);
            }
            catch (Exception ex)
            {
                _serilogLogger.Error(ex, "Exception occurred:");
                Environment.Exit(-1);
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                    loggingBuilder.AddSerilog(_serilogLogger);
                })
                .AddTransient<ApplicationLogic>();
        }
    }
}
