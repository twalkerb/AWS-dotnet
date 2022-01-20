using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.TimestreamWrite;
using Amazon.TimestreamWrite.Model;

namespace Timestream
{
    public class TimestreamTable
    {
        private readonly AmazonTimestreamWriteClient writeClient;
        private readonly AmazonTimestreamWriteConfig writeClientConfig;
        private readonly string databaseName;
        private readonly string s3ErrorReportBucketName;
        private readonly string tableName;
        public TimestreamTable()
        {
            writeClientConfig = new AmazonTimestreamWriteConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2,
                MaxErrorRetry = 10
            };
            writeClient = new AmazonTimestreamWriteClient(writeClientConfig);
            databaseName = Constants.databaseName;
            tableName = Constants.tableName;
            s3ErrorReportBucketName = Constants.s3ErrorReportBucketName;
        }

        public async Task Main()
        {
            try
            {
                await CreateTable();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task CreateTable()
        {
            Console.WriteLine($"Creating Table {tableName}");

            try
            {
                var createTableRequest = new CreateTableRequest
                {
                    DatabaseName = databaseName,
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
                        }
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
    }
}