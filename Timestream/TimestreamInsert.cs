using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.TimestreamWrite;
using Amazon.TimestreamWrite.Model;

namespace Timestream
{
    public class TimestreamInsert
    {
        private readonly AmazonTimestreamWriteClient writeClient;
        private readonly AmazonTimestreamWriteConfig writeClientConfig;
        private readonly string databaseName = "telemetryDB";
        private readonly string tableName = "event-data";
        
        public TimestreamInsert()
        {
            writeClientConfig = new AmazonTimestreamWriteConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2,
                MaxErrorRetry = 10
            };
            writeClient = new AmazonTimestreamWriteClient(writeClientConfig);
        }

        public async Task Main()
        {
            try
            {
                await WriteRecords();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task WriteRecords()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string currentTimeString = (now.ToUnixTimeMilliseconds()).ToString();

            List<Dimension> dimensions = new List<Dimension>
            {
                new Dimension { Name = "eventType", Value = "Test-2" },
                new Dimension { Name = "source", Value = "Source" },
                new Dimension { Name = "eventData", Value = "Testing Data" }

            };
            var eventData = new Record
            {
                Dimensions = dimensions,
                MeasureName = "counter",
                MeasureValue = "1",
                MeasureValueType = MeasureValueType.BIGINT,
                Time = currentTimeString,
                TimeUnit = TimeUnit.MILLISECONDS
            };


            List<Record> records = new List<Record>();
            records.Add(eventData);

            try
            {
                var writeRecordsRequest = new WriteRecordsRequest
                {
                    DatabaseName = databaseName,
                    TableName = tableName,
                    Records = records
                };
                WriteRecordsResponse response = await writeClient.WriteRecordsAsync(writeRecordsRequest);
                Console.WriteLine($"Write records status code: {response.HttpStatusCode.ToString()}");
            }
            catch (RejectedRecordsException e)
            {
                PrintRejectedRecordsException(e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Write records failure:" + e.ToString());
            }
        }


        private void PrintRejectedRecordsException(RejectedRecordsException e)
        {
            Console.WriteLine("RejectedRecordsException:" + e.ToString());
            foreach (RejectedRecord rr in e.RejectedRecords)
            {
                Console.WriteLine("RecordIndex " + rr.RecordIndex + " : " + rr.Reason);
                long? existingVersion = rr.ExistingVersion;
                if (existingVersion != null)
                {
                    Console.WriteLine("Rejected record existing version: " + existingVersion);
                }
            }
        }
    }
}