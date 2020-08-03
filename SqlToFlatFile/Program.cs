using System;
using System.Data.SqlClient;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SqlToFlatFileLib;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SqlToFlatFile
{
    class Program
    {
        /* sample command line
        -w "ISO with Reseller Info" -c "Server=intreport1;Database=aprivapos;Trusted_Connection=True;" -q "..\..\..\TestSqlToFlatFile\IsoWithResellerQuery.sql" -o "test1.xlsx"
        */
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("file.txt")
                .CreateLogger();

            var services = new ServiceCollection();
            ConfigureServices(services);

            Options parsedOptions = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => parsedOptions = options);

            if (null == parsedOptions)
            {
                log.Fatal("Application could not parse the command line parameters provided");
                Environment.Exit(-1);
            }

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                ApplicationLogic app = serviceProvider.GetService<ApplicationLogic>();
                // Start up logic here
                try
                {
                    app.Run(parsedOptions);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Exception occurred:");
                    Environment.Exit(-1);
                }
            }


        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging(configure => configure.AddSerilog())
                .AddTransient<ApplicationLogic>();
        }
    }
}
