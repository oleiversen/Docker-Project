using MongoDB.Bson.Serialization.Attributes;
using System;

namespace RestApplicationWithMongoBackend.Models
{
  public class KeyValue
  {
    [BsonId]
    public String Id { get; set; }
    public string Value { get; set; }
  }
}
