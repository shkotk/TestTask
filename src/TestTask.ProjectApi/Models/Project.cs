using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TestTask.ProjectApi.Models;

public class Project
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public ICollection<Chart> Charts { get; set; }

    public class Chart
    {
        public string Symbol { get; set; }
        public string TimeFrame { get; set; }
        public ICollection<Indicator> Indicators { get; set; }

        public class Indicator
        {
            public string Name { get; set; }
            public string Parameters { get; set; }
        }
    }
}