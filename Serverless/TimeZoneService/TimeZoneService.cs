using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeZoneService
{
    public class TimeZoneService
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public TimeSpan DstOffset { get; set; }
        public TimeSpan Offset { get; set; }
        public string TimeZoneId { get; set; }
    }
}