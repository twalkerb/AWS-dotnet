using MongoDB.Bson;

namespace SampleMigration.Web
{
    public class CountLogger
    {
        public ObjectId id { get; set; }
        public string EventDate { get; set; }
        public string Comment { get; set; }
    }
}