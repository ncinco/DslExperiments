namespace DslExperiment.Kafka.Producers
{
    public class GlobalConstants
    {
        public const string STORAGE_CONTAINER_IN = "dsl-transaction-category-in";
        public const string STORAGE_CONTAINER_PROCESSED = "dsl-transaction-category-processed";
        public const string PROCESSED_FILE_EXTENSION = ".processed.";
        public const string BUILD_VER_METADATA_PREFIX = "+build";
        public const string BUILD_VER_DATETIME_FORMATE = "yyyy-MM-ddTHH:mm:ss:fffZ";
    }
}