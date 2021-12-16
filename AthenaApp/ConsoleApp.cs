using System;
using System.Threading.Tasks;

namespace AthenaApp
{
    public class ConsoleApp
    {
        AthenaQuery AthenaQuery;
        
        public ConsoleApp(AthenaQuery athenaQuery)
        {
            AthenaQuery = athenaQuery;
        }

        public async Task Run()
        {
            try
            {
                await AthenaQuery.RunSampleQuery();
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