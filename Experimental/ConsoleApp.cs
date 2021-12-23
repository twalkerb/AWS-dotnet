using System;
using System.Threading.Tasks;

namespace Experimental
{
    public class ConsoleApp
    {
        
        DynamoDb DynamoDb;
        public ConsoleApp(DynamoDb dynamoDb)
        {
            DynamoDb = dynamoDb;
        }

        public async Task Run()
        {
            try
            {
                await DynamoDb.Run();
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