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
        private readonly string databaseName;
        private readonly string tableName;
        private readonly string filePath;
        public TimestreamInsert()
        {
            writeClientConfig = new AmazonTimestreamWriteConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2,
                MaxErrorRetry = 10
            };
            writeClient = new AmazonTimestreamWriteClient(writeClientConfig);
            databaseName = Constants.databaseName;
            tableName = Constants.tableName;
            filePath = Constants.filePath;
        }

        public async Task Main()
        {
            try
            {
                // await WriteRecords();
                await BulkWriteRecordsMultiMeasure();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task WriteRecords()
        {
            DateTimeOffset now = new DateTimeOffset(new DateTime(2022, 01, 15, 12, 00, 00));
            
            string currentTimeString = (now.ToUnixTimeMilliseconds()).ToString();

            List<Dimension> dimensions = new List<Dimension>
            {
                new Dimension { Name = "eventType", Value = "Resolve" },
                new Dimension { Name = "source", Value = "sds-xref-virtual-agent" },
                new Dimension { Name = "eventData", Value = "{ProductName:P1620 OPI AVOJUICE GINGER LILY,ManufacturerName:HFC PRESTIGE INTL CANADA INC-JDA CA,MfrPartNumber:,WorkItemId:1668744,ProductId:3587270,UpcFound:false,Upc:,Reason:resolve,ClientRequestId:20253600,ClientProductId:23448186,SystemId:30,DecisionComment:Predicted from Model2,ScorePrediction:0.0,ProductPrediction:3587270}" }
            };
            var record = new Record
            {
                Dimensions = dimensions,
                MeasureName = "counter",
                MeasureValue = "1",
                MeasureValueType = MeasureValueType.BIGINT,
                Time = currentTimeString,
                TimeUnit = TimeUnit.MILLISECONDS
            };

            // var record = new Record
            //     {
            //         Dimensions = dimensions,
            //         MeasureName = "telemetry_1",                   
            //         MeasureValues = new List<MeasureValue>{
            //                         new MeasureValue{
            //                             Name= "counter",
            //                             Value= "1",
            //                             Type= "BIGINT",
            //                         },
            //                         new MeasureValue{
            //                             Name= "eventData",
            //                             Value= "xxXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXxxXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            //                             Type= "VARCHAR",
            //                         }
            //                     },
            //         MeasureValueType = MeasureValueType.BIGINT,
            //         Time = currentTimeString,
            //         TimeUnit = TimeUnit.MILLISECONDS
            //     };
            // var record = new Record
            // {
            //     Dimensions = dimensions,
            //     MeasureName = "source",
            //     MeasureValue = data.sourceName,
            //     MeasureValues = new List<MeasureValue>{
            //                     new MeasureValue{
            //                         Name= "counter",
            //                         Value= "1",
            //                         Type= "BIGINT",
            //                     },
            //                     new MeasureValue{
            //                         Name= "eventData",
            //                         Value= data.eventData,
            //                         Type= "VARCHAR",
            //                     }
            //                 },
            //     MeasureValueType = MeasureValueType.MULTI,
            //     Time = DateTimeOffset.Parse(data.eventDate).ToUnixTimeMilliseconds().ToString(),
            //     TimeUnit = TimeUnit.MILLISECONDS
            // };


            List<Record> records = new List<Record>();
            records.Add(record);

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

            DateTimeOffset now = DateTimeOffset.UtcNow;
            long currentTime = now.ToUnixTimeMilliseconds();

            foreach (string line in File.ReadLines(filePath))
            {
                string[] columns = line.Split('^');
                if (columns.Count() > 5)
                    continue;

                var data = new EventData()
                {
                    sourceName = columns[0],
                    eventType = columns[1],
                    eventData = (columns[2]),
                    eventDate = columns[3],
                    counter = int.Parse(columns[4])
                };

                List<Dimension> dimensions = new List<Dimension> {
                    new Dimension { Name = "source", Value = data.sourceName },
                    new Dimension { Name = "eventType", Value = data.eventType },
                    new Dimension { Name = "eventData", Value = data.eventData }
                };

                // long recordTime = currentTime - counter * 50;
                long recordTime = currentTime - counter * 50;

                var record = new Record
                {
                    Dimensions = dimensions,
                    MeasureName = "counter",
                    MeasureValue = data.counter.ToString(),
                    MeasureValueType = MeasureValueType.BIGINT,
                    Time = DateTimeOffset.Parse(data.eventDate).ToUnixTimeMilliseconds().ToString(),
                    TimeUnit = TimeUnit.MILLISECONDS
                };

                counter++;
                records.Add(record);
                await SubmitBatchAsync(records, counter);
                records.Clear();

                // // when the batch hits the max size, submit the batch
                // records.Add(record);
                // if (records.Count == 100)
                // {
                //     await SubmitBatchAsync(records, counter);
                //     records.Clear();
                // }
            }

            if (records.Count != 0)
            {
                await SubmitBatchAsync(records, counter);
            }
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