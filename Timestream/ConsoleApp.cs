using System;
using System.Threading.Tasks;

namespace Timestream
{
    public class ConsoleApp
    {
        public ConsoleApp()
        {
            
        }

        public async Task Run()
        {
            try
            {
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