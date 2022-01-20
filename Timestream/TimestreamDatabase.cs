using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.TimestreamWrite;
using Amazon.TimestreamWrite.Model;

namespace Timestream
{
    public class TimestreamDatabase
    {
        private readonly AmazonTimestreamWriteClient writeClient;
        private readonly AmazonTimestreamWriteConfig writeClientConfig;
        private readonly string databaseName;
        
        public TimestreamDatabase()
        {
            writeClientConfig = new AmazonTimestreamWriteConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2,
                MaxErrorRetry = 10
            };
            writeClient = new AmazonTimestreamWriteClient(writeClientConfig);
            databaseName = Constants.databaseName;
        }

        public async Task Main()
        {
            try
            {
                await CreateDatabase();                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task CreateDatabase()
        {
            Console.WriteLine($"---Creating Database {databaseName}---");

            try
            {
                var createDatabaseRequest = new CreateDatabaseRequest
                {
                    DatabaseName = databaseName
                };
                await writeClient.CreateDatabaseAsync(createDatabaseRequest);
                Console.WriteLine($"Database {databaseName} created");
            }
            catch (ConflictException)
            {
                Console.WriteLine("Database already exists.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"---Create database failed: {e}---");
            }
        }
    }
}