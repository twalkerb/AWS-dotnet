using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon;
using Amazon.Athena;
using Amazon.Athena.Model;

namespace AthenaApp
{
    public class AthenaQuery
    {
        private const string ATHENA_QUERY_OUTPUT_PATH = "s3://my-athena-result-sets/";
        private const string ATHENA_DB = "telemetry";
        private readonly AmazonAthenaClient client;
        private readonly QueryExecutionContext context;
        private readonly ResultConfiguration resultConfiguration;

        public AthenaQuery()
        {
            client = new AmazonAthenaClient
            (
                Amazon.RegionEndpoint.USEast2
            );
            context = new QueryExecutionContext()
            {
                Database = ATHENA_DB
            };
            resultConfiguration = new ResultConfiguration()
            {
                OutputLocation = ATHENA_QUERY_OUTPUT_PATH
            };
        }

        public async Task RunSampleQuery()
        {
            try
            {
                Console.WriteLine("Running Sample Query");
                await QueryTable();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
            await Task.CompletedTask;
        }

        private async Task QueryTable()
        {
            Console.WriteLine("---Querying Table---");
            try
            {
                var request = new StartQueryExecutionRequest();
                request.QueryString = "Select * from events";
                request.QueryExecutionContext = context;
                request.ResultConfiguration = resultConfiguration;
                request.WorkGroup = "primary";

                var response = await client.StartQueryExecutionAsync(request);
                List<Dictionary<string, string>> queryResults = await GetQueryResults(response.QueryExecutionId);

                var events = new List<Models.Events>();
                foreach (var result in queryResults)
                {
                    var res = new Models.Events();
                    res.eventId = result["eventid"];
                    res.eventTimestamp = long.Parse(result["eventtimestamp"]);
                    res.eventSource = result["eventsource"];
                    res.eventData = result["eventdata"];
                    res.eventType = result["eventtype"];
                    res.eventCounter = int.Parse(result["eventcounter"]);
                    res.eventDate = DateTimeOffset.FromUnixTimeMilliseconds
                    (
                        long.Parse(result["eventtimestamp"])
                    ).DateTime;
                    events.Add(res);
                }
                Console.WriteLine(events.Count);
            }
            catch (InvalidRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task<List<Dictionary<string, string>>> GetQueryResults(string queryExecutionId)
        {
            List<Dictionary<string, string>> items = new List<Dictionary<string, string>>();
            var results = new GetQueryExecutionResponse();
            var executeQuery = new QueryExecution();

            var request = new GetQueryExecutionRequest()
            {
                QueryExecutionId = queryExecutionId
            };

            do
            {
                try
                {
                    results = await client.GetQueryExecutionAsync(request);
                    executeQuery = results.QueryExecution;

                    Console.WriteLine("Status: {0} {1}", executeQuery.Status.State, executeQuery.Status.StateChangeReason);
                    await Task.Delay(3000);
                }
                catch (InvalidRequestException ex)
                {
                    Console.WriteLine("GetQueryExec Error: {0}", ex.Message);
                }
            } while (executeQuery.Status.State == "RUNNING" || executeQuery.Status.State == "QUEUED");

            var queryResultRequest = new GetQueryResultsRequest()
            {
                QueryExecutionId = queryExecutionId
            };

            var queryResultResponse = new GetQueryResultsResponse();
            do
            {
                queryResultResponse = await client.GetQueryResultsAsync(queryResultRequest);
                foreach (Row row in queryResultResponse.ResultSet.Rows)
                {
                    var dict = new Dictionary<string, string>();
                    for (int i = 0; i < queryResultResponse.ResultSet.ResultSetMetadata.ColumnInfo.Count; i++)
                    {
                        dict.Add
                        (
                            queryResultResponse.ResultSet.ResultSetMetadata.ColumnInfo[i].Name,
                            row.Data[i].VarCharValue
                        );
                    }
                    items.Add(dict);                    
                    if (queryResultResponse.NextToken != null)
                        queryResultRequest.NextToken = queryResultResponse.NextToken;
                }
            } while (queryResultResponse.NextToken != null);
            
            // Removed the column Names from the zero index.
            items.RemoveAt(0);
            return items;
        }
    }
}