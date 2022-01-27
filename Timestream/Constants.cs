using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timestream
{
    public class Constants
    {      
        public const string databaseName = "telemetryDB";
        public const string tableName = "diwali-event-data-2701";
        public const string filePath = "~DataFile.txt";        
        public const string s3ErrorReportBucketName = "my-learners-bucket";
        public const long HT_TTL_HOURS = 12;
        public const long CT_TTL_DAYS = 365;
    }
}