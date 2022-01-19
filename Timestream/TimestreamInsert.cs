using System;
using System.IO;
using System.Linq;
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
        private readonly string csvFilePath = "~DataFile.csv";
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
                await BulkWriteRecordsMultiMeasure();
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

        public async Task BulkWriteRecordsMultiMeasure()
        {
            List<Record> records = new List<Record>();
            int counter = 0;

            List<Task> writetasks = new List<Task>();

            DateTimeOffset now = DateTimeOffset.UtcNow;
            long currentTime = now.ToUnixTimeMilliseconds();

            foreach (string line in File.ReadLines(csvFilePath))
            {
                string[] columns = line.Split(',');
                var data = new EventData()
                {
                    sourceName = columns[0],
                    eventType = columns[1],
                    eventData = $"{columns[2]}{columns[3]}",
                    eventDate = Convert.ToDateTime(columns[4]),
                    counter = int.Parse(columns[5])
                };

                List<Dimension> dimensions = new List<Dimension> {
                    new Dimension { Name = "source", Value = data.sourceName },
                    new Dimension { Name = "eventType", Value = data.eventType },
                    new Dimension { Name = "eventData", Value = data.eventData }
                };

                long recordTime = currentTime - counter * 50;

                var record = new Record
                {
                    Dimensions = dimensions,
                    MeasureName = "counter",
                    MeasureValue = "1",
                    MeasureValueType = MeasureValueType.BIGINT,
                    Time = recordTime.ToString(),
                    TimeUnit = TimeUnit.MILLISECONDS
                };

                records.Add(record);
                counter++;

                // when the batch hits the max size, submit the batch
                if (records.Count == 100)
                {
                    writetasks.Add(SubmitBatchAsync(records, counter));
                    records.Clear();
                }
            }

            if (records.Count != 0)
            {
                writetasks.Add(SubmitBatchAsync(records, counter));
            }

            await Task.WhenAll(writetasks.ToArray());

            Console.WriteLine($"Ingested {counter} records.");
        }

        private List<MeasureValue> GetMeasureValues(string[] columns)
        {
            List<MeasureValue> measureValues = new List<MeasureValue>();
            for (int i = 0; i < columns.Length; i += 3)
            {
                measureValues.Add(
                    new MeasureValue
                    {
                        Name = columns[i],
                        Value = columns[i + 1],
                        Type = columns[i + 2],
                    }
                );
            }
            return measureValues;
        }


        private async Task SubmitBatchAsync(List<Record> records, int counter)
        {
            try
            {
                var writeRecordsRequest = new WriteRecordsRequest
                {
                    DatabaseName = databaseName,
                    TableName = tableName,
                    Records = records
                };
                WriteRecordsResponse response = await writeClient.WriteRecordsAsync(writeRecordsRequest);
                Console.WriteLine($"Processed {counter} records. Write records status code:{response.HttpStatusCode.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Write records failure:" + e.ToString());
            }
        }

        private long DateTimeToTimestamp(DateTime dateTime) =>
            new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

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