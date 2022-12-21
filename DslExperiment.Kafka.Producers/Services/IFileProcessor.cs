using Microsoft.Extensions.Logging;
using System.IO;

namespace DslExperiment.Kafka.Producers.Services
{
    public interface IFileProcessor
    {
        int Parse(Stream input, ILogger logger);

        void CleanUp(string uri, ILogger logger);
    }
}