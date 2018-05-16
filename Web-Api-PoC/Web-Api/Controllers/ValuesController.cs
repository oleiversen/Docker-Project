using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestApplicationWithMongoBackend.Models;
using RestApplicationWithMongoBackend.Repository;

namespace RestApplicationWithMongoBackend.Controllers
{
  [Route("api/[controller]")]
  public class ValuesController : Controller
  {
    private readonly IKeyValueRepository keyValueRepository;
    private readonly ILogger logger;

    public ValuesController(IKeyValueRepository keyValueRepository, ILoggerFactory loggerFactory)
    {
      this.keyValueRepository = keyValueRepository;
      logger = loggerFactory.CreateLogger("ValuesController");

    }

    // GET api/values?limit=50
    [HttpGet]
    public async Task<IActionResult> Get(int limit = 1)
    {
      try
      {
        logger.LogInformation("Get limit {0}", limit);
        return new ObjectResult(await keyValueRepository.GetKeyValuesAsync(limit));
      }
      catch (Exception ex)
      {
        logger.LogError("Get {0}", ex.Message);
      }
      return StatusCode(500);
    }


    // GET api/values/9D9A691A-AE95-45A4-A423-08DD1A69D0D1
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
      try
      {
        logger.LogInformation("Get id:{0}", id);
        var result = await keyValueRepository.GetKeyValueAsync(id);
        if (result != null)
        {
          return new ObjectResult(result);
        }
        return NotFound();
      }
      catch (Exception ex)
      {
        logger.LogError("Get {0}", ex.Message);
      }
      return StatusCode(500);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]KeyValue keyValue)
    {
      try
      {
        if (keyValue == null)
        {
          await Console.Error.WriteLineAsync("keyValue == null");
          logger.LogError("keyValue == null");
        }
        logger.LogInformation("Post id:{0} value:{1}", keyValue.Id, keyValue.Value);
        await keyValueRepository.AddKeyValueAsync(new KeyValue
        {
          Id = keyValue.Id,
          Value = keyValue.Value
        });
        return Ok();
      }
      catch (Exception ex)
      {
        logger.LogError("Post {0}", ex.Message);
      }
      return StatusCode(500);
    }

    // PUT api/values/
    // body: 
    // {
    //   "id": "9D9A691A-AE95-45A4-A423-08DD1A69D0D1",
    //   "value": "Hallo World"
    // }
    [HttpPut()]
    public async Task<IActionResult> Put([FromBody]KeyValue keyValue)
    {
      try
      {
        logger.LogInformation("Put id:{0} value:{1}", keyValue.Id, keyValue.Value);
        await keyValueRepository.UpdateKeyValueAsync(keyValue.Id, new KeyValue
        {
          Id = keyValue.Id,
          Value = keyValue.Value
        });
        return Ok();
      }
      catch (Exception ex)
      {
        logger.LogError("Put {0}", ex.Message);
      }
      return StatusCode(500);
    }

    // DELETE api/values/9D9A691A-AE95-45A4-A423-08DD1A69D0D1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      try
      {
        logger.LogInformation("delete id:{0}", id);
        await keyValueRepository.RemoveKeyValueAsync(id);
        return Ok();
      }
      catch (Exception ex)
      {
        logger.LogError("Delete {0}", ex.Message);
      }
      return StatusCode(500);
    }
  }
}
