using RestApplicationWithMongoBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApplicationWithMongoBackend.Repository
{
  public interface IKeyValueRepository
  {
    Task AddKeyValueAsync(KeyValue item);
    Task<IEnumerable<KeyValue>> GetKeyValuesAsync(int limit);
    Task<KeyValue> GetKeyValueAsync(string id);
    Task<bool> RemoveAllKeyValuesAsync();
    Task<bool> RemoveKeyValueAsync(string id);
    Task<bool> UpdateKeyValueAsync(string id, KeyValue item);
  }
}