using System;
using System.Threading.Tasks;

namespace Timestream
{
    public class ConsoleApp
    {
        Timestream Timestream;
        public ConsoleApp(Timestream timestream)
        {
            Timestream = timestream;
        }

        public async Task Run()
        {
            try
            {
                await Timestream.RunProcess();
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