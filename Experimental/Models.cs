using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;


namespace Experimental
{
    public class Event
    {
        public Event()
        { }
        public Event(string source, string eventType, DateTime eventDate, string eventData, int counter)
        {
            this.source = source;
            this.eventType = eventType;
            this.eventDate = eventDate;
            this.eventData = eventData;
            this.counter = counter;
            this.eventId = $"{source}_{eventType}";
            this.eventTimestamp = DateTimeToTimestamp(eventDate);
        }
        [DynamoDBProperty("eventSource")]
        public string source;
        public string eventType;
        public DateTime eventDate
        {
            get
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(eventTimestamp).DateTime;
            }
            set
            {

            }
        }
        public string eventId;
        public long eventTimestamp;
        public string eventData;
        [DynamoDBProperty("eventCounter")]
        public int counter;

        public static long DateTimeToTimestamp(DateTime dateTime) =>
            new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

        public static DateTime TimestampToDateTime(long timestamp) =>
            DateTimeOffset.FromUnixTimeSeconds(0).DateTime;
    }
}
