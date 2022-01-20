using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.TimestreamWrite;
using Amazon.TimestreamWrite.Model;

namespace Timestream
{
    public class SampleTimestream
    {
        private readonly AmazonTimestreamWriteClient writeClient;
        private readonly AmazonTimestreamWriteConfig writeClientConfig;
        private readonly string DATABASE_NAME = "sampleDB";
        private readonly string TABLE_NAME = "sample-table";
        public const string MultiMeasureValueSampleDb = "multiMeasureValueSampleDb";
        public const string MultiMeasureValueSampleTable = "multiMeasureValueSampleTable";
        private const string s3ErrorReportBucketName = "my-learners-bucket";
        private const string filePath = "~sample-multi.csv";
        private const bool skipDeletion = false;

        public SampleTimestream()
        {
            writeClientConfig = new AmazonTimestreamWriteConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2,
                Timeout = TimeSpan.FromSeconds(10000),
                MaxErrorRetry = 10
            };
            writeClient = new AmazonTimestreamWriteClient(writeClientConfig);
        }
        public async Task MainAsync()
        {
            // await CreateDatabase(DATABASE_NAME);
            // await DescribeDatabase(DATABASE_NAME);
            // await ListDatabases();

            // await CreateTable(DATABASE_NAME,TABLE_NAME);
            // await DescribeTable(DATABASE_NAME,TABLE_NAME);
            // await ListTables(DATABASE_NAME);
            // await UpdateTable(DATABASE_NAME,TABLE_NAME);

            // Simple records ingestion
            await WriteRecordsMultiMeasure(DATABASE_NAME,TABLE_NAME);
            // await WriteRecordsWithCommonAttributes(DATABASE_NAME,TABLE_NAME);

            // write multi value records
            // await CreateDatabase(MultiMeasureValueSampleDb);
            // await CreateTable(MultiMeasureValueSampleDb, MultiMeasureValueSampleTable);
            // await WriteRecordsMultiMeasureValueSingleRecord(MultiMeasureValueSampleDb, MultiMeasureValueSampleTable);
            // await WriteRecordsMultiMeasureValueMultipleRecords(MultiMeasureValueSampleDb, MultiMeasureValueSampleTable);

            // if (filePath != null)            
            //     await BulkWriteRecordsMultiMeasure();
            
        }

        private async Task CreateDatabase(string dbName)
        {
            Console.WriteLine($"Creating Database {dbName}");
            try
            {
                var createDatabaseRequest = new CreateDatabaseRequest
                {
                    DatabaseName = dbName
                };
                await writeClient.CreateDatabaseAsync(createDatabaseRequest);
                Console.WriteLine($"Database {dbName} created");
            }
            catch (ConflictException)
            {
                Console.WriteLine("Database already exists.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Create database failed: {e}");
            }
        }

        private async Task DescribeDatabase(string dbName)
        {
            Console.WriteLine("Describing Database");

            try
            {
                var describeDatabaseRequest = new DescribeDatabaseRequest
                {
                    DatabaseName = dbName
                };
                DescribeDatabaseResponse response = await writeClient.DescribeDatabaseAsync(describeDatabaseRequest);
                Console.WriteLine($"Database {dbName} has id:{response.Database.Arn}");
            }
            catch (ResourceNotFoundException)
            {
                Console.WriteLine("Database does not exist.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Describe database failed:" + e.ToString());
            }
        }

