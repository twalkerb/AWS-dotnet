using System.Collections.Generic;

namespace InvocationPayloadFix
{
    public class DummyData
    {
        public List<CtRoot> ctRoot { get; set; }
    }

    public class CtRoot
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string dob { get; set; }
        public string telephone { get; set; }
        public string[] pets { get; set; }
        public float score { get; set; }
        public string email { get; set; }
        public string url { get; set; }
        public string verified { get; set; }
        public string description { get; set; }
        public float salary { get; set; }
        public address address { get; set; }
    }

    public class address
    {
        public string street { get; set; }
        public string town { get; set; }
        public string postcode { get; set; }
    }
}