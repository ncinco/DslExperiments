using Confluent.Kafka;
using DslExperiment.Kafka.Producers.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace DslExperiment.Kafka.Producers.Services
{
    public class TransactionCategoryEventProducer : ITransactionCategoryEventProducer
    {

        private readonly ProducerConfig producerConfig;
        private readonly string topic;
        public TransactionCategoryEventProducer()
        {
            topic = "dsl-transaction-category";
            producerConfig = new ProducerConfig
            {
                BootstrapServers = "pkc-4n66v.australiaeast.azure.confluent.cloud:9092", //Environment.GetEnvironmentVariable("kafka.bootstrap.servers"),
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = "MJSR5BSO4BT5CBY7",
                SaslPassword = "kao8mKdbviSmGPGXnt6vNs7nifcbXZy44LWrh7CL18K6hK6UuGGnYKVsW8PykTkA",
                EnableIdempotence = true,
                BatchSize = 1000000,
                LingerMs = 5
            };
        }

        /// <summary>
        /// This method produce event to the topic and flush right away.
        /// <example>
        public void PublishToStream(TransactionCategoryKafkaModel kakfkaModel, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Producing to Kafka using BatchSize in bytes: {producerConfig.BatchSize}");
                logger.LogInformation($"Producing to Kafka using LingerMs in bytes: {producerConfig.LingerMs}");
                if (kakfkaModel != null)
                {
                    using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
                    {
                        publishEvent(producer, kakfkaModel, logger);
                        producer.Flush();
                    }
                }
                else
                {
                    logger.LogInformation("CifData is null, nothing to produce..");
                }

            }
            catch (Exception e)
            {
                logger.LogError($"Exception during producing event: {e.Message}");
                throw;
            }

        }
        /// <summary>
        /// This method produce all events to the topic before flushing.
        /// <example>
        public void PublishToStream(TransactionCategoryKafkaModel[] kakfkaModels, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Producing to Kafka using BatchSize in bytes: {producerConfig.BatchSize}");
                logger.LogInformation($"Producing to Kafka using LingerMs in bytes: {producerConfig.LingerMs}");
                if (kakfkaModels != null)
                {
                    using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
                    {
                        // send all events to producer before flushing
                        foreach (var cifData in kakfkaModels)
                        {
                            publishEvent(producer, cifData, logger);
                        }
                        producer.Flush();
                    }
                }
                else
                {
                    logger.LogInformation("CifDataArray is null, nothing to produce..");
                }

            }
            catch (Exception e)
            {
                logger.LogError($"Exception during producing event: {e.Message}");
                throw;
            }
        }

        private void publishEvent(IProducer<string, string> producer, TransactionCategoryKafkaModel kafkaModel, ILogger logger)
        {
            try
            {
                producer.Produce(topic, new Message<string, string> { Key = kafkaModel.TransactionId, Value = JsonSerializer.Serialize(kafkaModel) },
                 (deliveryReport) =>
                 {
                     if (deliveryReport.Error.Code != ErrorCode.NoError)
                     {
                         logger.LogError($"Failed to deliver message: {deliveryReport.Error.Reason}");
                     }
                     else
                     {
                         logger.LogInformation($"Produced event to topic: key = {kafkaModel.TransactionId}");
                     }
                 });
            }
            catch (Exception e)
            {
                logger.LogError($"Exception raised during producing an event with key: {kafkaModel.TransactionId}, exception {e.Message}");
            }
        }
    }
}