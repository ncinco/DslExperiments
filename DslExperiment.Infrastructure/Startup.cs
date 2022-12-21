using Cassandra;
using Cassandra.Mapping;
using DslExperiment.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

[assembly: FunctionsStartup(typeof(DslExperiment.ViewModels.Startup))]

namespace DslExperiment.ViewModels
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            var logger = new LoggerFactory().CreateLogger< Startup>();

            var runningDirectory = GetRunningDirectory();
            var parentDirectory = Directory.GetParent(runningDirectory);
            var connectionZipLocation = $"{parentDirectory.FullName}\\secure-connect-transactions.zip";

            logger.LogInformation($"Connection zip location: {connectionZipLocation}");

            var session = Cluster.Builder()
                .WithCloudSecureConnectionBundle(connectionZipLocation)
                .WithCredentials("AZjhNqTMgAilAZrCDBGejmtg", "ed2hryN3vtrxJScvLocdFNncCSWAfIUur6tDCc3dMtdETdSQzApAjbCaJPuUiPL0r2y12_J.0aIEKA.73FH8I2qUrqynMZki45n_8HZ3n56H1aOjzJeCUBGgwL3GB2LA")
                .Build()
                .Connect();

            MappingConfiguration.Global.Define<DomainMappings>();
            var mapper = new Mapper(session);

            builder.Services.AddSingleton<IMapper>(mapper);
            builder.Services.AddSingleton(session);
        }

        private string GetRunningDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}