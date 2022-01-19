using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Timestream
{
    public class Timestream
    {
        TimestreamDatabase TimestreamDatabase;
        TimestreamTable TimestreamTable;
        TimestreamInsert TimestreamInsert;
        public Timestream(
            TimestreamDatabase timestreamDatabase,
            TimestreamTable timestreamTable,
            TimestreamInsert timestreamInsert
        )
        {
            TimestreamDatabase = timestreamDatabase;
            TimestreamTable = timestreamTable;
            TimestreamInsert = timestreamInsert;
        }

        public async Task RunProcess()
        {
            try
            {
                await TimestreamDatabase.Main();
                await TimestreamTable.Main();
                await TimestreamInsert.Main();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
            await Task.CompletedTask;
        }




    }
}