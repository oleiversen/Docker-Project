using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RestApplicationWithMongoBackend.Models;

namespace RestApplicationWithMongoBackend.Repository
{
  public class KeyValueContext
  {
    private readonly IMongoDatabase database = null;

    public KeyValueContext(IOptions<Settings> settings)
    {
      var client = new MongoClient(settings.Value.ConnectionString);

      if (client != null)
      {
        database = client.GetDatabase(settings.Value.Database);
      }
    }

    public IMongoCollection<KeyValue> KeyValues
    {
      get
      {
        return database.GetCollection<KeyValue>("KeyValue");
      }
    }

  }
}
