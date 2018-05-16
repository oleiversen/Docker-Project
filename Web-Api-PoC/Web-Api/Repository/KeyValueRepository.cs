using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using RestApplicationWithMongoBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestApplicationWithMongoBackend.Repository
{

  public class KeyValueRepository : IKeyValueRepository
  {
    private const int TIMEOUTMS = 10000;

    private KeyValueContext context = null;
    private readonly IOptions<Settings> settings;
    private readonly ILogger logger;
    private int exceptionCount = 0;

    public KeyValueRepository(IOptions<Settings> settings, ILoggerFactory loggerFactory)
    {
      this.settings = settings;
      logger = loggerFactory.CreateLogger("KeyValueRepository");
      CreateMongoContex();
    }

    private void CreateMongoContex()
    {
      exceptionCount = 0;
      context = new KeyValueContext(settings);
    }

    public async Task<IEnumerable<KeyValue>> GetKeyValuesAsync(int limit)
    {
      try
      {
        if (exceptionCount > 10)
        {
          CreateMongoContex();
        }
        using (var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(TIMEOUTMS)))
        {
          var cursor = await context.KeyValues.FindAsync(_ => true, new FindOptions<KeyValue> { Limit = limit }, timeoutCancellationTokenSource.Token);
          return await cursor.ToListAsync();
        }
      }
      catch (Exception ex)
      {
        logger.LogError("GetKeyValueAsync {0}", ex.Message);
        exceptionCount++;
        throw;
      }
    }
    public async Task<KeyValue> GetKeyValueAsync(string id)
    {
      var filter = Builders<KeyValue>.Filter.Eq("Id", id);

      try
      {
        if (exceptionCount > 10)
        {
          CreateMongoContex();
        }
        using (var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(TIMEOUTMS)))
        {
          return await context.KeyValues
                              .Find(filter)
                              .FirstOrDefaultAsync(timeoutCancellationTokenSource.Token);
        }
      }
      catch (Exception ex)
      {
        logger.LogError("GetKeyValueAsync {0}", ex.Message);
        exceptionCount++;
        throw;
      }
    }

    public async Task AddKeyValueAsync(KeyValue item)
    {
      try
      {
        if (exceptionCount > 10)
        {
          CreateMongoContex();
        }
        using (var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(TIMEOUTMS)))
        {
          await context.KeyValues.InsertOneAsync(item, null, timeoutCancellationTokenSource.Token);
        }
      }
      catch (Exception ex)
      {
        logger.LogError("AddKeyValueAsync {0}", ex.Message);
        exceptionCount++;
        throw;
      }
    }

    public async Task<bool> RemoveKeyValueAsync(string id)
    {
      try
      {
        if (exceptionCount > 10)
        {
          CreateMongoContex();
        }
        using (var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(TIMEOUTMS)))
        {
          DeleteResult actionResult = await context.KeyValues.DeleteOneAsync(
              Builders<KeyValue>.Filter.Eq("Id", id), timeoutCancellationTokenSource.Token);
          return actionResult.IsAcknowledged
              && actionResult.DeletedCount > 0;
        }
      }
      catch (Exception ex)
      {
        logger.LogError("RemoveKeyValueAsync {0}", ex.Message);
        exceptionCount++;
        return false;
      }

    }

    public async Task<bool> UpdateKeyValueAsync(string id, KeyValue item)
    {
      var filter = Builders<KeyValue>.Filter.Eq(s => s.Id, id);
      var update = Builders<KeyValue>.Update
                      .Set(s => s.Value, item.Value);
      try
      {
        if (exceptionCount > 10)
        {
          CreateMongoContex();
        }
        using (var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(TIMEOUTMS)))
        {
          UpdateResult actionResult = await context.KeyValues.UpdateOneAsync(filter, update, null, timeoutCancellationTokenSource.Token);

          return actionResult.IsAcknowledged
              && actionResult.ModifiedCount > 0;
        }
      }
      catch (Exception ex)
      {
        logger.LogError("UpdateKeyValueAsync {0}", ex.Message);
        exceptionCount++;
        return false;
      }
    }

    public async Task<bool> RemoveAllKeyValuesAsync()
    {
      try
      {
        if (exceptionCount > 10)
        {
          CreateMongoContex();
        }
        using (var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(TIMEOUTMS)))
        {

          DeleteResult actionResult = await context.KeyValues.DeleteManyAsync(new BsonDocument(), timeoutCancellationTokenSource.Token);
          return actionResult.IsAcknowledged
              && actionResult.DeletedCount > 0;
        }
      }
      catch (Exception ex)
      {
        logger.LogError("RemoveAllKeyValuesAsync {0}", ex.Message);
        exceptionCount++;
        return false;
      }
    }
  }
}