        private async Task ListDatabases()
        {
            Console.WriteLine("Listing Databases");

            try
            {
                var listDatabasesRequest = new ListDatabasesRequest
                {
                    MaxResults = 5
                };
                ListDatabasesResponse response = await writeClient.ListDatabasesAsync(listDatabasesRequest);
                PrintDatabases(response.Databases);
                var nextToken = response.NextToken;
                while (nextToken != null)
                {
                    listDatabasesRequest.NextToken = nextToken;
                    response = await writeClient.ListDatabasesAsync(listDatabasesRequest);
                    PrintDatabases(response.Databases);
                    nextToken = response.NextToken;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("List database failed:" + e.ToString());
            }
        }

        private async Task CreateTable(string dbName, string tableName)
        {
            Console.WriteLine($"Creating Table {tableName}");
            try
            {
                var createTableRequest = new CreateTableRequest
                {
                    DatabaseName = dbName,
                    TableName = tableName,
                    RetentionProperties = new RetentionProperties
                    {
                        MagneticStoreRetentionPeriodInDays = Constants.CT_TTL_DAYS,
                        MemoryStoreRetentionPeriodInHours = Constants.HT_TTL_HOURS
                    },
                    // Enable MagneticStoreWrite
                    MagneticStoreWriteProperties = new MagneticStoreWriteProperties
                    {
                        EnableMagneticStoreWrites = true,
                        // Persist MagneticStoreWrite rejected records in S3
                        MagneticStoreRejectedDataLocation = new MagneticStoreRejectedDataLocation
                        {
                            S3Configuration = new S3Configuration
                            {
                                BucketName = s3ErrorReportBucketName,
                                EncryptionOption = "SSE_S3",
                            },
                        },
                    }
                };
                await writeClient.CreateTableAsync(createTableRequest);
                Console.WriteLine($"Table {tableName} created");
            }
            catch (ConflictException)
            {
                Console.WriteLine("Table already exists.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Create table failed: {e}");
            }
        }

        private async Task DescribeTable(string dbName, string tableName)
        {
            Console.WriteLine("Describing Table");

            try
            {
                var describeTableRequest = new DescribeTableRequest
                {
                    DatabaseName = dbName,
                    TableName = tableName,
                };
                DescribeTableResponse response = await writeClient.DescribeTableAsync(describeTableRequest);
                Console.WriteLine($"Table {tableName} has id:{response.Table.Arn}");
            }
            catch (ResourceNotFoundException)
            {
                Console.WriteLine("Table does not exist.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Describe table failed:" + e.ToString());
            }
        }

        private async Task ListTables(string dbName)
        {
            Console.WriteLine("Listing Tables");

            try
            {
                var listTablesRequest = new ListTablesRequest
                {
                    MaxResults = 5,
                    DatabaseName = dbName
                };
                ListTablesResponse response = await writeClient.ListTablesAsync(listTablesRequest);
                PrintTables(response.Tables);
                string nextToken = response.NextToken;
                while (nextToken != null)
                {
                    listTablesRequest.NextToken = nextToken;
                    response = await writeClient.ListTablesAsync(listTablesRequest);
                    PrintTables(response.Tables);
                    nextToken = response.NextToken;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("List table failed:" + e.ToString());
            }
        }

        public async Task UpdateTable(string dbName, string tableName)
        {
            Console.WriteLine("Updating Table");

            try
            {
                var updateTableRequest = new UpdateTableRequest
                {
                    DatabaseName = dbName,
                    TableName = tableName,
                    RetentionProperties = new RetentionProperties
                    {
                        MagneticStoreRetentionPeriodInDays = Constants.CT_TTL_DAYS + 1,
                        MemoryStoreRetentionPeriodInHours = Constants.HT_TTL_HOURS + 2
                    }
                };
                UpdateTableResponse response = await writeClient.UpdateTableAsync(updateTableRequest);
                Console.WriteLine($"Table {tableName} updated");
            }
            catch (ResourceNotFoundException)
            {
                Console.WriteLine("Table does not exist.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Update table failed:" + e.ToString());
            }
        }

        private async Task WriteRecordsMultiMeasure(string dbName, string tableName)
        {
            Console.WriteLine("Writing records");

            DateTimeOffset now = DateTimeOffset.UtcNow;
            string currentTimeString = (now.ToUnixTimeMilliseconds()).ToString();

            List<Dimension> dimensions = new List<Dimension>{
                new Dimension { Name = "region", Value = "us-east-1" },
                new Dimension { Name = "az", Value = "az1" },
                new Dimension { Name = "hostname", Value = "host1" }
            };

            var cpuUtilization = new Record
            {
                Dimensions = dimensions,
                MeasureName = "cpu_utilization",
                MeasureValues = new List<MeasureValue>{
                                    new MeasureValue{
                                        Name= "cpu1",
                                        Value= "13.5",
                                        Type= "DOUBLE",
                                    },
                                    new MeasureValue{
                                        Name= "cpu2",
                                        Value= "15",
                                        Type= "DOUBLE",
                                    }
                                },
                MeasureValueType = MeasureValueType.MULTI,
                Time = currentTimeString
            };

            var memoryUtilization = new Record
            {
                Dimensions = dimensions,
                MeasureName = "memory_utilization",
                MeasureValues = new List<MeasureValue>{
                                    new MeasureValue{
                                        Name= "memory1",
                                        Value= "40",
                                        Type= "DOUBLE",
                                    },
                                    new MeasureValue{
                                        Name= "memory2",
                                        Value= "60",
                                        Type= "DOUBLE",
                                    }
                                },
                MeasureValueType = MeasureValueType.MULTI,
                Time = currentTimeString
            };


            List<Record> records = new List<Record> {
                cpuUtilization,
                memoryUtilization
            };

            try
            {
                var writeRecordsRequest = new WriteRecordsRequest
                {
                    DatabaseName = dbName,
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

        private async Task WriteRecordsWithCommonAttributes(string dbName, string tableName)
        {
            Console.WriteLine("Writing records with common attributes");

            DateTimeOffset now = DateTimeOffset.UtcNow;
            string currentTimeString = (now.ToUnixTimeMilliseconds()).ToString();

            List<Dimension> dimensions = new List<Dimension>{
                new Dimension { Name = "region", Value = "us-east-1" },
                new Dimension { Name = "az", Value = "az1" },
                new Dimension { Name = "hostname", Value = "host1" }
            };

            var commonAttributes = new Record
            {
                Dimensions = dimensions,
                MeasureValueType = MeasureValueType.MULTI,
                Time = currentTimeString
            };

            var cpuUtilization = new Record
            {
                MeasureName = "cpu_utilization",
                MeasureValues = new List<MeasureValue>{
                                                new MeasureValue{
                                                    Name= "cpu1",
                                                    Value= "13.5",
                                                    Type= "DOUBLE",
                                                }
                                            },
            };

            var memoryUtilization = new Record
            {
                MeasureName = "memory_utilization",
                MeasureValues = new List<MeasureValue>{
                                                new MeasureValue{
                                                    Name= "memory1",
                                                    Value= "40",
                                                    Type= "DOUBLE",
                                                }
                                            }
            };


            List<Record> records = new List<Record>();
            records.Add(cpuUtilization);
            records.Add(memoryUtilization);

            try
            {
                var writeRecordsRequest = new WriteRecordsRequest
                {
                    DatabaseName = dbName,
                    TableName = tableName,
                    Records = records,
                    CommonAttributes = commonAttributes
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

        private async Task WriteRecordsMultiMeasureValueSingleRecord(string dbName, string tableName)
        {
            Console.WriteLine("Writing records with multi value attributes");

            DateTimeOffset now = DateTimeOffset.UtcNow;
            string currentTimeString = (now.ToUnixTimeMilliseconds()).ToString();

            List<Dimension> dimensions = new List<Dimension>{
                new Dimension { Name = "region", Value = "us-east-1" },
                new Dimension { Name = "az", Value = "az1" },
                new Dimension { Name = "hostname", Value = "host1" }
            };

            var commonAttributes = new Record
            {
                Dimensions = dimensions,
                Time = currentTimeString
            };

            var cpuUtilization = new MeasureValue
            {
                Name = "cpu_utilization",
                Value = "13.6",
                Type = "DOUBLE"
            };

            var memoryUtilization = new MeasureValue
            {
                Name = "memory_utilization",
                Value = "40",
                Type = "DOUBLE"
            };

            var computationalRecord = new Record
            {
                MeasureName = "cpu_memory",
                MeasureValues = new List<MeasureValue> {cpuUtilization, memoryUtilization},
                MeasureValueType = "MULTI"
            };


            List<Record> records = new List<Record>();
            records.Add(computationalRecord);

            try
            {
                var writeRecordsRequest = new WriteRecordsRequest
                {
                    DatabaseName = dbName,
                    TableName = tableName,
                    Records = records,
                    CommonAttributes = commonAttributes
                };
                WriteRecordsResponse response = await writeClient.WriteRecordsAsync(writeRecordsRequest);
                Console.WriteLine($"Write records status code: {response.HttpStatusCode.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Write records failure:" + e.ToString());
            }
        }

        private async Task WriteRecordsMultiMeasureValueMultipleRecords(string dbName, string tableName)
        {
            Console.WriteLine("Writing records with multi value attributes mixture type");

            DateTimeOffset now = DateTimeOffset.UtcNow;
            string currentTimeString = (now.ToUnixTimeMilliseconds()).ToString();

            List<Dimension> dimensions = new List<Dimension>{
                new Dimension { Name = "region", Value = "us-east-1" },
                new Dimension { Name = "az", Value = "az1" },
                new Dimension { Name = "hostname", Value = "host1" }
            };

            var commonAttributes = new Record
            {
                Dimensions = dimensions,
                Time = currentTimeString
            };

            var cpuUtilization = new MeasureValue
            {
                Name = "cpu_utilization",
                Value = "13.6",
                Type = "DOUBLE"
            };

            var memoryUtilization = new MeasureValue
            {
                Name = "memory_utilization",
                Value = "40",
                Type = "DOUBLE"
            };

            var activeCores = new MeasureValue
            {
                Name = "active_cores",
                Value = "4",
                Type = "BIGINT"
            };

            var computationalRecord = new Record
            {
                MeasureName = "computational_utilization",
                MeasureValues = new List<MeasureValue> {cpuUtilization, memoryUtilization, activeCores},
                MeasureValueType = "MULTI"
            };

            var aliveRecord = new Record
            {
                MeasureName = "is_healthy",
                MeasureValue = "true",
                MeasureValueType = "BOOLEAN"
            };

            List<Record> records = new List<Record>();
            records.Add(computationalRecord);
            records.Add(aliveRecord);

            try
            {
                var writeRecordsRequest = new WriteRecordsRequest
                {
                    DatabaseName = dbName,
                    TableName = tableName,
                    Records = records,
                    CommonAttributes = commonAttributes
                };
                WriteRecordsResponse response = await writeClient.WriteRecordsAsync(writeRecordsRequest);
                Console.WriteLine($"Write records status code: {response.HttpStatusCode.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Write records failure:" + e.ToString());
            }
        }

        private async Task BulkWriteRecordsMultiMeasure()
        {
            List<Record> records = new List<Record>();
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int counter = 0;

            List<Task> writetasks = new List<Task>();

            foreach (string line in File.ReadLines(filePath))
            {
                string[] columns = line.Split(',');

                List<Dimension> dimensions = new List<Dimension> {
                    new Dimension { Name = columns[0], Value = columns[1] },
                    new Dimension { Name = columns[2], Value = columns[3] },
                    new Dimension { Name = columns[4], Value = columns[5] }
                };

                long recordTime = currentTime - counter * 50;

                var record = new Record {
                    Dimensions = dimensions,
                    MeasureName = "metrics",
                    MeasureValues = GetMeasureValues(columns.Skip(8).ToArray()),
                    MeasureValueType = MeasureValueType.MULTI,
                    Time = recordTime.ToString()
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

            if(records.Count != 0)
            {
                writetasks.Add(SubmitBatchAsync(records, counter));
            }

            await Task.WhenAll(writetasks.ToArray());

            Console.WriteLine($"Ingested {counter} records.");
        }

        private List<MeasureValue> GetMeasureValues(string[] columns)
        {
            List<MeasureValue> measureValues = new List<MeasureValue>();
            for(int i = 0; i < columns.Length; i += 3)
            {
                measureValues.Add(
                    new MeasureValue
                    {
                        Name= columns[i],
                        Value= columns[i+1],
                        Type= columns[i+2],
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
                    DatabaseName = DATABASE_NAME,
                    TableName = TABLE_NAME,
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

        private void PrintTables(List<Table> tables)
        {
            foreach (Table table in tables)
                Console.WriteLine($"Table: {table.TableName}");
        }

        private void PrintDatabases(List<Database> databases)
        {
            foreach (Database database in databases)
                Console.WriteLine($"Database:{database.DatabaseName}");
        }
    }
}