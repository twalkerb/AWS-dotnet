using System;
using System.Threading.Tasks;

namespace CloudWatch
{
    public class ConsoleApp
    {
        
        Dashboard Dashboard;
        public ConsoleApp(Dashboard dashboard)
        {
            Dashboard = dashboard;
        }

        public async Task Run()
        {
            try
            {
                await Dashboard.Run();
            }            
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
            await Task.CompletedTask;
        }       
    } 
}