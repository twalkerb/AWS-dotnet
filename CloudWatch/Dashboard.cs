using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;

namespace CloudWatch
{
    public class Dashboard
    {
        private readonly AmazonCloudWatchClient client;
                
        public Dashboard()
        {
            client = new AmazonCloudWatchClient
            (
                Amazon.RegionEndpoint.USEast2
            );
        }

        public async Task Run()
        {
            try
            {
                var dashboards = await ListDashboards();
                await GetDashboard(dashboards);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
            await Task.CompletedTask;
        }

        public async Task<List<DashboardEntry>> ListDashboards()
        {
            var request = new ListDashboardsRequest();
            var response = await client.ListDashboardsAsync(request);
            return response.DashboardEntries;
        }

        public async Task GetDashboard(List<DashboardEntry> dashboards)
        {
            var dashboardName = dashboards[0].DashboardName;
            var request = new GetDashboardRequest
            {
                DashboardName = dashboardName,
            };
            var response = await client.GetDashboardAsync(request);
        }
    }
}