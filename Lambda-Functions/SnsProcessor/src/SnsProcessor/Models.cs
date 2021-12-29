using System;
using System.IO;

namespace SnsProcessor
{
    public class EventMessage
    {
        public string eventId { get; set; }
        public string eventMessageId {get; set;}
        public string eventSubject {get; set;}
        public string eventMessageText { get; set; }
        public long eventTimestamp { get; set; }
        public string eventTopicArn {get; set;}        
    }    
}
