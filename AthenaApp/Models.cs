using System;
using Amazon;
using Amazon.Runtime;
using Amazon.Athena;
using Amazon.Athena.Model;

namespace AthenaApp.Models
{
    public class Events
    {
        public string eventId;
        public long eventTimestamp;
        public string eventData;
        public string eventSource;
        public string eventType;
        public int eventCounter;
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
    }
}