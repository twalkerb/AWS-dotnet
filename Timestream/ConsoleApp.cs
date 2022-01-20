using System;
using System.Threading.Tasks;

namespace Timestream
{
    public class ConsoleApp
    {
        Timestream Timestream;
        SampleTimestream SampleTimestream;
        public ConsoleApp(Timestream timestream, SampleTimestream sampleTimestream)
        {
            Timestream = timestream;
            SampleTimestream = sampleTimestream;
        }

        public async Task Run()
        {
            try
            {
                // await Timestream.RunProcess();
                await SampleTimestream.MainAsync();
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